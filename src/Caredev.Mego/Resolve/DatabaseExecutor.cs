// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0.
// See License.txt in the project root for license information.
namespace Caredev.Mego.Resolve
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Transactions;
    using Res = Properties.Resources;
    /// <summary>
    /// 数据库执行对象。
    /// </summary>
    internal class DatabaseExecutor : IDisposable
    {
        private readonly DbProviderFactory _ProviderFactory;
        private readonly IDbAccessProvider _AccessProvider;
        private Transaction _LastTransaction;
        private DbTransaction _LocalTransaction;
        private bool _ContextOwnsConnection = true;
        private bool _OpenedConnection = false;
        private int _ConnectionRequestCount = 0;
        /// <summary>
        /// 创建数据库执行对象。
        /// </summary>
        /// <param name="provider">数据提供程序。</param>
        /// <param name="connection">当前连接对象。</param>
        /// <param name="contextOwnsConnection">当前数据上下文是否拥有连接对象。</param>
        internal DatabaseExecutor(IDbAccessProvider provider, DbConnection connection, bool contextOwnsConnection)
        {
            _AccessProvider = provider;
            _ProviderFactory = provider.Factory;
            Connection = connection;
            Connection.StateChange += ConnectionStateChange;
            _ContextOwnsConnection = contextOwnsConnection;
        }
        /// <summary>
        /// 当前连接对象。
        /// </summary>
        internal DbConnection Connection { get; }
        /// <summary>
        /// 使用指定的事务。
        /// </summary>
        /// <param name="transaction"></param>
        internal void UseTransaction(DbTransaction transaction)
        {
            if (_disposed)
            {
                throw new InvalidOperationException(Res.ExceptionObjectIsDisposed);
            }
            _LocalTransaction = transaction;
        }
        /// <summary>
        /// 使用当前连接开始事务处理。
        /// </summary>
        /// <returns>开始的事务对象。</returns>
        internal DbTransaction BeginTransaction()
        {
            EnsureConnection();
            _LocalTransaction = Connection.BeginTransaction();
            return _LocalTransaction;
        }
        /// <summary>
        /// 在事务中执行指定函数。
        /// </summary>
        /// <typeparam name="T">返回值类型。</typeparam>
        /// <param name="func">需要执行的函数。</param>
        /// <param name="startLocalTransaction">是否开始本地事务。</param>
        /// <returns>返回执行函数的结果。</returns>
        internal T ExecuteInTransaction<T>(Func<T> func, bool startLocalTransaction = true)
        {
            try
            {
                EnsureConnection();
                var needLocalTransaction = false;
                if (_LocalTransaction == null && _LastTransaction == null)
                {
                    needLocalTransaction = startLocalTransaction;
                }
                DbTransaction localTransaction = null;
                if (needLocalTransaction)
                {
                    //内部创建事务并提交
                    localTransaction = Connection.BeginTransaction();
                    _LocalTransaction = localTransaction;
                    try
                    {
                        var result = func();
                        localTransaction.Commit();
                        return result;
                    }
                    catch
                    {
                        localTransaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        _LocalTransaction = null;
                    }
                }
                else
                {
                    return func();
                }
            }
            finally
            {
                ReleaseConnection();
            }
        }
        /// <summary>
        /// 使用指定的SQL语句创建命令对象。
        /// </summary>
        /// <param name="sql">指定SQL语句。</param>
        /// <returns>创建的命令对象。</returns>
        internal DbCommand CreateCommand(string sql)
        {
            var command = Connection.CreateCommand();
            if (_LocalTransaction != null)
            {
                command.Transaction = _LocalTransaction;
            }
            command.CommandText = sql;
            return command;
        }
        /// <summary>
        /// 使用指定的SQL语句及用户参数创建命令对象。
        /// </summary>
        /// <param name="sql">指定SQL语句。</param>
        /// <param name="parameters">参数集合对象。</param>
        /// <returns>创建的命令对象。</returns>
        internal DbCommand CreateCommand(string sql, params object[] parameters)
        {
            var command = CreateCommand(sql);
            var dbParameters = new DbParameter[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var para = command.CreateParameter();
                para.ParameterName = "p" + i.ToString();
                para.Value = parameters[i] ?? DBNull.Value;
                dbParameters[i] = para;
            }
            command.Parameters.AddRange(dbParameters);
            return command;
        }
        //连接状态发生改变时回调。
        private void ConnectionStateChange(object sender, StateChangeEventArgs e)
        {
            if (e.CurrentState == ConnectionState.Closed)
            {
                _ConnectionRequestCount = 0;
                _OpenedConnection = false;
            }
        }
        //确认上下文连接登记到当前分布式事务中。
        private Transaction EnsureContextIsEnlistedInCurrentTransaction(Transaction currentTransaction)
        {
            if (Connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException(Res.ExceptionConnectionStateError);
            }
            // TABLE OF ACTIONS WE PERFORM HERE:
            //
            //  #  lastTransaction     currentTransaction         ConnectionState   WillClose      Action                                  Behavior when no explicit transaction (started with .ElistTransaction())     Behavior with explicit transaction (started with .ElistTransaction())
            //  1   null                null                       Open              No             no-op;                                  implicit transaction will be created and used                                explicit transaction should be used
            //  2   non-null tx1        non-null tx1               Open              No             no-op;                                  the last transaction will be used                                            N/A - it is not possible to EnlistTransaction if another transaction has already enlisted
            //  3   null                non-null                   Closed            Yes            connection.Open();                      Opening connection will automatically enlist into Transaction.Current        N/A - cannot enlist in transaction on a closed connection
            //  4   null                non-null                   Open              No             connection.Enlist(currentTransaction);  currentTransaction enlisted and used                                         N/A - it is not possible to EnlistTransaction if another transaction has already enlisted
            //  5   non-null            null                       Open              No             no-op;                                  implicit transaction will be created and used                                explicit transaction should be used
            //  6   non-null            null                       Closed            Yes            no-op;                                  implicit transaction will be created and used                                N/A - cannot enlist in transaction on a closed connection
            //  7   non-null tx1        non-null tx2               Open              No             connection.Enlist(currentTransaction);  currentTransaction enlisted and used                                         N/A - it is not possible to EnlistTransaction if another transaction has already enlisted
            //  8   non-null tx1        non-null tx2               Open              Yes            connection.Close(); connection.Open();  Re-opening connection will automatically enlist into Transaction.Current     N/A - only applies to TransactionScope - requires two transactions and CommitableTransaction and TransactionScope cannot be mixed
            //  9   non-null tx1        non-null tx2               Closed            Yes            connection.Open();                      Opening connection will automatcially enlist into Transaction.Current        N/A - cannot enlist in transaction on a closed connection
            var transactionHasChanged = (null != currentTransaction && !currentTransaction.Equals(_LastTransaction)) ||
                                        (null != _LastTransaction && !_LastTransaction.Equals(currentTransaction));
            if (transactionHasChanged)
            {
                if (!_OpenedConnection)
                {
                    // We didn't open the connection so, just try to enlist the connection in the current transaction. 
                    // Note that the connection can already be enlisted in a transaction (since the user opened 
                    // it s/he could enlist it manually using EntityConnection.EnlistTransaction() method). If the 
                    // transaction the connection is enlisted in has not completed (e.g. nested transaction) this call 
                    // will fail (throw). Also currentTransaction can be null here which means that the transaction
                    // used in the previous operation has completed. In this case we should not enlist the connection
                    // in "null" transaction as the user might have enlisted in a transaction manually between calls by 
                    // calling EntityConnection.EnlistTransaction() method. Enlisting with null would in this case mean "unenlist" 
                    // and would cause an exception (see above). Had the user not enlisted in a transaction between the calls
                    // enlisting with null would be a no-op - so again no reason to do it. 
                    if (currentTransaction != null)
                    {
                        Connection.EnlistTransaction(currentTransaction);
                    }
                }
                else if (_ConnectionRequestCount > 1)
                {
                    // We opened the connection. In addition we are here because there are multiple
                    // active requests going on (read: enumerators that has not been disposed yet) 
                    // using the same connection. (If there is only one active request e.g. like SaveChanges
                    // or single enumerator there is no need for any specific transaction handling - either
                    // we use the implicit ambient transaction (Transaction.Current) if one exists or we 
                    // will create our own local transaction. Also if there is only one active request
                    // the user could not enlist it in a transaction using EntityConnection.EnlistTransaction()
                    // because we opened the connection).
                    // If there are multiple active requests the user might have "played" with transactions
                    // after the first transaction. This code tries to deal with this kind of changes.

                    if (null == _LastTransaction)
                    {
                        Debug.Assert(currentTransaction != null, "transaction has changed and the lastTransaction was null");

                        // Two cases here: 
                        // - the previous operation was not run inside a transaction created by the user while this one is - just
                        //   enlist the connection in the transaction
                        // - the previous operation ran withing explicit transaction started with EntityConnection.EnlistTransaction()
                        //   method - try enlisting the connection in the transaction. This may fail however if the transactions 
                        //   are nested as you cannot enlist the connection in the transaction until the previous transaction has
                        //   completed.
                        Connection.EnlistTransaction(currentTransaction);
                    }
                    else
                    {
                        // We'll close and reopen the connection to get the benefit of automatic transaction enlistment.
                        // Remarks: We get here only if there is more than one active query (e.g. nested foreach or two subsequent queries or SaveChanges
                        // inside a for each) and each of these queries are using a different transaction (note that using TransactionScopeOption.Required 
                        // will not create a new transaction if an ambient transaction already exists - the ambient transaction will be used and we will 
                        // not end up in this code path). If we get here we are already in a loss-loss situation - we cannot enlist to the second transaction
                        // as this would cause an exception saying that there is already an active transaction that needs to be committed or rolled back
                        // before we can enlist the connection to a new transaction. The other option (and this is what we do here) is to close and reopen
                        // the connection. This will enlist the newly opened connection to the second transaction but will also close the reader being used
                        // by the first active query. As a result when trying to continue reading results from the first query the user will get an exception
                        // saying that calling "Read" on a closed data reader is not a valid operation.
                        Connection.Close();
                        Connection.Open();
                        _OpenedConnection = true;
                    }
                }
            }
            return currentTransaction;
        }
        //确认连接可用。
        private void EnsureConnection()
        {
            if (_disposed)
            {
                throw new InvalidOperationException(Res.ExceptionObjectIsDisposed);
            }
            var connection = Connection;
            if (connection.State == ConnectionState.Broken)
            {
                Connection.Close();
            }
            if (connection.State == ConnectionState.Closed)
            {
                Connection.Open();
                _OpenedConnection = true;
            }
            if (_OpenedConnection)
            {
                _ConnectionRequestCount++;
            }
            _LastTransaction = EnsureContextIsEnlistedInCurrentTransaction(Transaction.Current);
        }
        //释放连接资源，如果当前是独占数据库，则不会释放连接，直接释放整个数据上下文为止。
        private void ReleaseConnection()
        {
            if (_OpenedConnection && !_AccessProvider.IsExclusive)
            {
                if (_ConnectionRequestCount > 0)
                {
                    _ConnectionRequestCount--;
                }
                if (_ConnectionRequestCount == 0)
                {
                    Connection.Close();
                    _LocalTransaction = null;
                    _OpenedConnection = false;
                }
                _LastTransaction = null;
            }
        }
        /// <summary>
        /// <see cref="IDisposable"/>接口实现。
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                var connection = Connection;
                if (connection != null)
                {
                    connection.StateChange -= ConnectionStateChange;
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                    if (_ContextOwnsConnection)
                    {
                        connection.Dispose();
                    }
                }
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
        private bool _disposed = false;
    }
}