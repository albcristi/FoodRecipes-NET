using FoodRecipes.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecipes.Services
{
    public class UserService
    {
        private static String connectionString = "server=localhost;uid=root;pwd=password;database=food_recipes;";

        private MySqlConnection connection = new MySqlConnection(connectionString);

        private IConfiguration _config;

        public UserService(IConfiguration configuration)
        {
            this._config = configuration;
        }

        public string generateJWebToken(UserModel userModel)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Boolean CheckUserCredentials(String user_name, String password)
        {
            UserModel u = new UserModel();
            u.id = 1;
            u.user_name = user_name;
            u.password = password;
            var s = generateJWebToken(u);

            MySqlCommand command = new MySqlCommand();
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = "select count(*) from user where user_name='" + user_name + "' and password='" + password + "'";
            Int64 val = (Int64) command.ExecuteScalar();
            connection.Close();
            return val==1;
        }

        public Int32 getUserId(string user_name)
        {
            MySqlCommand command = new MySqlCommand();
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = "select * from user where user_name='"+user_name+"'";
            MySqlDataReader reader = command.ExecuteReader();
            Int32 id = 0;
            if (reader.Read())
                id = reader.GetInt32("id");
            reader.Close();
            connection.Close();
            return id;
        }

        public Boolean AddSession(Int32 user_id, string sesssionToken, DateTime ttl)
        {

            MySqlCommand command = new MySqlCommand();
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = "insert into session(user_id,token,ttl) values(" + user_id + ", '" + sesssionToken + "', @ttl)";
            command.Parameters.AddWithValue("@ttl", ttl);
            Int32 res = command.ExecuteNonQuery();
            connection.Close();
            return res == 1;

        }

        public Boolean UpdateSessionTtl(Int32 user_id, DateTime dateTime)
        {
            MySqlCommand command = new MySqlCommand();
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = "update session set ttl=@ttl where user_id=@usr";
            command.Parameters.AddWithValue("@ttl", dateTime);
            command.Parameters.AddWithValue("@usr", user_id);
            Int32 res = command.ExecuteNonQuery();
            connection.Close();
            return res == 1;
        }

        public Boolean RemoveSession(Int32 user_id)
        {
            MySqlCommand command = new MySqlCommand();
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = "delete from session where user_id=@usr";
            command.Parameters.AddWithValue("@usr", user_id);
            Int32 res = command.ExecuteNonQuery();
            connection.Close();
            return res == 1;
        }

        public UserModel GetUserBySessionToken(string token)
        {
            MySqlCommand command = new MySqlCommand();
            if(connection.State != ConnectionState.Open)
                     connection.Open();
            command.Connection = connection;
            command.CommandText = "select c.id, c.user_name, c.password from session s inner join user c on c.id=s.user_id where s.token=@tok";
            command.Parameters.AddWithValue("@tok", token);
            UserModel u = new UserModel();
            u.id = 0;
            u.user_name = "not val";
            u.password = "";
            MySqlDataReader res = command.ExecuteReader();
            if (res.Read())
            {
                    u.id = res.GetInt32("id");
                    u.user_name = res.GetString("user_name");
                    u.password = res.GetString("password");
                
            }
            res.Close();
            connection.Close();
            
            return u;
            
        }

        public Boolean SessionIsValid(string token, DateTime dateTime)
        {
            MySqlCommand command = new MySqlCommand();
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = "select * from session where token=@tok";
            command.Parameters.AddWithValue("@tok", token);
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                DateTime date = reader.GetDateTime("ttl");
                reader.Close();
                connection.Close();
                return DateTime.Compare(date, dateTime) > 0;
            }

            reader.Close();
            connection.Close();
            return false;
        }
    }


}
