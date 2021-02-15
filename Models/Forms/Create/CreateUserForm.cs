using System.ComponentModel.DataAnnotations;

namespace Models.Forms.Create
{
    public class CreateUserForm
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        public string Role { get; set; }

    }
}