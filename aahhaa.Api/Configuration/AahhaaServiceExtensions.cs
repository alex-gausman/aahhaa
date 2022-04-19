using aahhaa.Api.Endpoints;
using aahhaa.Core.Models;
using aahhaa.Infrastructure.Data;
using aahhaa.Infrastructure.Data.Repositories;
using aahhaa.Shared.Data.Interfaces;
using FluentValidation;
using LiteDB.Async;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace aahhaa.Api.Configuration
{
    public static class AahhaaServiceExtensions
    {
        public static void AddAahhaaServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEndpointsProvidesMetadataApiExplorer();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.Configure<LiteDatabaseOptions>(configuration.GetSection(nameof(LiteDatabaseOptions)));
            
            services.AddTransient<ILiteDatabaseAsync>(services =>
            {
                var options = services.GetRequiredService<IOptions<LiteDatabaseOptions>>().Value;
                return new LiteDatabaseAsync(options.DatabasePath);
            });
            
            services.AddTransient<IRepository<User>, Repository<User>>();
            services.AddTransient<IRepository<Note>, Repository<Note>>();

            services.AddTransient<IEndpoints, UsersEndpoints>();
            services.AddTransient<IEndpoints, NotesEndpoints>();
        }
    }
}
