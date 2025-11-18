using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ServerApp
{
    public static class Database
    {
        private static readonly string connectionString =
            "Server=127.0.0.1;Port=3306;Database=chatdb;Uid=root;Pwd=1582005;";

        // ---- KIỂM TRA KẾT NỐI ----
        public static bool TestConnection()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // ---- ĐĂNG NHẬP ----
        public static Tuple<string, string> Authenticate(string username, string password)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT username, role FROM users WHERE username=@u AND password=@p", conn);

                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                        return Tuple.Create(rd.GetString("username"), rd.GetString("role"));
                }
            }
            return null;
        }

        // ---- ĐĂNG KÝ ----
        public static bool Register(string username, string password)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "INSERT INTO users(username,password,role) VALUES(@u,@p,'user')", conn);

                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        // ---- LẤY DANH SÁCH USER (ĐỂ SHOW Ở ROOMFORM) ----
        public static List<string> GetAllUsers()
        {
            var users = new List<string>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT username FROM users", conn);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        users.Add(rd.GetString("username"));
                    }
                }
            }

            return users;
        }

        // ---- LẤY USER TRỪ CHÍNH MÌNH ----
        public static List<string> GetAllUsersExcept(string username)
        {
            var users = new List<string>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT username FROM users WHERE username<>@me", conn);

                cmd.Parameters.AddWithValue("@me", username);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        users.Add(rd.GetString("username"));
                    }
                }
            }

            return users;
        }


        // ---- LƯU TIN NHẮN RIÊNG ----
        public static void SavePrivateMessage(string sender, string receiver, string payload)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "INSERT INTO messages(sender, receiver, text) VALUES(@s,@r,@t)", conn);

                cmd.Parameters.AddWithValue("@s", sender);
                cmd.Parameters.AddWithValue("@r", receiver);
                cmd.Parameters.AddWithValue("@t", payload);

                cmd.ExecuteNonQuery();
            }
        }


        // ---- LẤY LỊCH SỬ CHAT ----
        public static List<object> GetPrivateHistory(string user1, string user2)
        {
            var messages = new List<object>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"
                    SELECT sender, receiver, text, time 
                    FROM messages
                    WHERE (sender=@u1 AND receiver=@u2)
                       OR (sender=@u2 AND receiver=@u1)
                    ORDER BY time ASC
                ", conn);

                cmd.Parameters.AddWithValue("@u1", user1);
                cmd.Parameters.AddWithValue("@u2", user2);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var payload = rd.GetString("text");
                        messages.Add(new
                        {
                            sender = rd.GetString("sender"),
                            receiver = rd.GetString("receiver"),
                            payload = payload,
                            time = rd.GetDateTime("time").ToString("HH:mm:ss")
                        });
                    }
                }
            }

            return messages;
        }
    }
}
