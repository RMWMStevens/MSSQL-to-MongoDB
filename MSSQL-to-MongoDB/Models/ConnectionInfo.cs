using MSSQL_to_MongoDB.Models.Enums;
using System;

namespace MSSQL_to_MongoDB.Models
{
    [Serializable]
    public class ConnectionInfo
    {
        public string ConnectionString { get; set; }

        public string GetExampleFormat(DatabaseSystem system)
        {
            switch (system)
            {
                case DatabaseSystem.MSSQL:

                case DatabaseSystem.MongoDB:

                default:
                    return string.Empty;
            }
        }
    }
}
