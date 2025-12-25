using CHNU_Connect.DAL.Repositories.Interfaces;

namespace CHNU_Connect.DAL.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        IUserRepository UserRepository { get; }
        IPostRepository PostRepository { get; }
        IEventRepository EventRepository { get; }
        IGroupRepository GroupRepository { get; }
        IAdminActionRepository AdminActionRepository { get; }
        ICommentRepository CommentRepository { get; }
        IEventParticipantRepository EventParticipantRepository { get; }
        IGroupMemberRepository GroupMemberRepository { get; }
        IPostLikeRepository PostLikeRepository { get; }
        IChatRepository ChatRepository { get; }
        IChatMemberRepository ChatMemberRepository { get; }
        IChatMessageRepository ChatMessageRepository { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
