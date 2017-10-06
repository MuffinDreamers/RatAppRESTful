using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using RatAppAPI.Models;
using System.Diagnostics;

namespace RatAppAPI.App_Start
{
    public class DBStorage
    {
        public enum Status
        {
            DUPLICATE_USER = 2627,
            UNKNOWN_ERROR = 9999,
            NO_ERROR = 0
        }
        const string dataSource = "muffindreamers.database.windows.net";
        const string userID = "muffin";
        const string password = "dreamers123!";
        const string initialCatalog = "RatAppDB";

        string connectionString;

        public DBStorage()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = dataSource;
            builder.UserID = userID;
            builder.Password = password;
            builder.InitialCatalog = initialCatalog;
            connectionString = builder.ConnectionString;
        }

#region Users
        public User InsertUser(User user)
        {
            Status status = ExecuteCommand($"INSERT INTO Users VALUES('{user.Username}', '{user.Password}', '{Enum.GetName(typeof(User.Roles), user.Role)}');").Item1;
            if (status != Status.NO_ERROR)
                return null;
            return GetUserByUsername(user.Username);
        }

        public User GetUserById(int id)
        {
            Object[][] result = ExecuteCommand($"SELECT * FROM Users WHERE id = {id};").Item2;
            if (result.Length == 0)
                return null;
            return new User(result[0]);
        }

        public User GetUserByUsername(string username)
        {
            Object[][] result = ExecuteCommand($"SELECT * FROM Users WHERE username = '{username}';").Item2;
            if (result.Length == 0)
                return null;
            return new User(result[0]);
        }

        public bool DeleteUser(int id)
        {
            Status status = ExecuteCommand($"DELETE FROM Users WHERE id = {id};").Item1;
            return status == Status.NO_ERROR;
        }
#endregion

#region Sightings
        public Sighting GetSightingById(int id)
        {
            Object[][] result = ExecuteCommand($"SELECT * FROM Sightings WHERE ID = {id};").Item2;
            if (result.Length == 0)
                return null;
            return new Sighting(result[0]);
        }

        public Sighting[] GetAllSightings()
        {
            Object[][] objects = ExecuteCommand("SELECT * FROM Sightings;").Item2;
            Debug.WriteLine($"Found {objects.Length} sightings in the database");
            Sighting[] sightings = new Sighting[objects.Length];
            for(int i = 0; i < objects.Length; i++)
            {
                try
                {
                    sightings[i] = new Sighting(objects[i]);
                } catch(InvalidOperationException)
                {
                    return null;
                }

            }
            return sightings;
        }

        public Sighting InsertSighting(Sighting sighting)
        {
            Tuple<Status, Object[][]> result = ExecuteCommand($"INSERT INTO Sightings VALUES('{sighting.DateCreated}', " +
                $"'{sighting.LocationType}', {sighting.Zipcode}, '{sighting.StreetAddress}', " +
                $"'{sighting.City}', '{sighting.Borough}', {sighting.Latitude}, {sighting.Longitude}); SELECT SCOPE_IDENTITY();");
            if (result.Item1 != Status.NO_ERROR)
                return null;
            Sighting newSighting = new Sighting(sighting);
            newSighting.ID = Convert.ToInt32(result.Item2[0][0]);
            return newSighting;
        }

        public bool DeleteSighting(int id)
        {
            Status status = ExecuteCommand($"DELETE FROM Sightings WHERE id = {id};").Item1;
            return status == Status.NO_ERROR;
        }
#endregion

        private Tuple<Status, Object[][]> ExecuteCommand(string sqlCommand)
        {
            Debug.WriteLine("Preparing to execute SQL command: " + sqlCommand);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                List<Object[]> data = new List<Object[]>();//Data returned from the SQL Database
                //SqlCommand command = new SqlCommand(sqlCommand, connection);
                try
                {
                    //int rowsAffected = command.ExecuteNonQuery();
                    using (SqlCommand command = new SqlCommand(sqlCommand, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Object[] values = new Object[reader.FieldCount];
                                reader.GetValues(values);
                                foreach(Object o in values) {
                                    Debug.Write(o.ToString() + ",");
                                }
                                Debug.WriteLine("");

                                data.Add(values);
                            }
                        }
                    }
                    return Tuple.Create(Status.NO_ERROR, data.ToArray());
                }
                catch(SqlException e)
                {
                    Debug.WriteLine("Sql Error: " + e.Number);
                    if (!Enum.IsDefined(typeof(Status), e.Number))//If we don't have an error code for this
                        return Tuple.Create(Status.UNKNOWN_ERROR, data.ToArray());
                    return Tuple.Create((Status)e.Number, data.ToArray());
                }
            }
        }

        private void DisplaySqlErrors(SqlException exception)
        {
            for (int i = 0; i < exception.Errors.Count; i++)
            {
                Debug.WriteLine("Index #" + i + "\n" +
                    "Error " + exception.Errors[i].Number + ": " + exception.Errors[i].ToString() + "\n");
            }
            Debug.WriteLine("");
        }
    }
}