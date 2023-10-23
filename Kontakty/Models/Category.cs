namespace Kontakty.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Subcategory? Subcategory { get; set; }
    }
}
