using crud.Models;
using crud.Repositories;
using crud.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace crud
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000")  // Allow frontend URL
                              .AllowAnyMethod()                      // Allow all HTTP methods
                              .AllowAnyHeader()                      // Allow all headers
                              .AllowCredentials();                   // Allow credentials (if needed)
                    });
            });
            // Add this temporary diagnostic code in Program.cs at the beginning
            Console.WriteLine("JWT Key: " + builder.Configuration["Jwt:key"]);
            Console.WriteLine("JWT Issuer: " + builder.Configuration["Jwt:Issuer"]);
            Console.WriteLine("JWT Audience: " + builder.Configuration["Jwt:Audience"]);

            // ✅ Configure Database Connection
            builder.Services.AddDbContext<SpendSmartDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 0))
                )
            );

            // ✅ Register Services and Repositories
            builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IExpenseService, ExpenseService>();

            // ✅ Configure JWT Authenticationvar
             var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                            Console.WriteLine("Auth header received: " + authHeader);
                            if (authHeader != null && authHeader.StartsWith("Bearer "))
                            {
                                var token = authHeader.Substring(7);
                                Console.WriteLine("Token length: " + token.Length);
                                Console.WriteLine("Token bytes: " + string.Join(",", Encoding.UTF8.GetBytes(token)));
                                Console.WriteLine("Dots count: " + token.Count(c => c == '.'));
                            }

                            return Task.CompletedTask;
                        }
                        };
                    });

                        // ✅ Add Controllers
            builder.Services.AddControllers();

            // ✅ Configure Swagger with JWT Authentication
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // 🔹 Add JWT Authentication support in Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter 'Bearer {your_token}' without quotes",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();
            //var expTime = DateTimeOffset.FromUnixTimeSeconds(1741589444).UtcDateTime;
            //Console.WriteLine("Token Expiry Time (UTC): " + expTime);


            // ✅ Enable Swagger UI
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }

            // ✅ Middleware Order (Important)
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
