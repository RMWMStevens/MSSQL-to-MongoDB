using System;
using System.Data.SqlClient;

namespace MSSQL_to_MongoDB.Services
{
    public class MsSqlService : DatabaseService
    {
        public MsSqlService()
        {
            system = Models.Enums.DatabaseSystem.MSSQL;
        }

        public override string GetExampleFormat()
        {
            var sqlAuth = @"Data Source=COMPUTERNAME;Initial Catalog=DATABASENAME;User ID=USERNAME;Password=PASSWORD";
            var winAuth = @"Data Source=COMPUTERNAME;Initial Catalog=DATABASENAME;Integrated Security=SSPI;";

            return $"For SQL Authentication: \n{sqlAuth}\nFor Windows Authentication: \n{winAuth}";
        }

        public void RunQuery()
        {
            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            var query = "SELECT TOP 10 * FROM MOVIES";
            var command = new SqlCommand(query, sqlConnection);
            var dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                var row = string.Empty;

                for (var i = 0; i < dataReader.FieldCount; i++)
                {
                    if (i != 0) { row += " - "; }
                    row += dataReader.GetValue(i);
                }
                Console.WriteLine(row);
            }

            sqlConnection.Close();
        }
    }
}
