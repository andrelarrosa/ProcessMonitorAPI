using System.Diagnostics;
using WebApplication1.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();
app.UseCors(); 
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

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

app.MapHub<ProcessHub>("/processhub");

app.Run();

