
using ConsultingKoiFish.API.ConfigExtensions;
using ConsultingKoiFish.BLL.Helpers.Config;
using ConsultingKoiFish.DAL;
using ConsultingKoiFish.DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ConsultingKoiFish.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(option =>
			{
				option.SwaggerDoc("v1", new OpenApiInfo { Title = "ConsultingKoiFish API", Version = "v1" });
				option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Please enter a valid token",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Bearer"
				});
				option.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type=ReferenceType.SecurityScheme,
								Id="Bearer"
							}
						},
						new string[]{}
					}
				});
			});

			//set up policy
			builder.Services.AddCors(opts =>
			{
				opts.AddPolicy("corspolicy", build =>
				{
					build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
				});
			});

			//set up DB
			var connectionString = builder.Configuration.GetConnectionString("ConsultingKoiFish");
			Console.WriteLine($"ConnectionString: {connectionString}");

			builder.Services.AddDbContext<ConsultingKoiFishContext>(options => options.UseSqlServer(connectionString));

			//set Identity
			builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ConsultingKoiFishContext>().AddDefaultTokenProviders();

			//set repo base
			builder.Services.AddRepoBase();

			//set Unit Of Work
			builder.Services.AddUnitOfWork();

			//set AutoMapper
			builder.Services.AddMapper();

			//set Services
			builder.Services.AddBLLServices();

			//Add Email Config
			var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
			builder.Services.AddSingleton(emailConfig);
			
			//Add VnPay Config
			var vnpConfig = builder.Configuration.GetSection("VnPayConfiguration").Get<VnPayConfiguration>();
			builder.Services.AddSingleton(vnpConfig);

			//Add config for Required Email
			builder.Services.Configure<IdentityOptions>(opts =>
			{
				opts.SignIn.RequireConfirmedEmail = true;
				opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				opts.Lockout.MaxFailedAccessAttempts = 5;
				opts.Lockout.AllowedForNewUsers = true;
			});

			//Add authentication
			builder.Services.AddAuthentication(options => {
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options => {
				options.SaveToken = true;
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidAudience = builder.Configuration["JWT:ValidAudience"],
					ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
					ClockSkew = TimeSpan.Zero,
					ValidateLifetime = true,
				};
			});


			//Add config for verify token
			builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(1));

			// API Behavior
			builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

			//Add HttpClient
			builder.Services.AddHttpClient();

			var app = builder.Build();

			// Seed data when the application starts
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					 services.SeedData().Wait();
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred while seeding the database.");
				}
			}

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseCors("corspolicy");
			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
