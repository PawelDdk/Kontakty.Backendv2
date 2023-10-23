using Kontakty.Models;

namespace Kontakty.Dto.AddContact
{
    public class AddContactRequest
    {
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
