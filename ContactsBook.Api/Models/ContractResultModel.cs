using ContactsBook.Api;

namespace ContactsBook.Api.Models;

public record ContactResultModel 
{
    public ContactResultModel(Contact contact, string db)
    {
        Db = db;
        Contact = new ContactModel { 
            Email = contact.Email, 
            FirstName = contact.FirstName, 
            LastName = contact.LastName, 
            Id = contact.Key
        };
    }

    public string Db { get; set;}
    public ContactModel Contact { get; set;}
}