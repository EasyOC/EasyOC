using EasyOC.OrchardCore.OpenApi.Dto;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Services.UserInfo
{
    public interface IUserInfoAppService
    {
        Task<UserDetailsDto> GetUserInfoAsync();
    }
}