using Kontakty.Enums;

namespace Kontakty.Mappers
{
    public class CategoryMapper
    {
        public CategoryEnum MapCategory(string categoryName)
        {
            switch(categoryName)
            {
                case "Buissnes": return CategoryEnum.Buissnes;
                case "Private": return CategoryEnum.Private;
                case "Other": return CategoryEnum.Other;
                default: return CategoryEnum.Other;
            }
        }
    }
}
