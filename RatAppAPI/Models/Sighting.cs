using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RatAppAPI.Models
{
    public class Sighting
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }
        [JsonProperty("locationType")]
        public string LocationType { get; set; } //TODO: Change this to an enum
        [JsonProperty("zipcode")]
        public int Zipcode { get; set; }
        [JsonProperty("streetAddress")]
        public string StreetAddress { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("borough")]
        public string Borough { get; set; } //TODO: Change this to an enum
        [JsonProperty("latitude")]
        public float Latitude { get; set; }
        [JsonProperty("longitude")]
        public float Longitude { get; set; }

        [JsonConstructor]
        public Sighting(int id, DateTime dateCreated, string locationType, int zipcode,
            string streetAddress, string city, string borough, float latitude, float longitude)
        {
            ID = id;
            DateCreated = dateCreated;
            LocationType = locationType;
            Zipcode = zipcode;
            StreetAddress = streetAddress;
            City = city;
            Borough = borough;
            Latitude = latitude;
            Longitude = longitude;
        }

        public Sighting(Object[] values)
        {
            if(values.Length != 9)
                throw new InvalidOperationException("Sighting can only be constructed by an array with 9 values");
            if (values[0].GetType() != typeof(int))
                throw new InvalidOperationException("First value of array must be the ID int");
            if (values[1].GetType() != typeof(DateTime))
                throw new InvalidOperationException("Second value of array must be the DateCreated DateTime");
            if (values[2].GetType() != typeof(string))
                throw new InvalidOperationException("Third value of array must be the LocationType string");
            if (values[3].GetType() != typeof(int))
                throw new InvalidOperationException("Fourth value of array must be the Zipcode int");
            if (values[4].GetType() != typeof(string))
                throw new InvalidOperationException("Fifth value of array must be the StreetAddress string");
            if (values[5].GetType() != typeof(string))
                throw new InvalidOperationException("Sixth value of array must be the City string");
            if (values[6].GetType() != typeof(string))
                throw new InvalidOperationException("Seventh value of array must be the Borough string");
            if (values[7].GetType() != typeof(double))
                throw new InvalidOperationException("Eigth value of array must be the Latitude float");
            if (values[8].GetType() != typeof(double))
                throw new InvalidOperationException("Nineth value of array must be the Longitude float");

            ID = Convert.ToInt32(values[0]);
            DateCreated = Convert.ToDateTime(values[1]);
            LocationType = Convert.ToString(values[2]);
            Zipcode = Convert.ToInt32(values[3]);
            StreetAddress = Convert.ToString(values[4]);
            City = Convert.ToString(values[5]);
            Borough = Convert.ToString(values[6]);
            Latitude = (float)Convert.ToDouble(values[7]);
            Longitude = (float)Convert.ToDouble(values[8]);
        }

        public Sighting(Sighting old)
        {
            ID = old.ID;
            DateCreated = old.DateCreated;
            LocationType = string.Copy(old.LocationType);
            Zipcode = old.Zipcode;
            StreetAddress = string.Copy(old.StreetAddress);
            City = string.Copy(old.City);
            Borough = string.Copy(old.Borough);
            Latitude = old.Latitude;
            Longitude = old.Longitude;
        }
    }
}