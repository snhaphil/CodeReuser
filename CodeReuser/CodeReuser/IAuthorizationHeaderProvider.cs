using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CodeReuser
{
    public interface IAuthorizationHeaderProvider
    {
        Task<AuthenticationHeaderValue> GetAsync();
    }
}
