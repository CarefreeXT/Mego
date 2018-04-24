
namespace Caredev.Mego.Tests.Core
{
    using System.Text;
    using System.IO;
    using System;
    using FirebirdSql.Data.FirebirdClient;
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
            var builder = new FbConnectionStringBuilder(context.Database.Connection.ConnectionString);
            //%ProgramFiles%\Firebird\Firebird_2_5\
            var path = builder.Database + ".FDB";
            if (!File.Exists(path))
            {
                throw new InvalidOperationException();
            }
        }
    }
}