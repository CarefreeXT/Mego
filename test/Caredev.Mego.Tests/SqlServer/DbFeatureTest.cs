
namespace Caredev.Mego.Tests.Core
{
    using System.Text;
    using System.Data.SqlClient;

    public partial class DbFeatureTest
    {
        public const int MaxInsertRowCount = 1000;
        public const int MaxParameterCount = 2098;

#if SQLSERVER2012
        public const bool HasMaxInsertRowCount = true;
#else
        public const bool HasMaxInsertRowCount = false;
#endif
        public const bool HasMaxParameterCount = true;

        public const string MaxParameterCountTestSql = "SELECT 1";
        public string MaxInsertRowCountTestSql(string name, int count)
        {
            var builder = new StringBuilder();
            builder.AppendLine(@"DECLARE @" + name + " AS TABLE(Id INT NULL);");
            builder.AppendLine(@"INSERT  INTO @" + name + "( [Id] )");

#if SQLSERVER2005
            builder.Append("SELECT 0");
            for (int i = 1; i < count; i++)
            {
                builder.AppendFormat(" UNION ALL SELECT {0}", i);
            }
#else
            builder.AppendLine(@"VALUES");
            builder.Append("(0)");
            for (int i = 1; i < count; i++)
            {
                builder.AppendFormat(",({0})", i);
            }
#endif

            builder.Append(";");
            return builder.ToString();
        }

        public void CreateDatabaseIfNoExsits(DbContext context)
        {
            var stringbuilder = new SqlConnectionStringBuilder(context.Database.Connection.ConnectionString);
            var database = stringbuilder.InitialCatalog;
            stringbuilder.InitialCatalog = "master";
            stringbuilder.AttachDBFilename = string.Empty;
            using (var con = new SqlConnection(stringbuilder.ToString()))
            {
                if (con.ExecuteScalar<int>("SELECT COUNT(1) FROM sys.databases WHERE name = @p0", database) == 0)
                {
                    con.ExecuteNonQuery("CREATE DATABASE " + database);
                }
            }
        }
    }
}