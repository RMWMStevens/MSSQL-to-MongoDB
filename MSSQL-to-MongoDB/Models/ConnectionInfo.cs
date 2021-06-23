using MSSQL_to_MongoDB.Models.Enums;
using System;

namespace MSSQL_to_MongoDB.Models
{
    [Serializable]
    public class ConnectionInfo
    {
        public string MongoDB { get; set; }
        public string MSSQL { get; set; }

        public string GetExampleFormat(DatabaseSystem system)
        {
            switch (system)
            {
                case DatabaseSystem.MSSQL:
                    return @"Data Source=COMPUTERNAME;Initial Catalog=DATABASENAME;User ID=USERNAME;Password=PASSWORD";
                case DatabaseSystem.MongoDB:
                    return @"mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb";
                default:
                    return string.Empty;
            }
        }
    }
}
