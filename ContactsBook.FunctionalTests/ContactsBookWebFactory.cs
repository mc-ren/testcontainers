using Microsoft.AspNetCore.Mvc.Testing;

public class ContactsBookWebFactory : WebApplicationFactory<Program>
{
    private readonly string _testContainerMongoConnectionString;
    private readonly string _testContainerPgConnectionString;

    public ContactsBookWebFactory(string testContainerMongoConnectionString, string testContainerPgConnectionString)
    {
        _testContainerMongoConnectionString = testContainerMongoConnectionString;
        _testContainerPgConnectionString = testContainerPgConnectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"ConnectionStrings:MongoDb",_testContainerMongoConnectionString },
                {"ConnectionStrings:Postgres",_testContainerPgConnectionString }
            });
        });
    }
}