using MSSQL_to_MongoDB.Services;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MSSQL_to_MongoDB.Helpers
{
    public static class SqlHelper
    {
        public static List<string> RunQuery(string sqlQuery)
        {
            var sqlConnection = new SqlConnection(SqlService.ConnectionString);
            sqlConnection.Open();

            var command = new SqlCommand(sqlQuery, sqlConnection);
            var dataReader = command.ExecuteReader();

            var rows = new List<string>();

            while (dataReader.Read())
            {
                var row = string.Empty;

                for (var i = 0; i < dataReader.FieldCount; i++)
                {
                    if (i != 0) { row += "|"; }
                    row += dataReader.GetValue(i);
                }
                rows.Add(row);
            }

            sqlConnection.Close();
            return rows;
        }
    }
}
