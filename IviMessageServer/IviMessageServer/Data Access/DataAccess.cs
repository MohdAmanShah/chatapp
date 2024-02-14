using Microsoft.Data.SqlClient;
namespace IviMessageServer.Data_Access
{
    public class DataAccess
    {
        private readonly SqlConnection connection;
        private readonly string connectionString = "Server=NOA;Database=ivimessage;Trusted_Connection=True;TrustServerCertificate=Yes";
        public DataAccess()
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        public int Adduser(int Id, string UserName)
        {

            return 1;
        }
    }
}
