using FoodRecipes.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace FoodRecipes.Services
{
    public class RecipeService
    {

        private static String connectionString = "server=localhost;uid=root;pwd=password;database=food_recipes;";

        private MySqlConnection connection = new MySqlConnection(connectionString);

        public RecipeService()
        {
          
        }
        public List<RecipeModel> getAll()
        {
            MySqlCommand command = new MySqlCommand();
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = "select c.id, c.name, c.description, c.type, c.steps, u.user_name from recipe c inner join user u on u.id=c.chef_id";
            MySqlDataReader reader = command.ExecuteReader();
            List<RecipeModel> ls = new List<RecipeModel>();

            while (reader.Read())
            {
                RecipeModel r = new RecipeModel();
                r.id = reader.GetInt32("id");
                r.chef_name = reader.GetString("user_name");
                r.description = reader.GetString("description");
                r.name = reader.GetString("name");
                r.steps = reader.GetString("steps");
                r.type = reader.GetString("type");
                ls.Add(r);
            }

            reader.Close();
            connection.Close();
            return ls;

        }

        public List<RecipeModel> getAllOfType(String givenType)
        {
            MySqlCommand command = new MySqlCommand();
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = "select c.id, c.name, c.description, c.type, c.steps, u.user_name from recipe c inner join user u on u.id=c.chef_id where c.type='" + givenType + "'";

            MySqlDataReader reader = command.ExecuteReader();
            List<RecipeModel> ls = new List<RecipeModel>();

            while (reader.Read())
            {
                RecipeModel r = new RecipeModel();
                r.id = reader.GetInt32("id");
                r.chef_name = reader.GetString("user_name");
                r.description = reader.GetString("description");
                r.name = reader.GetString("name");
                r.steps = reader.GetString("steps");
                r.type = reader.GetString("type");
                ls.Add(r);
            }

            reader.Close();
            connection.Close();
            return ls;

        }

        public bool RemoveRecipe(int rec_id)
        {
            MySqlCommand command = new MySqlCommand();
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = "delete from recipe where id=@rec_id";
            command.Parameters.AddWithValue("@rec_id", rec_id);
            Int32 res = command.ExecuteNonQuery();
            connection.Close();
            return res != 0;

        }

        public bool AddRecipe(string name, string description, string steps, string recType,Int32 chef_id)
        {
            MySqlCommand command = new MySqlCommand();
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = "insert into recipe(chef_id,name,description,type,steps) values(@chid, @na,@desc,@ty,@st)";

            command.Parameters.AddWithValue("@chid",chef_id);
            command.Parameters.AddWithValue("@na",name);
            command.Parameters.AddWithValue("@desc", description);
            command.Parameters.AddWithValue("@ty", recType);
            command.Parameters.AddWithValue("@st", steps);
            Int32 res = command.ExecuteNonQuery();
            connection.Close();
            return res != 0;
        }

        public RecipeModelDto getRecipe(int rec_id)
        {
            MySqlCommand command = new MySqlCommand();
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = "select c.id, c.name, c.description, c.type, c.steps, u.user_name from recipe c inner join user u on u.id=c.chef_id where c.id="+rec_id;

            MySqlDataReader reader = command.ExecuteReader();
            RecipeModelDto r = new RecipeModelDto();
            if (reader.Read())
            {
                r.id = reader.GetInt32("id");
                r.chef_name = reader.GetString("user_name");
                r.description = reader.GetString("description");
                r.name = reader.GetString("name");
                r.steps = reader.GetString("steps");
                r.typeRec = reader.GetString("type");

            }
            reader.Close();
            connection.Close();
            return r;

        }

        public bool UpdateRecipe(Int32 id, Int32 ch_id, string description, string steps, string recType)
        {
            MySqlCommand command = new MySqlCommand();
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = "update recipe set chef_id=@ch, description=@desc, type=@rt,  steps=@st where id=@i";
            command.Parameters.AddWithValue("@ch", ch_id);
            command.Parameters.AddWithValue("@desc", description);
            command.Parameters.AddWithValue("@rt", recType);
            command.Parameters.AddWithValue("@st", steps);
            command.Parameters.AddWithValue("@i", id);
            Int32 res = command.ExecuteNonQuery();
            return res != 0;

        }
    }
}
