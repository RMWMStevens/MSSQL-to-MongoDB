namespace MSSQL_to_MongoDB.Models.MSSQL
{
    public class User
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string BirthDate { get; set; }
        public string CountryCode { get; set; }
        public string Sex { get; set; }
    }
}
