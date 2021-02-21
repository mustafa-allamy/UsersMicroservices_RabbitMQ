using Models.Enums;
using System;
using System.Collections.Generic;

namespace Models.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Head { get; set; }
        public string Content { get; set; }
        public List<Guid> IncludedUsersIds { get; set; }
        public string IncludedSegment { get; set; }
        public string Email { get; set; }
        public NotificationType NotificationType { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public bool IsDelevried { get; set; }
    }
}