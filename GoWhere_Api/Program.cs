using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
// using Microsoft.OpenApi.Models; // not required when calling AddSwaggerGen with defaults
using System.Text;

namespace GoWhere_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            // Add Swagger (Swashbuckle)
            builder.Services.AddSwaggerGen();
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
                app.MapOpenApi();
            }
            app.UseCors("RestrictedCorsPolicy");
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
