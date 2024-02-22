using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PseudoFTP.Api.Tasks;
using PseudoFTP.Extension;
using PseudoFTP.Model.Database;
using Tonisoft.AspExtensions.Cors;
using Tonisoft.AspExtensions.Module;

namespace PseudoFTP.Api;

class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Configure Database.
        Globals.Profile = builder.Configuration["Profile"] ?? "Default";
        Globals.Version = builder.Configuration["Version"] ?? "Undefined";
        builder.Services.AddUnitOfWork<PrimaryDbContext>();
        ConfigureDatabase<PrimaryDbContext>(builder.Services, builder.Configuration);

        // Add modules to the container.
        builder.Services.RegisterModules(typeof(Program));

        // Add automapper.
        var autoMapperConfig = new MapperConfiguration(config => { config.AddProfile(new AutoMapperProfile()); });
        builder.Services.AddSingleton(autoMapperConfig.CreateMapper());

        builder.Services.AddControllers().AddNewtonsoftJson();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Cors Policy.
        var corsOptions = new CorsOptions();
        builder.Configuration.GetRequiredSection(CorsOptions.CorsSection).Bind(corsOptions);
        if (corsOptions.Enable)
        {
            builder.Services.AddCors(options => {
                options.AddPolicy(
                    CorsOptions.CorsPolicyName,
                    policy => {
                        if (corsOptions.AllowAny)
                        {
                            policy.AllowAnyOrigin();
                        }
                        else
                        {
                            foreach (string origin in corsOptions.Origins)
                            {
                                policy.WithOrigins(origin);
                            }

                            policy.AllowCredentials();
                        }

                        policy.AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
        }

        // Background Task
        builder.Services.AddSingleton<TransferTask>();
        builder.Services.AddHostedService(provider => provider.GetRequiredService<TransferTask>());

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseCors(CorsOptions.CorsPolicyName);
        app.MapControllers();

        app.Run();
    }

    private static void ConfigureDatabase<TContext>(IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext
    {
        string database = configuration.GetConnectionString("Database") ?? throw new Exception("Missing database");
        string connection = configuration.GetConnectionString("DefaultConnection") ??
                            throw new Exception("Missing database connection");

        switch (database)
        {
            case "MySQL":
                services.AddDbContext<TContext>(option => { option.UseMySQL(connection); });
                break;
            case "SQLite":
                services.AddDbContext<TContext>(option => { option.UseSqlite(connection); });
                break;
            default:
                throw new Exception($"Invalid database: {database}");
        }
    }
}