using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using NickBuhro.Translit;

namespace first
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SqlConnection connection =
                 //new SqlConnection(@"Data Source=localhost,1433\sql1;Initial Catalog=demo2021;User ID=sa;Password=Passw0rd%"))
                 new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=demo2021;Integrated Security=True"))
            {
                connection.Open();
                List<int> RolesList = GetRolesIDs(connection);
                if (RolesList.Count <= 0)
                {
                    ClearTable(connection);
                    InsertDefaultRoles(connection);
                    RolesList = GetRolesIDs(connection);
                    foreach (int role in RolesList)
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            InsertDefaultUsers(connection, role);
                        }
                    }
                }
                Console.Write("Select id: ");
                try
                {
                    int role_id = int.Parse(Console.ReadLine());
                    if (!RolesList.Contains(role_id)) throw new Exception("Unknown role id");

                    Console.Write($"Role {GetRoleNameById(connection, role_id)}");

                    List<string> users = GetAllUsersByRoleId(connection, role_id);

                    DisplayList(users);
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }
        }
        public static List<int> GetRolesIDs(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.Roles";
            List<int> RolesList = new List<int>();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string title = reader.GetString(1);
                    Console.WriteLine($"{id}\t{title}");
                    RolesList.Add(id);
                }
            }
            return RolesList;
        }
        public static string GetRoleNameById(SqlConnection connection, int id)
        {
            SqlCommand roleNameCmd = connection.CreateCommand();
            roleNameCmd.CommandText = "SELECT Roles.Title FROM Roles WHERE Roles.Id = @RoleId";
            SqlParameter parameterA = new SqlParameter("@RoleId", id);
            roleNameCmd.Parameters.Add(parameterA);
            string result = "";
            using (SqlDataReader reader = roleNameCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result = reader.GetString(0);
                }
            }
            return result;
        }
        public static List<string> GetAllUsersByRoleId(SqlConnection connection, int id)
        {
            SqlCommand usersCmd = connection.CreateCommand();
            usersCmd.CommandText = "SELECT Users.Login FROM dbo.Users WHERE Users.RoleId = @RoleID";
            SqlParameter parameterB = new SqlParameter("@RoleId", id);
            usersCmd.Parameters.Add(parameterB);
            List<string> users = new List<string>();
            using (SqlDataReader reader = usersCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(reader.GetString(0));
                }
            }
            return users;
        }
        public static void DisplayList<T>(List<T> list)
        {
            Console.Write("(");
            for (int i = 0; i < list.Count; ++i)
            {
                Console.Write($"{list[i]}");
                if (i < list.Count - 1)
                {
                    Console.Write(",");
                }
            }
            Console.Write(")");
        }
        public static void ClearTable(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Users; DELETE FROM Roles;";
            cmd.ExecuteNonQuery();
        }
        public static void InsertDefaultRoles(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO ROLES (Title) VALUES (N'Administrative'),(N'Business'), (N'Customer service')";
            cmd.ExecuteNonQuery();
        }
        public static void InsertDefaultUsers(SqlConnection connection, int role_id)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO Users (RoleId,[Login],[Password]) VALUES ({role_id}, '{RandomString(10)}', '{RandomString(10)}')";
            cmd.ExecuteNonQuery();
        }
        public static string RandomString(int len)
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnoprstuvmxxyz";
            return new string(Enumerable.Repeat(chars, len)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}