using DbUp;
using Microsoft.Extensions.Configuration;
using Npgsql;


var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

string connectionString = configuration.GetConnectionString("Default") ?? throw new NullReferenceException();

// FOR DEVELOPMENT WE DELETE THE DATABASE EVERYTIME FIRST
string adminConnectionString = configuration.GetConnectionString("Admin") ?? throw new NullReferenceException();
try
{
    using var connection = new NpgsqlConnection(adminConnectionString);
    await connection.OpenAsync();
    using var command = connection.CreateCommand();
    command.CommandText = "DROP DATABASE IF EXISTS \"BlazorERP\"";
    await command.ExecuteNonQueryAsync();
}
catch (Exception)
{


}


EnsureDatabase.For.PostgresqlDatabase(connectionString);

var upgrader = DeployChanges.To.PostgresqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(typeof(Program).Assembly)
    .LogToConsole()
    .Build();

if (upgrader.IsUpgradeRequired())
{
    var result = upgrader.PerformUpgrade();

    if (!result.Successful)
    {

    }
}