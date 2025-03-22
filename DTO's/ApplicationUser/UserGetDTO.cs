namespace Ecommerce.DTO_s.ApplicationUser
{
    public class UserGetDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Location { get; set; }
        public string? Bio { get; set; }
    }
}
