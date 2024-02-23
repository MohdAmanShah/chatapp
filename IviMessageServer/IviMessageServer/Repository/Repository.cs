using Microsoft.Data.SqlClient;
using System.Data;
using IviMessageServer.Repository.Interface;
using System.Runtime.ConstrainedExecution;
namespace IviMessageServer.Repository
{
    public class Repository:IRepository
    {
        private readonly string connectionString = "Server=NOA;Database=ivimessage;Trusted_Connection=True;TrustServerCertificate=Yes";
        protected readonly SqlConnection connection;

        public Repository()
        {
            connection = new SqlConnection(connectionString);
        }

        public void OpenConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while opening connection: " + ex.Message);
            }
        }

        public void CloseConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while closing connection: " + ex.Message);
            }
        }
    }
}
