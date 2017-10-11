using ContactList.App_Start;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace RatAppAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //ReadCSV();
        }

        private void ReadCSV()
        {
            LocalStorage ls = new LocalStorage();
            Debug.WriteLine("Loading CSV into Database...");
            Stopwatch stopwatch = Stopwatch.StartNew();
            ls.LoadSightingsFromFileIntoDB(@"C:\Development\Visual Studio 2017\Projects\RatAppRESTful\RatAppAPI\Rat_Sightings.csv");
            stopwatch.Stop();
            Debug.WriteLine($"Done loading CSV into Database after {stopwatch.Elapsed.TotalSeconds} seconds");
        }
    }
}
