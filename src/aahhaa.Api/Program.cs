using aahhaa.Api.Configuration;
using aahhaa.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAahhaaServices(builder.Configuration);
var app = builder.Build();

// Enable CORS for any domain
app.UseCors(policy => {
    policy.AllowAnyOrigin();
    policy.AllowAnyMethod();
    policy.AllowAnyHeader();
});

// Register the routes and handlers
app.Services.GetServices<IEndpoints>()
    .ToList()
    .ForEach(endpoints => endpoints.Register(app));

app.Run();