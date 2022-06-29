using MySql.Data;
using MySql.Data.MySqlClient;

namespace University_CRM
{
    internal class DB
    {
       
        public static MySqlConnection GetConnection()
        {
            MySqlConnection conn = new MySqlConnection("server=localhost;uid=root;pwd=root;database=university");
            conn.Open();
            return conn;
        }

        public static void CloseConnection(MySqlConnection conn)
        {
            conn.Close();
        }
    }
}
