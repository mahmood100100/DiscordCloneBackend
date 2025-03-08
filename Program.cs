using DiscordCloneBackend.Application.Common.Response;
using DiscordCloneBackend.Application.IServices;
using DiscordCloneBackend.Application.Mappings;
using DiscordCloneBackend.Application.Services;
using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Interfaces.IExternalServices;
using DiscordCloneBackend.Core.Interfaces.INotificationServices;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.Data;
using DiscordCloneBackend.Infrastructure.ExternalServices;
using DiscordCloneBackend.Infrastructure.NotificationServices;
using DiscordCloneBackend.Infrastructure.Repositories;
using DiscordCloneBackend.Presentation.Hubs;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DiscordCloneBackend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            DotEnv.Load();
            var builder = WebApplication.CreateBuilder(args);

            // Load secret key and ensure it's not missing
            var key = builder.Configuration["ApiSettings:SecretKey"];
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("JWT Secret Key is missing in configuration.");
            }

            // Add DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register repositories
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IServerRepository, ServerRepository>();
            builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IMemberRepository, MemberRepository>();
            builder.Services.AddScoped<IDirectMessageRepository, DirectMessageRepository>();
            builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
            builder.Services.AddScoped<IChannelRepository, ChannelRepository>();

            // Register services
            builder.Services.AddScoped(typeof(IGenericService<,,>), typeof(GenericService<,,>));
            builder.Services.AddScoped<IServerService, ServerService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IDirectMessageService, DirectMessageService>();
            builder.Services.AddScoped<IConversationService, ConversationService>();
            builder.Services.AddScoped<IChannelService, ChannelService>();
            builder.Services.AddScoped<IUserService, UserService>();

            // External services
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IFileService, FileService>();

            //Hub Notification services
            builder.Services.AddScoped<IMemberNotificationService, MemberNotificationService>();
            builder.Services.AddScoped<IChannelNotificationService, ChannelNotificationService>();
            builder.Services.AddScoped<IServerNotificationService, ServerNotificationService>();
            builder.Services.AddScoped<IMessageNotificationService, MessageNotificationService>();
            builder.Services.AddScoped<IDirectMessageNotificationService, DirectMessageNotificationService>();
            builder.Services.AddScoped<IUserNotificationService, UserNotificationService>();

            // Configure AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Configure Identity with enhanced security
            builder.Services.AddIdentity<LocalUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // Configure custom bad request response
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return new BadRequestObjectResult(new ApiValidationResponse(StatusCode: 400, Errors: errors));
                };
            });

            // Configure JWT Authentication with detailed logging
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidateAudience = false,
                        ValidateIssuer = false
                    };

                    // Add JWT event handlers for logging
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("Token validated successfully.");
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            Console.WriteLine("Authentication challenge triggered.");
                            return Task.CompletedTask;
                        }
                    };
                });

            // Configure Data Protection Token Lifespan
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(15);
            });

            // Register Controllers & API Exploration
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure CORS
            var origin = builder.Configuration["CorsSettings:FrontendUrl"];
            Console.WriteLine(origin);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(origin)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            //SignalR configuring
            builder.Services.AddSignalR();

            var app = builder.Build();

            // Seed the database
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var context = serviceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    await SeedData.Initialize(serviceProvider, context);
                }
                catch (Exception ex)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            // Configure Middleware Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");

            // Enable Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Map Controllers
            app.MapControllers();

            // Map SignalR Hub
            app.MapHub<MemberHub>("/memberHub");
            app.MapHub<ChannelHub>("/channelHub");
            app.MapHub<ServerHub>("/serverHub");
            app.MapHub<MessageHub>("/messageHub");
            app.MapHub<DirectMessageHub>("/directMessageHub");
            app.MapHub<UserHub>("/userHub");

            app.Run();
        }
    }
}