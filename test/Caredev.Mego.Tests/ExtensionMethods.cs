using Caredev.Mego.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;

namespace Caredev.Mego.Tests
{
    public static class ExtensionMethods
    {
        public static void NoCommit(this DbContext context, Action<DbConnection, DbTransaction> action)
        {
            var con = context.Database.Connection;
            con.Open();
            var tran = con.BeginTransaction();
            try
            {
                action(con, tran);
            }
            finally
            {
                tran.Rollback();
                con.Close();
            }
        }

        public static void AddParameter(this DbCommand com, string name, object value)
        {
            var p = com.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            com.Parameters.Add(p);
        }

        public static int ExecuteNonQuery(this DbConnection connection, string sql, params object[] parameters)
        {
            var com = CreateCommand(connection, sql, parameters);
            return com.ExecuteNonQuery();
        }

        public static T ExecuteScalar<T>(this DbConnection connection, string sql, params object[] parameters)
        {
            var com = CreateCommand(connection, sql, parameters);
            return (T)Convert.ChangeType(com.ExecuteScalar(), typeof(T));
        }

        private static DbCommand CreateCommand(DbConnection connection, string sql, object[] parameters)
        {
            var com = connection.CreateCommand();
            com.CommandText = sql;
            for (int i = 0; i < parameters.Length; i++)
            {
                com.AddParameter("@p" + i.ToString(), parameters[i]);
            }
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
            return com;
        }

        public static void ConcurrencyTest(this DbContext db, Action action)
        {
            if (!db.Database.Provider.IsExclusive)
            {
                try
                {
                    action();
                    Assert.Fail();
                }
                catch (DbCommitConcurrencyException e)
                {

                }
                catch (DbException e)
                {

                }
            }
        }
    }
}