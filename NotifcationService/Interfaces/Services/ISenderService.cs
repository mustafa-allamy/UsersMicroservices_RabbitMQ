using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotifcationService.Interfaces.Services
{
    public interface ISenderService
    {
        /// <summary>
        /// Use to send notification to specified users
        /// </summary>
        /// <param name="header"></param>
        /// <param name="content"></param>
        /// <param name="externalUserIds"></param>
        /// <returns></returns>
        Task SendNotification(string header, string content, List<string> externalUserIds);
        /// <summary>
        /// Use to send notification to segment
        /// </summary>
        /// <param name="header"></param>
        /// <param name="content"></param>
        /// <param name="includedSegments"></param>
        /// <returns></returns>
        Task SendNotification(string header, string content, string includedSegments);
        Task SendEmail(string to, string name, string subject, string body);
    }
}