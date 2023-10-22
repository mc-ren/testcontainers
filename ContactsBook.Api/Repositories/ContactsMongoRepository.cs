using MongoDB.Bson;
using MongoDB.Driver;

namespace ContactsBook.Api.Repositories;

public interface IContactsMongoRepository
{
    Task<IEnumerable<Contact>> GetAllAsync();
    Task<Contact> GetByIdAsync(string id);
    Task CreateAsync(Contact contact);
    Task UpdateAsync(Contact contact);
    Task DeleteAsync(string id);
}

public class ContactsMongoRepository : IContactsMongoRepository
{
    private IMongoDatabase _mongoDatabase;

    public ContactsMongoRepository(string connectionString)
    {
        var mongoClient = new MongoClient(connectionString);
        _mongoDatabase = mongoClient.GetDatabase("contactsBook");
    }

    public async Task CreateAsync(Contact contact)
    {
        var contacts = _mongoDatabase.GetCollection<Contact>("contacts");
        await contacts.InsertOneAsync(contact);
    }

    public async Task DeleteAsync(string id)
    {
        var contacts = _mongoDatabase.GetCollection<Contact>("contacts");
        var contact = await contacts.Find(e => e.Key == id).FirstOrDefaultAsync();
        await contacts.DeleteManyAsync(e => e.Key == contact.Key);
    }

    public async Task<IEnumerable<Contact>> GetAllAsync()
    {
        var contacts = _mongoDatabase.GetCollection<Contact>("contacts");
        return await contacts.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<Contact> GetByIdAsync(string id)
    {
        var contacts = _mongoDatabase.GetCollection<Contact>("contacts");
        var contact = await contacts.Find(e => e.Key == id).FirstOrDefaultAsync();
        return contact;
    }

    public async Task UpdateAsync(Contact contact)
    {
        var contacts = _mongoDatabase.GetCollection<Contact>("contacts");
        var contactToUpdate = await contacts.Find(e => e.Key == contact.Key).FirstOrDefaultAsync();

        contactToUpdate.FirstName = contact.FirstName;
        contactToUpdate.LastName = contact.LastName;
        contactToUpdate.Email = contact.Email;

        await contacts.ReplaceOneAsync(e => e.Key == contact.Key, contactToUpdate);
    }
}