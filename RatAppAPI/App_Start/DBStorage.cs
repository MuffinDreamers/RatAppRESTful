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
            SqlParameter[] sqlParams = 
            {
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@Password", user.Password),
                new SqlParameter("@Role", Enum.GetName(typeof(User.Roles), user.Role))
            };
            Status status = ExecuteCommand($"INSERT INTO Users VALUES(@Username, @Password, @Role);", sqlParams).Item1;
            if (status != Status.NO_ERROR)
                return null;
            return GetUserByUsername(user.Username);
        }

        public User GetUserById(int id)
        {
            Object[][] result = ExecuteCommand($"SELECT * FROM Users WHERE id = @ID;", 
                new SqlParameter[] { new SqlParameter("@ID", id) }).Item2;
            if (result.Length == 0)
                return null;
            return new User(result[0]);
        }

        public User GetUserByUsername(string username)
        {
            Object[][] result = ExecuteCommand($"SELECT * FROM Users WHERE username = @Username;",
                new SqlParameter[] { new SqlParameter("@Username", username) }).Item2;
            if (result.Length == 0)
                return null;
            return new User(result[0]);
        }

        public bool DeleteUser(int id)
        {
            Status status = ExecuteCommand($"DELETE FROM Users WHERE id = @ID;",
                new SqlParameter[] { new SqlParameter("@ID", id) }).Item1;
            return status == Status.NO_ERROR;
        }
#endregion

#region Sightings
        public Sighting GetSightingById(int id)
        {
            Object[][] result = ExecuteCommand($"SELECT * FROM Sightings WHERE ID = @ID;", 
                new SqlParameter[] { new SqlParameter("@ID", id) }).Item2;
            if (result.Length == 0)
                return null;
            return new Sighting(result[0]);
        }

        public Sighting[] GetAllSightings()
        {
            Object[][] objects = ExecuteCommand("SELECT TOP 100 * FROM Sightings ORDER BY DateCreated DESC;").Item2;
            Debug.WriteLine($"Selected {objects.Length} sightings from the database");
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

        public Sighting InsertSighting(Sighting sighting, bool overrideID = false)
        {
            return InsertSightings(new[] { sighting }, overrideID).ElementAt(0);
        }

        public IEnumerable<Sighting> InsertSightings(IEnumerable<Sighting> sightings, bool overrideID = false)
        {
            sightings = sightings.Where(i => i != null);
            if (sightings.Count() < 1)
                return null;

            string command = "";
            if (overrideID)
                command += "SET IDENTITY_INSERT Sightings ON;";
            command += "INSERT INTO SIGHTINGS (";
            if (overrideID)
                command += "ID, ";
            command += "DateCreated, LocationType, Zipcode, StreetAddress, City, Borough, Latitude, " +
                       "Longitude) VALUES";

            List<SqlParameter> sqlParams = new List<SqlParameter>();

            for (int i = 0; i < sightings.Count(); i++)
            {
                command += "(";
                if (overrideID)
                {
                    command += $"@ID{i}, ";
                    sqlParams.Add(new SqlParameter($"@ID{i}", sightings.ElementAt(i).ID));
                }
                command += $"@DateCreated{i}, @LocationType{i}, @Zipcode{i}, " +
                           $"@StreetAddress{i}, @City{i}, @Borough{i}, " +
                           $"@Latitude{i}, @Longitude{i}),";

                sqlParams.Add(new SqlParameter($"@DateCreated{i}", sightings.ElementAt(i).DateCreated));
                sqlParams.Add(new SqlParameter($"@LocationType{i}", sightings.ElementAt(i).LocationType));
                sqlParams.Add(new SqlParameter($"@Zipcode{i}", sightings.ElementAt(i).Zipcode));
                sqlParams.Add(new SqlParameter($"@StreetAddress{i}", sightings.ElementAt(i).StreetAddress));
                sqlParams.Add(new SqlParameter($"@City{i}", sightings.ElementAt(i).City));
                sqlParams.Add(new SqlParameter($"@Borough{i}", sightings.ElementAt(i).Borough));
                sqlParams.Add(new SqlParameter($"@Latitude{i}", sightings.ElementAt(i).Latitude));
                sqlParams.Add(new SqlParameter($"@Longitude{i}", sightings.ElementAt(i).Longitude));
            }

            //Remove extra comma at end
            command = command.TrimEnd(',');

            command += "; SELECT SCOPE_IDENTITY();";
            if (overrideID)
                command += "SET IDENTITY_INSERT Sightings OFF;";


            Tuple<Status, Object[][]> result = ExecuteCommand(command, sqlParams);

            Sighting[] newSightings = new Sighting[sightings.Count()];

            int lastID = Convert.ToInt32(result.Item2[0][0]);
            int firstID = lastID - sightings.Count() + 1;

            for (int i = 0; i < sightings.Count(); i++)
            {
                newSightings[i] = new Sighting(sightings.ElementAt(i));
                if(!overrideID)
                {
                    newSightings[i].ID = firstID + i;
                }
            }

            return newSightings;
        }

        public bool DeleteSighting(int id)
        {
            Status status = ExecuteCommand($"DELETE FROM Sightings WHERE id = @ID;",
                new SqlParameter[] { new SqlParameter("@ID", id) }).Item1;
            return status == Status.NO_ERROR;
        }
#endregion

        private Tuple<Status, Object[][]> ExecuteCommand(string sqlCommand, IEnumerable<SqlParameter> sqlParams = null)
        {
            //Debug.WriteLine("Preparing to execute SQL command: " + sqlCommand);
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
                        if (sqlParams != null)
                            command.Parameters.AddRange(sqlParams.ToArray());

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Object[] values = new Object[reader.FieldCount];
                                reader.GetValues(values);
                                /*foreach(Object o in values) {
                                    Debug.Write(o.ToString() + ",");
                                }
                                Debug.WriteLine("");*/

                                data.Add(values);
                            }
                        }
                    }
                    return Tuple.Create(Status.NO_ERROR, data.ToArray());
                }
                catch(SqlException e)
                {
                    Debug.WriteLine("Sql Error: " + e.Number + ", " + e.Message);
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