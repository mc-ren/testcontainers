using System.Text.Json.Serialization;
using ContactsBook.Api.Helpers;
using ContactsBook.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;
 
    services.AddControllers().AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // ignore omitted parameters on models to enable optional params (Contact update)
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

    services.AddScoped<IContactsMongoRepository>(_ =>
    {
        return new ContactsMongoRepository(builder.Configuration.GetConnectionString("MongoDb"));
    });

    services.AddScoped<IContactsPgRepository>(_ =>
    {
        return new ContactsPgRepository(builder.Configuration.GetConnectionString("Postgres"));
    });

}

var app = builder.Build();

// configure HTTP request pipeline
{
    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();
    app.MapControllers();
}

app.Run("http://localhost:4000"); // for local use only 

public partial class Program {}