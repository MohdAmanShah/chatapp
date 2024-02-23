namespace IviMessageServer.Repository.Interface
{
    public interface IUnitOfWork
    {
        IMessageRepository MessageRepository { get; }
        IUserRepository UserRepository { get; }
    }
}
