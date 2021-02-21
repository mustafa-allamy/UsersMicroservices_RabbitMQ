using System;

namespace Models.Dtos
{
    public class UserUpdatedNotificationDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}