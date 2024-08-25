var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var dashBoardPort = builder.Configuration["ORLEANS-SILO-DASHBOARD"]!;
int.TryParse(dashBoardPort, out var dashBoardPortInt);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddKeyedRedisClient("redis");
builder.UseOrleans(option =>
{
    option.UseDashboard(option =>
    {
        option.Port = dashBoardPortInt;
    });
});
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/greet/{name}", (string name, IGrainFactory grainFactory) =>
{
    var helloGrain = grainFactory.GetGrain<IHelloGrain>(name);
    return helloGrain.SayHello(name);
})
.WithOpenApi();

app.Run();





public interface IHelloGrain : IGrainWithStringKey
{
    Task<string> SayHello(string name);
}


public class HelloGrain : IHelloGrain
{
    public Task<string> SayHello(string name)
    {
        return Task.FromResult($"Good day {name}");
    }
}