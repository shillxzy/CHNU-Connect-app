using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Repositories;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace CHNU_Connect.DAL.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction? _transaction;

        private IUserRepository? _userRepository;
        private IPostRepository? _postRepository;
        private IEventRepository? _eventRepository;
        private IGroupRepository? _groupRepository;
        private IAdminActionRepository? _adminActionRepository;
        private ICommentRepository? _commentRepository;
        private IEventParticipantRepository? _eventParticipantRepository;
        private IGroupMemberRepository? _groupMemberRepository;
        private IPostLikeRepository? _postLikeRepository;
        private IChatRepository? _chatRepository;
        private IChatMemberRepository? _chatMemberRepository;
        private IChatMessageRepository? _chatMessageRepository;


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        public IPostRepository PostRepository => _postRepository ??= new PostRepository(_context);
        public IEventRepository EventRepository => _eventRepository ??= new EventRepository(_context);
        public IGroupRepository GroupRepository => _groupRepository ??= new GroupRepository(_context);
        public IAdminActionRepository AdminActionRepository => _adminActionRepository ??= new AdminActionRepository(_context);
        public ICommentRepository CommentRepository => _commentRepository ??= new CommentRepository(_context);
        public IEventParticipantRepository EventParticipantRepository => _eventParticipantRepository ??= new EventParticipantRepository(_context);
        public IGroupMemberRepository GroupMemberRepository => _groupMemberRepository ??= new GroupMemberRepository(_context);
        public IPostLikeRepository PostLikeRepository => _postLikeRepository ??= new PostLikeRepository(_context);
        public IChatRepository ChatRepository => _chatRepository ??= new ChatRepository(_context);
        public IChatMemberRepository ChatMemberRepository => _chatMemberRepository ??= new ChatMemberRepository(_context);
        public IChatMessageRepository ChatMessageRepository => _chatMessageRepository ??= new ChatMessageRepository(_context);


        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new GenericRepository<T>(_context);
            }

            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
