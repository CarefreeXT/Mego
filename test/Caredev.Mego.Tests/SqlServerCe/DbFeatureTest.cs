
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
        public const bool HasMaxParameterCount = false;

        public const string MaxParameterCountTestSql = @"SELECT * FROM [Customers] WHERE [Id] IN ";
        public const string ParameterPrefix = "@p";

        public string MaxInsertRowCountTestSql(string name, int count)
        {
            return "SELECT 1";
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