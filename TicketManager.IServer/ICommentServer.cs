using TicketManager.Model.Common;

namespace TicketManager.IServer
{
    public interface ICommentServer
    {
        Task<int> AddComment(UserComment userComm);
    }
}
