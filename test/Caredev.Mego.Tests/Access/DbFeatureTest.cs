
namespace Caredev.Mego.Tests.Core
{
    using System;
    using System.Text;
    using System.Data.OleDb;
    using System.IO;

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
            builder.AppendLine(@"CREATE TABLE [" + name + "] (Id INTEGER);");
            return builder.ToString();
            builder.AppendLine(@"INSERT  INTO [" + name + "] ( [Id] )");

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
            var stringbuilder = new OleDbConnectionStringBuilder(context.Database.Connection.ConnectionString);
            var database = stringbuilder.DataSource;
            var file = new FileInfo(typeof(DbFeatureTest).Assembly.Location);
            var fullname = Path.Combine(file.Directory.FullName, database);
            if (File.Exists(fullname))
            {
                throw new InvalidOperationException($"Please must manually create the file [{fullname}]");
            }
        }
    }
}