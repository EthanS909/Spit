using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Entity;

namespace Spit
{
    public class Database
    {
        SQLiteConnection sqlite_conn;
        public Database()
        {
            sqlite_conn = CreateConnection();
        }

        private SQLiteConnection CreateConnection()
        {
            sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True;");

            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {

            }
            return sqlite_conn;
        }
    }
}
