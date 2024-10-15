
using ConsultingKoiFish.API.ConfigExtensions;
using ConsultingKoiFish.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
			builder.Services.AddSwaggerGen();

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
			builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ConsultingKoiFishContext>().AddDefaultTokenProviders();

			//set repo base
			builder.Services.AddRepoBase();

			//set Unit Of Work
			builder.Services.AddUnitOfWork();

			//set AutoMapper
			builder.Services.AddMapper();

			//set Services
			builder.Services.AddBLLServices();

			// API Behavior
			builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

			var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AVR API V1");
        c.RoutePrefix = string.Empty;
        c.EnableTryItOutByDefault();
    });
}

			app.UseCors("corspolicy");
			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
