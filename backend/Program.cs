using GoWhere_Api.Repo.IRepo;
using GoWhere_Api.Service.IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GoWhere_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            // Use default Swagger generation; security UI will be provided via injected JS in Swagger UI
            builder.Services.AddSwaggerGen();
            
            // Add HttpClient for AI service
            builder.Services.AddHttpClient();
            
            builder.Services.Scan(s => s
                .FromAssemblyOf<IUserRepo>()
                .AddClasses(classes => classes
                    .Where(type => type.Name.EndsWith("Repo")))
                .AsMatchingInterface()
                .WithScopedLifetime()   );
            builder.Services.Scan(s => s
                .FromAssemblyOf<IAuthService>()
                .AddClasses(classes => classes
                    .Where(type => type.Name.EndsWith("Service")))
                .AsMatchingInterface()
                .WithScopedLifetime());
            builder.Services.AddDbContext<Model.ApplicationDbContext>(opt
                => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddOData(option => option.Select().Filter().OrderBy().Expand().SetMaxTop(100).Count());
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCors(option =>
            {
                option.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
                option.AddPolicy("RestrictedCorsPolicy", builder =>
                {
                    builder.WithOrigins("https://localhost:3000", "http://localhost:3001")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                            .AllowCredentials()
                            .WithExposedHeaders("Content-Disposition")
                        .SetIsOriginAllowed(origin => true);

                });
            });

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Model.ApplicationDbContext>();
                dbContext.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Swashbuckle middleware
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoWhere API V1");
                });

                // Minimal API OpenAPI mapping (keeps existing behavior)
            }
            app.UseStaticFiles();
            app.UseCors("RestrictedCorsPolicy");
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
