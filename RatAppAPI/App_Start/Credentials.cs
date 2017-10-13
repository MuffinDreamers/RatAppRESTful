using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactList.App_Start
{
    public struct Credentials
    {
        public static string DataSource { get; } = "";
        public static string UserId { get; } = "";
        public static string Password { get; } = "";
        public static string InitialCatalog { get; } = "";
    }
}