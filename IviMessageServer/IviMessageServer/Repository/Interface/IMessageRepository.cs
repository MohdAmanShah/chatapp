namespace IviMessageServer.Repository.Interface
{
    public interface IMessageRepository:IRepository
    {
        void AddMessage();
        void RemoveMessage();
    }
}
