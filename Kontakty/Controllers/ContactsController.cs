using Kontakty.Dto.AddContact;
using Kontakty.Dto.GetContact;
using Kontakty.Enums;
using Kontakty.Mappers;
using Kontakty.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Kontakty.Controllers
{
    
    [ApiController]
    public class ContactsController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly CategoryMapper _categoryMapper;

        public ContactsController(DatabaseContext databaseContext, CategoryMapper categoryMapper)
        {
            _databaseContext = databaseContext;
            _categoryMapper = categoryMapper;
        }
        //edytuj

        [HttpPost("/addContact")]
        public IActionResult AddContact([FromBody] AddContactRequest addContactRequest)
        {
            if (!PasswordIsCorrect(addContactRequest.Password))
                return BadRequest("Password is not correct");

            if (!EmailIsUniq(addContactRequest.Email))
                return BadRequest("Email is not uniqe");

            if (BirthDateIsFeature(addContactRequest.DateOfBirth))
                return BadRequest("The date is futuree");

            var mapCategory = _categoryMapper.MapCategory(addContactRequest.Category);

            Category category;

            if (mapCategory is CategoryEnum.Private)
            {
                category = _databaseContext.Categories.Where(category => category.Name == addContactRequest.Category).FirstOrDefault();
            }
            else if (mapCategory is CategoryEnum.Buissnes)
            {
                category = _databaseContext.Categories.Where(category => category.Name == addContactRequest.Category
                                                       && category.Subcategory.Name == addContactRequest.Subcategory).FirstOrDefault();

                if (category == null)
                {
                    var newCategory = _databaseContext.Categories.Where(category => category.Name == addContactRequest.Category).FirstOrDefault();
                    var newSubcategory = _databaseContext.Subcategories.Where(subcategory => subcategory.Name == addContactRequest.Subcategory).FirstOrDefault();

                    if (newSubcategory is null)
                        return BadRequest("Wrong subcategory");

                    category = new Category()
                    {
                        Name = newCategory.Name,
                        Subcategory = new Subcategory()
                        {
                            Name = addContactRequest.Subcategory,
                        }
                    };
                }
            }
            else
            {
                category = _databaseContext.Categories.Where(category => category.Name == addContactRequest.Category
                                                       && category.Subcategory.Name == addContactRequest.Subcategory).FirstOrDefault();

                if (category == null)
                {
                    var newCategory = _databaseContext.Categories.Where(category => category.Name == addContactRequest.Category).FirstOrDefault();

                    category = new Category()
                    {
                        Name = newCategory.Name,
                        Subcategory = new Subcategory()
                        {
                            Name = addContactRequest.Subcategory,
                        }
                    };
                }
            }

            var contact = new Contact
            {
                FirstName = addContactRequest.FirstName,
                LastName = addContactRequest.LastName,
                Email = addContactRequest.Email,
                Password = addContactRequest.Password,
                Phone = addContactRequest.Phone,
                DateOfBirth = DateTime.Parse(addContactRequest.DateOfBirth),
                Category = category,
            };

            _databaseContext.Contacts.Add(contact);
            _databaseContext.SaveChanges(); 

            return Ok();
        }

        [HttpGet("/getAllContacts")]
        public ActionResult<IList<GetResponse>> GetAllContacts()
        {
            var contacts = _databaseContext.Contacts.Where(x => true)
                .Include(x => x.Category)
                .Include(y => y.Category.Subcategory)
                .ToList();
            var response = new List<GetResponse>();

            foreach (var contact in contacts)
            {
                var getResponse = new GetResponse()
                {
                    Id = contact.Id,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Email = contact.Email,
                    Category = contact.Category.Name,
                    Subcategory = contact.Category.Subcategory.Name,
                    DateOfBirth = contact.DateOfBirth.ToString(),
                    Password = contact.Password,
                    Phone = contact.Phone,
                };

                response.Add(getResponse);
            }

            return response;
        }

        [HttpGet("/getContactById")]
        public ActionResult<GetResponse> GetById([FromBody] Guid id)
        {
            var contacts = _databaseContext.Contacts
                .Include(x => x.Category)
                .Include(y => y.Category.Subcategory)
                .Where(contact => contact.Id == id).FirstOrDefault();

            if(contacts is null)
                return BadRequest("This contact not exist!");

            return Ok(contacts);
        }

        [HttpDelete("/deleteContact")]
        public IActionResult DeleteContacts([FromQuery] Guid id)
        {
            var contact = _databaseContext.Contacts
                .Where(contact=> contact.Id == id).FirstOrDefault();

            if (contact is null)
                return BadRequest("This contact not exist!");

            _databaseContext.Contacts.Remove(contact);
            _databaseContext.SaveChanges();

            return Ok();
        }


        private bool PasswordIsCorrect(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%^*()#])[A-Za-z\d@$!%^*()#]{8,}$");
        }

        private bool BirthDateIsFeature(string dateOfBirth)
        {
            return DateTime.Parse(dateOfBirth).CompareTo(DateTime.Now) > 0;
        }

        private bool EmailIsUniq(string email)
        {
            return !_databaseContext.Contacts.Any(contact => contact.Email == email);
        }
    }
}
