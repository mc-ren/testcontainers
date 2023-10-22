
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ContactsBook.Api.Repositories;
using ContactsBook.Api.Models;

namespace ContactsBook.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactsMongoRepository _contactsMongoRepository;
    private readonly IContactsPgRepository _contactsPgRepository;

    public ContactsController(IContactsMongoRepository contactsMongoRepository, IContactsPgRepository contactsPgRepository)
    {
        _contactsMongoRepository = contactsMongoRepository;
        _contactsPgRepository = contactsPgRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var returnList = new List<ContactResultModel>();
        var mongoContacts = await _contactsMongoRepository.GetAllAsync();
        foreach(var contact in mongoContacts)
        {
            returnList.Add(new ContactResultModel(contact, "mongo"));
        }
        var postgresContacts = await _contactsPgRepository.GetAllAsync();
        foreach(var contact in postgresContacts)
        {
            returnList.Add(new ContactResultModel(contact, "postgres"));
        }

        return Ok(returnList);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var mongoContact = await _contactsMongoRepository.GetByIdAsync(id);
        var postgresContact = await _contactsPgRepository.GetByIdAsync(id);
        
        var returnList = new List<ContactResultModel>
        {
            new(mongoContact, "mongo"),
            new(postgresContact, "postgres")
        };
        
        return Ok(returnList);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ContactModel model)
    {
        var contact = new Contact {
            Key = model.Id!,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email
        };

        await _contactsMongoRepository.CreateAsync(contact);
        await _contactsPgRepository.CreateAsync(contact);

        return Ok(new { message = "Contact created" });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] ContactModel model)
    {
        var contact = new Contact {
            Key = id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email
        };
        await _contactsMongoRepository.UpdateAsync(contact);
        await _contactsPgRepository.UpdateAsync(contact);

        return Ok(new { message = "Contact updated" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _contactsMongoRepository.DeleteAsync(id);
        await _contactsPgRepository.DeleteAsync(id);

        return Ok(new { message = "User deleted" });
    }
}