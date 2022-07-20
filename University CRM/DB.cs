using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace University_CRM
{
    internal class DB
    {
       
        public static MySqlConnection GetConnection()
        {
            MySqlConnection conn = new MySqlConnection("server=db-mysql-fra1-23926-do-user-11906132-0.b.db.ondigitalocean.com;" +
                                                       "port=25060;" +
                                                       "uid=doadmin;" +
                                                       "pwd=AVNS_81E2oOjULm7AddEr_8Q;" +
                                                       "database=university;");
            conn.Open();
            return conn;
        }

        public static async Task<MySqlConnection> GetAsyncConnection()
        {
            MySqlConnection conn = new MySqlConnection("server=db-mysql-fra1-23926-do-user-11906132-0.b.db.ondigitalocean.com;" +
                                                       "port=25060;" +
                                                       "uid=doadmin;" +
                                                       "pwd=AVNS_81E2oOjULm7AddEr_8Q;" +
                                                       "database=university;");
            await conn.OpenAsync();
            return conn;
        }

        public static void CloseConnection(MySqlConnection conn)
        {
            conn.Close();
        }


    }
}
