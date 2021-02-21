namespace Models.Forms.Create
{
    public interface ICreateUserForm
    {
        string FullName { get; set; }
        string Email { get; set; }
        string PhoneNumber { get; set; }
    }
}