namespace MSSQL_to_MongoDB.Services
{
    public class MongoDbService : DatabaseService
    {
        public MongoDbService()
        {
            system = Models.Enums.DatabaseSystem.MongoDB;
        }

        public override string GetExampleFormat()
        {
            return @"mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb";
        }
    }
}
