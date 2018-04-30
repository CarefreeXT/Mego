
namespace Caredev.Mego.Tests.Core
{
    using System.Text;
    using System.Data.SqlServerCe;
    using System.IO;
    public partial class DbFeatureTest
    {
        public const int MaxInsertRowCount = 1000;
        public const int MaxParameterCount = 2098;

        public const bool HasMaxInsertRowCount = false;
        public const bool HasMaxParameterCount = true;

        public const string MaxParameterCountTestSql = "SELECT 1";
        public const string ParameterPrefix = "@p";

        public string MaxInsertRowCountTestSql(string name, int count)
        {
            var builder = new StringBuilder();
            builder.AppendLine(@"DECLARE @" + name + " AS TABLE(Id INT NULL);");
            builder.AppendLine(@"INSERT  INTO @" + name + "( [Id] )");
            
            builder.Append("SELECT 0");
            for (int i = 1; i < count; i++)
            {
                builder.AppendFormat(" UNION ALL SELECT {0}", i);
            }

            builder.Append(";");
            return builder.ToString();
        }

        public void CreateDatabaseIfNoExsits(DbContext context)
        {
            var stringbuilder = new SqlCeConnectionStringBuilder(context.Database.Connection.ConnectionString);
            var database = stringbuilder.DataSource;
            if (!File.Exists(database))
            {
                new SqlCeEngine(stringbuilder.ToString()).CreateDatabase();
            }
        }
    }
}