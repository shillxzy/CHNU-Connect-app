using CHNU_Connect.DAL.Repositories;

namespace CHNU_Connect.DAL.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        IUserRepository UserRepository { get; }
        IPostRepository PostRepository { get; }
        IEventRepository EventRepository { get; }
        IGroupRepository GroupRepository { get; }
        IMessageRepository MessageRepository { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
