﻿
namespace Caredev.Mego.Tests.Core
{
    using System.Text;
    using Npgsql;
    public partial class DbFeatureTest
    {
        public const int MaxInsertRowCount = 1000;
        public const int MaxParameterCount = 3000;

        public const bool HasMaxInsertRowCount = false;
        public const bool HasMaxParameterCount = false;
        
        public const string MaxParameterCountTestSql = @"SELECT * FROM public.""Customers"" WHERE ""Id"" IN ";
        public const string ParameterPrefix = "@p";
        public string MaxInsertRowCountTestSql(string name, int count)
        {
            var builder = new StringBuilder();
            builder.AppendLine(@"CREATE TEMPORARY TABLE """ + name + @"""(Id integer NULL);");
            builder.AppendLine(@"INSERT  INTO """ + name + @"""( Id )");

            builder.AppendLine(@"VALUES");
            builder.Append("(0)");
            for (int i = 1; i < count; i++)
            {
                builder.AppendFormat(",({0})", i);
            }

            builder.Append(";");
            return builder.ToString();
        }

        public void CreateDatabaseIfNoExsits(DbContext context)
        {
            var stringbuilder = new NpgsqlConnectionStringBuilder(context.Database.Connection.ConnectionString);
            var database = stringbuilder.Database;
            stringbuilder.Database = "postgres";
            using (var con = new NpgsqlConnection(stringbuilder.ToString()))
            {
                if (con.ExecuteScalar<int>("SELECT COUNT(1) FROM pg_database WHERE datname = @p0", database) == 0)
                {
                    con.ExecuteNonQuery("CREATE DATABASE " + database);
                }
            }
        }
    }
}