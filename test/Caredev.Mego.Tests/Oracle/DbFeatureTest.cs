
namespace Caredev.Mego.Tests.Core
{
    using System.Text;
    using Oracle.ManagedDataAccess.Client;

    public partial class DbFeatureTest
    {
        public const int MaxInsertRowCount = 1000;
        public const int MaxParameterCount = 3000;

        public const bool HasMaxInsertRowCount = false;
        public const bool HasMaxParameterCount = false;

        public const string MaxParameterCountTestSql = @"SELECT * FROM ""SIMPLE"".""Customers"" WHERE ""Id"" IN";
        public string MaxInsertRowCountTestSql(string name, int count)
        {
            var builder = new StringBuilder();
            builder.AppendLine("BEGIN");
            builder.AppendLine(@"CREATE GLOBAL TEMPORARY TABLE " + name + @"(Id NUMBER);");
            builder.AppendLine(@"INSERT ALL");
            for (int i = 0; i < count; i++)
            {
                builder.AppendLine(@"INTO " + name + @" ( Id ) VALUES (" + i.ToString() + ")");
            }
            builder.AppendLine("SELECT * FROM dual;");
            builder.AppendLine("END;");
            return builder.ToString();
        }

        public void CreateDatabaseIfNoExsits(DbContext context)
        {
            var stringbuilder = new OracleConnectionStringBuilder(context.Database.Connection.ConnectionString);
            var database = stringbuilder.UserID.ToUpper();
            stringbuilder.UserID = "SYSTEM";
            using (var con = new OracleConnection(stringbuilder.ToString()))
            {
                if (con.ExecuteScalar<int>("SELECT COUNT(1) FROM ALL_USERS WHERE USERNAME=:p0", database) == 0)
                {
                    con.ExecuteNonQuery($"CREATE USER {database} IDENTIFIED BY manager");
                    con.ExecuteNonQuery($"GRANT DBA TO  {database}");
                }
            }
        }
    }
}