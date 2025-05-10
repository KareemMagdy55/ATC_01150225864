namespace Nadwa.Data.Repositories.Interface;

public interface IUnitOfWork : IDisposable, IAsyncDisposable {
    Task<int> CompleteAsync();
    public IEventRepository EventRepository { get; }
    public IApplicationUserRepository ApplicationUserRepository { get; }
}