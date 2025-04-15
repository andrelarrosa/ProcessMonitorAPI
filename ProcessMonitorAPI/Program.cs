using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/GetAllProcesses", () =>
    {
        Process[] processes = Process.GetProcesses();
        List<Dictionary<string, string>> namesAndIds = new(); 
        foreach (var process in processes)
        {
            Dictionary<string, string> nameAndId = new Dictionary<string, string>();
            nameAndId.Add("name", process.ProcessName);
            nameAndId.Add("id", process.Id.ToString());
            namesAndIds.Add(nameAndId);
        }

        return namesAndIds;
    })
    .WithName("GetWeatherForecast");

app.Run();

