using aahhaa.Api.Configuration;
using aahhaa.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAahhaaServices(builder.Configuration);
var app = builder.Build();

// Register the routes and handlers
app.Services.GetServices<IEndpoints>()
    .ToList()
    .ForEach(endpoints => endpoints.Register(app));

app.Run();