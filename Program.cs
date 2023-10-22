using System.Text.Json.Serialization;
using WebApi;
using WebApi.Helpers;
using WebApi.Repositories;

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

    // Inject IConfiguration in the constructor of your repository classes.
    services.AddScoped<IContactsMongoRepository>(mongo =>
    {
        return new ContactsMongoRepository(builder.Configuration.GetConnectionString("MongoDb"));
    });

    services.AddScoped<IContactsPgRepository>(postgres =>
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

app.Run("http://localhost:4000");

public partial class Program {}