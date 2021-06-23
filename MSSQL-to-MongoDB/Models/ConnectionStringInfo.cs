using System;

namespace MSSQL_to_MongoDB.Models
{
    [Serializable]
    public class ConnectionStringInfo
    {
        // Example: mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb
        public string MongoDB { get; set; }
        // Example: @"Data Source=COMPUTERNAME;Initial Catalog=DATABASENAME;User ID=USERNAME;Password=PASSWORD"
        public string MSSQL { get; set; }
    }
}
