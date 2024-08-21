using TicketManager.Model.User;
using TicketManager.QueryModel.User;

namespace TicketManager.IServer
{
    public interface IUserServer
    {
        Task<UserInfo> userLogin(UserQuery query);

        Task<int> RegisterUser(UserInfo info);

        Task<bool> UpdateUserInfo(UserInfo info);

        Task<bool> DelUserInfo(UserInfo info);
    }
}
