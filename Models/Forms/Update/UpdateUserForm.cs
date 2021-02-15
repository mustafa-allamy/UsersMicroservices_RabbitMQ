using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Forms.Update
{
    public class UpdateUserForm
    {
        [Required]
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}