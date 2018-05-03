
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

        public const string MaxParameterCountTestSql = "SELECT * FROM Customers WHERE Id IN ";
        public const string ParameterPrefix = "@p";

        public string MaxInsertRowCountTestSql(string name, int count)
        {
            var builder = new StringBuilder();
            return "SELECT TOP 1 * FROM Customers";
        }

        public void CreateDatabaseIfNoExsits(DbContext context)
        {
            var stringbuilder = new OleDbConnectionStringBuilder(context.Database.Connection.ConnectionString);
            var database = stringbuilder.DataSource;
            var file = new FileInfo(typeof(DbFeatureTest).Assembly.Location);
            var fullname = Path.Combine(file.Directory.FullName, database);
            if (!File.Exists(fullname))
            {
                var ext = Path.GetExtension(fullname);
                byte[] filecontent = null;
                if (ext == ".xlsx")
                {
                    filecontent = Properties.Resources.EmptyExcel2010;
                }
                else
                {
                    filecontent = Properties.Resources.EmptyExcel2003;
                }
                File.WriteAllBytes(fullname, filecontent);
            }
        }
    }
}