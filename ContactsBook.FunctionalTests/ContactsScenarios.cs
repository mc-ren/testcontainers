using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;
using ContactsBook.Api;
using ContactsBook.Api.Controllers;
using ContactsBook.Api.Repositories;
using FluentAssertions;
using Npgsql;
using System.Data;
using ContactsBook.Api.Models;

namespace ContactsBook.FunctionalTests
{
    public class ContactsScenarios : IAsyncLifetime
    {
        private readonly MongoDbContainer _mongoDbContainer;
        private readonly PostgreSqlContainer _postgreSqlContainer;

        public ContactsScenarios()
        {
            _mongoDbContainer = new MongoDbBuilder().Build();
            _postgreSqlContainer = new PostgreSqlBuilder().Build();
        }

        [Fact]
        public async Task ShouldSuccessfullyRunCrudOperationsForDatabases()
        {
            var mongoRepo = new ContactsMongoRepository(_mongoDbContainer.GetConnectionString());
            var postgresRepo = new ContactsPgRepository(_postgreSqlContainer.GetConnectionString());

            var contactController = new ContactsController(mongoRepo, postgresRepo);

            var id = ObjectId.GenerateNewId().ToString();

            var testContact = new ContactModel // Prepare the test data
            {
                Id = id,
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@mail.cop",
            };
            
            // create
            await contactController.Create(testContact);

            // read
            var result = await contactController.GetById(id) as OkObjectResult;
            var contacts = result!.Value as IEnumerable<ContactResultModel>;
            var contactsList = contacts!.ToList();
            contactsList[0].Contact.Id.Should().Be(id);
            contactsList[1].Contact.Id.Should().Be(id);
            contactsList.Any(x => x.Db == "mongo").Should().BeTrue();
            contactsList.Any(x => x.Db == "postgres").Should().BeTrue();

            // read
            var results = await contactController.GetAll() as OkObjectResult;
            contacts = result!.Value as IEnumerable<ContactResultModel>;
            contactsList = contacts!.ToList();

            contactsList.Count.Should().BeGreaterThan(1);

            var newName = "Test2";

            testContact = new ContactModel // Prepare the test data
            {
                Id = id,
                FirstName = newName,
                LastName = "Test",
                Email = "Test@mail.cop",
            };

            // update
            await contactController.Update(id, testContact);

            result = await contactController.GetById(id) as OkObjectResult;
            var updatedContacts = result!.Value as IEnumerable<ContactResultModel>;
            contactsList = updatedContacts!.ToList();
            contactsList[0].Contact.FirstName.Should().Be(newName);
            contactsList[1].Contact.FirstName.Should().Be(newName);
            contactsList.Any(x => x.Db == "mongo").Should().BeTrue();
            contactsList.Any(x => x.Db == "postgres").Should().BeTrue();

            // delete
            await contactController.Delete(id);

            var finalResults = await contactController.GetAll() as OkObjectResult;
            var finalContacts = finalResults!.Value as IEnumerable<ContactResultModel>;
            var finalContactsList = finalContacts!.ToList();

            finalContactsList.Count.Should().Be(0);      
        }

        public async Task DisposeAsync()
        {
            await _mongoDbContainer.DisposeAsync().AsTask();
            await _postgreSqlContainer.DisposeAsync().AsTask();
        }

        public async Task InitializeAsync()
        {
            await _mongoDbContainer.StartAsync();
            await _postgreSqlContainer.StartAsync();

            SetupPostgres(_postgreSqlContainer.GetConnectionString());
        }

        private static void SetupPostgres(string connectionString) 
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;

                var sql = "CREATE TABLE IF NOT EXISTS Contacts (" +
                    "Key VARCHAR, " +
                    "FirstName VARCHAR, " +
                    "LastName VARCHAR, " +
                    "Email VARCHAR " +
                    ");";

                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}
