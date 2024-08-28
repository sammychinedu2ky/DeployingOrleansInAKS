var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var dashBoardPort = builder.Configuration["ORLEANS-SILO-DASHBOARD"]!;
var dashboardPortInt = Convert.ToInt32(dashBoardPort);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddKeyedRedisClient("redis");
builder.UseOrleans(option =>
{
    if(builder.Environment.IsProduction())
    {
        option.UseKubernetesHosting();
    }

    option.UseDashboard(option =>
    {
        option.BasePath = "/dashboard";
        option.Port = dashboardPortInt;
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



app.MapGet("/greet/{name}", (string name, IGrainFactory grainFactory) =>
{
    var helloGrain = grainFactory.GetGrain<IHelloGrain>(name);
    return helloGrain.SayHello(name);
})
.WithOpenApi();

app.MapGet("/", () => "Hello World");

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