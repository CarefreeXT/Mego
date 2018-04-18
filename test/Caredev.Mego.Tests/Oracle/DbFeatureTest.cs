
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

        public const string MaxParameterCountTestSql = "SELECT 1";
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

        }
    }
}