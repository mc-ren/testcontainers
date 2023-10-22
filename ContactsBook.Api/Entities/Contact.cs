using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ContactsBook.Api;

public class Contact
{
    public ObjectId _id { get; set; }
    
    public string Key { get; set; } = default!;

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; }= default!;

    public string Email { get; set; }= default!;
}