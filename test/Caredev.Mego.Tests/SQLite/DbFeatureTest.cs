
namespace Caredev.Mego.Tests.Core
{
    using System.Text;
    using System.Data.SQLite;

    public partial class DbFeatureTest
    {
        public const int MaxInsertRowCount = 1000;
        public const int MaxParameterCount = 999;

        public const bool HasMaxInsertRowCount = false;
        public const bool HasMaxParameterCount = true;

        public const string MaxParameterCountTestSql = "SELECT 1";
        public string MaxInsertRowCountTestSql(string name, int count)
        {
            var builder = new StringBuilder();
            builder.AppendLine(@"CREATE TEMPORARY TABLE `" + name + "`([Id] INT NULL);");
            builder.AppendLine(@"INSERT  INTO `" + name + "`( [Id] )");

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
            var stringbuilder = new SQLiteConnectionStringBuilder(context.Database.Connection.ConnectionString);
            if (!System.IO.File.Exists(stringbuilder.DataSource))
            {
                SQLiteConnection.CreateFile(stringbuilder.DataSource);
            }
        }
    }
}