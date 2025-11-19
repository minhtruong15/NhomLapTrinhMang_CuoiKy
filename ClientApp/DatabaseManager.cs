using MySql.Data.MySqlClient;
using System.Data;

namespace ClientApp
{
    public static class DatabaseManager
    {
        private static string connStr = "Server=localhost;Database=chatdb;Uid=root;Pwd=1582005;";

        public static DataTable ExecuteQuery(string sql)
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    var adp = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adp.Fill(dt);
                    return dt;
                }
            }
        }

        public static int ExecuteNonQuery(string sql)
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
