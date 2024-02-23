using IviMessageServer.DataModels;
using IviMessageServer.Repository;
using IviMessageServer.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace IviMessageServer.Repository
{
    public class UserRepository : Repository, IUserRepository
    {
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
            }
            finally
            {
                CloseConnection();
            }
        }

        public Chat SelectRandomUser(int Id)
        {

            Chat chat = new Chat();
            User user = new User();
            user.Id = 0;
            user.Name = "Null";
            try
            {
                string query = @"
                WITH G1 AS (
                  SELECT RecipentID
                  FROM Chats
                  WHERE CreatorID = @Id
                ),
                G2 AS (
                  SELECT CreatorID
                  FROM Chats
                  WHERE RecipentID = @Id
                ),
                FG AS (
                  SELECT RecipentID AS UserId
                  FROM G1
                  UNION
                  SELECT CreatorID AS UserId
                  FROM G2
                ),
                RandomUser AS (
                  SELECT TOP 1 u.ID, u.Name
                  FROM users u
                  WHERE u.ID != @Id
                    AND u.ID NOT IN (SELECT UserId FROM FG)
                  ORDER BY NEWID()
                )
                SELECT ID, Name FROM RandomUser;";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", Id);
                OpenConnection();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int selectedUserId = reader.GetInt32(0);
                        string selectedUserName = reader.GetString(1);
                        Console.WriteLine($"Selected User ID: {selectedUserId}, Name: {selectedUserName}");
                        user.Id = selectedUserId;
                        user.Name = selectedUserName;
                        chat.user = user;
                    }
                }
                if (user.Id != 0)
                {
                    chat.ChatId = CreateChat(Id, user.Id);
                }
            }
            catch
            {

            }
            finally
            {
                CloseConnection();
            }
            return chat;
        }

        private int CreateChat(int CreateorId, int RecipentID)
        {
            int ChatId = 0;
            try
            {
                string query = @"INSERT INTO Chats (CreatorID, RecipentID) OUTPUT INSERTED.ChatId  VALUES (@CId, @RId);";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CId", CreateorId);
                command.Parameters.AddWithValue("@Rid", RecipentID);
                ChatId = Convert.ToInt32(command.ExecuteScalar());
            }
            catch
            {

            }
            return ChatId;
        }

        public string DeleteChat(int ChatId)
        {
            string result = string.Empty;
            try
            {
                OpenConnection();
                string query = @"DELETE FROM Chats WHERE ChatId = @ChatId;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ChatId", ChatId);
                command.ExecuteNonQuery();
                result = $"{ChatId}:chat deleted";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        public int[] DeleteChats(int Id)
        {
            List<int> ids = new List<int>();
            try
            {
                string query = @"
        WITH G1 AS (
            SELECT RecipentID
            FROM Chats
            WHERE CreatorID = @Id
        ),
        G2 AS (
            SELECT CreatorID
            FROM Chats
            WHERE RecipentID = @Id
        ),
        FG AS (
            SELECT RecipentID AS UserId
            FROM G1
            UNION
            SELECT CreatorID AS UserId
            FROM G2
        )
        SELECT UserID FROM FG;";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", Id);
                OpenConnection();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["UserID"]);
                        ids.Add(id);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                CloseConnection();
            }
            return ids.ToArray();
        }
    }
}
