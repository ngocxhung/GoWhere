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

            // Add SignalR
            builder.Services.AddSignalR();
            // ✅ Configure Swagger with JWT Bearer Authentication
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "GoWhere API",
                    Version = "v1",
                    Description = "API for GoWhere Travel Platform - Vietnam Tourism",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "GoWhere Team",
                        Email = "support@gowhere.vn"
                    }
                });

                // Add JWT Authentication
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                                  "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                                  "Example: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
                });
               
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
            
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
                    // Add SignalR Jwt Authentication
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
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
                
                // Auto-create AIScheduleHistories table if not exists
                try
                {
                    var tableExists = dbContext.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AIScheduleHistories')
                        BEGIN
                            CREATE TABLE [dbo].[AIScheduleHistories] (
                                [Id] INT IDENTITY(1,1) NOT NULL,
                                [UserId] INT NOT NULL,
                                [Destination] NVARCHAR(MAX) NOT NULL,
                                [ProvinceId] INT NULL,
                                [Budget] DECIMAL(18, 2) NOT NULL,
                                [NumberOfDays] INT NOT NULL,
                                [TravelStyle] NVARCHAR(MAX) NOT NULL,
                                [CompanionType] NVARCHAR(MAX) NOT NULL,
                                [Pax] INT NOT NULL,
                                [ScheduleJson] NVARCHAR(MAX) NOT NULL,
                                [Summary] NVARCHAR(MAX) NULL,
                                [EstimatedTotalCost] DECIMAL(18, 2) NOT NULL,
                                [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                                CONSTRAINT [PK_AIScheduleHistories] PRIMARY KEY CLUSTERED ([Id] ASC),
                                CONSTRAINT [FK_AIScheduleHistories_Users_UserId] FOREIGN KEY ([UserId]) 
                                    REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE,
                                CONSTRAINT [FK_AIScheduleHistories_Provinces_ProvinceId] FOREIGN KEY ([ProvinceId]) 
                                    REFERENCES [dbo].[Provinces] ([ProvinceId]) ON DELETE NO ACTION
                            );
                            
                            CREATE NONCLUSTERED INDEX [IX_AIScheduleHistories_UserId] 
                                ON [dbo].[AIScheduleHistories] ([UserId] ASC);
                            
                            CREATE NONCLUSTERED INDEX [IX_AIScheduleHistories_ProvinceId] 
                                ON [dbo].[AIScheduleHistories] ([ProvinceId] ASC);
                        END
                    ");
                    Console.WriteLine("✅ AIScheduleHistories table checked/created successfully");

                    // Add AllowedCount to AISuggestionRequests if not exists and sync existing data
                    dbContext.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AISuggestionRequests' AND COLUMN_NAME = 'AllowedCount')
                        BEGIN
                            ALTER TABLE [dbo].[AISuggestionRequests] ADD [AllowedCount] INT DEFAULT 2 NOT NULL;
                        END
                    ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Warning: Could not create AIScheduleHistories table: {ex.Message}");
                }
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

            app.UseAuthentication();  // ✅ Add this BEFORE UseAuthorization
            app.UseAuthorization();
            app.MapHub<Hubs.NotificationHub>("/hubs/notifications");
            app.MapControllers();

            app.Run();
        }
    }
}
