namespace Kontakty.Dto.GetContact
{
    public class GetResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string Phone { get; set; }
        public string DateOfBirth { get; set; }
    }
}
