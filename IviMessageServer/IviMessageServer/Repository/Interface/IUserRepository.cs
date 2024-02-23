using IviMessageServer.DataModels;

namespace IviMessageServer.Repository.Interface
{
    public interface IUserRepository:IRepository
    {
        int AddUser(User user);
        void RemoveUser(int Id);
        Chat SelectRandomUser(int Id);
        string DeleteChat(int ChatId);
        int[] DeleteChats(int ChatId);
    }
}
