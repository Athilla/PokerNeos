using System.Threading.Tasks;

namespace Transversals.Business.Authentication.Application.Services
{
    public interface IEmailService
    {
        bool IsValidEmailAddress(string address);
        Task<bool> SendAsync(string subject, string content, params string[] tos);
    }
}