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
            return @"Data Source=COMPUTERNAME;Initial Catalog=DATABASENAME;User ID=USERNAME;Password=PASSWORD";
        }
    }
}
