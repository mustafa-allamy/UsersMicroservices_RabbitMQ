using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotifcationService.Interfaces.Services
{
    public interface ISenderService
    {
        Task SendNotification(string header, string content, List<string> externalUserIds);
        Task SendNotification(string header, string content, string includedSegments);
        Task SendEmail(string to, string name, string subject, string body);
    }
}