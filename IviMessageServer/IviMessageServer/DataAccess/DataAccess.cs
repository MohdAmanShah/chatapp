using Microsoft.Data.SqlClient;
using IviMessageServer.DataModels;
using System.Data;
namespace IviMessageServer.Data_Access
{
    public class DataAccess
    {
        private readonly string connectionString = "Server=NOA;Database=ivimessage;Trusted_Connection=True;TrustServerCertificate=Yes";
        private readonly SqlConnection connection;

        public DataAccess()
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

        public int AddUser(User user)
        {
            int generatedId = 0;
            try
            {
                string query = "INSERT INTO users (Name) VALUES (@Name); SELECT SCOPE_IDENTITY();";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", user.Name);
                OpenConnection();
                generatedId = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing query: " + ex.Message);
                return generatedId;
            }
            finally
            {
                CloseConnection();
            }
            return generatedId;
        }

        public void RemoveUser(int id)
        {
            try
            {
                string query = @"
IF EXISTS (SELECT 1 FROM users WHERE Id = @Id)
BEGIN
    DELETE FROM users WHERE Id = @Id
END";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                OpenConnection();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing query: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

    }
}
