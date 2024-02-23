using IviMessageServer.Repository.Interface;

namespace IviMessageServer.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IMessageRepository MessageRepository { get; set; }
        public IUserRepository UserRepository { get; set; }

        public UnitOfWork()
        {
            this.MessageRepository = new MessageRepository();
            this.UserRepository = new UserRepository();
        }
    }
}
