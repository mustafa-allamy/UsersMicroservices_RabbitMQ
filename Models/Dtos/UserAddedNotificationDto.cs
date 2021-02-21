using System;

namespace Models.Dtos
{
    public class UserAddedNotificationDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}