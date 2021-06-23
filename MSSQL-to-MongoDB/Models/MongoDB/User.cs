using System;
using System.Collections.Generic;

namespace MSSQL_to_MongoDB.Models.MongoDB
{
    public class User
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string CountryCode { get; set; }
        public string Sex { get; set; }
        public Country Country { get; set; }
        public List<Platform> Platforms { get; set; }
        public List<string> MediaTypes { get; set; }
        public List<Movie> Favorites { get; set; }
    }
}
