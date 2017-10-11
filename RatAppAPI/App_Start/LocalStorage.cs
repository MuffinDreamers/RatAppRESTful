using Microsoft.VisualBasic.FileIO;
using RatAppAPI.App_Start;
using RatAppAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace ContactList.App_Start
{
    public class LocalStorage
    {
        public LocalStorage()
        {

        }

        public void LoadSightingsFromFileIntoDB(string file)
        {
            DBStorage db = new DBStorage();
            using(TextFieldParser parser = new TextFieldParser(file))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;

                parser.ReadLine(); //Skip the first line with the column titles

                for(int i = 0; i < 61600; i++)
                {
                    parser.ReadLine();
                }

                Sighting[] sightings = new Sighting[200];
                int index = 0;
                bool overrideID = false;

                while(!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    int id; // field 0
                    DateTime dateCreated; // field 1
                    string locationType = fields[7]; // field 7
                    int zipcode; // field 8
                    string streetAddress = fields[9]; // field 9
                    string city = fields[16]; // field 16
                    string borough = fields[23]; // field 23
                    float latitude; // field 49
                    float longitude; // field 50



                    if(int.TryParse(fields[0], out id))
                    {
                        overrideID = true;
                    }
                    else
                    {
                        overrideID = false;
                    }
                    if(!DateTime.TryParse(fields[1], out dateCreated))
                    {
                        dateCreated = DateTime.Now;
                    }
                    if(!int.TryParse(fields[8], out zipcode))
                    {
                        zipcode = -1;
                    }
                    if(!float.TryParse(fields[49], out latitude)) {
                        latitude = 0.0f;
                    }
                    if(!float.TryParse(fields[50], out longitude))
                    {
                        longitude = 0.0f;
                    }

                    sightings[index++] = new Sighting(id, dateCreated, locationType, zipcode, streetAddress, city, borough, latitude, longitude);

                    if(!overrideID)// if there was a problem reading this ID and it needs to be auto generated
                    {
                        index--;
                        db.InsertSighting(sightings[index]);
                        continue;
                    }

                    if(index == sightings.Length)
                    {
                        db.InsertSightings(sightings, true);
                        sightings = new Sighting[sightings.Length];
                        index = 0;
                    }
                }
                db.InsertSightings(sightings, true);
            }
        }
    }
}