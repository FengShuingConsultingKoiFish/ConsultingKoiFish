using ConsultingKoiFish.BLL.Helpers.Mapper;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Repositories;
using ConsultingKoiFish.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using CloudinaryDotNet;
using ConsultingKoiFish.BLL.Helpers.Validations.PurchasePackagedValidation;

namespace ConsultingKoiFish.API.ConfigExtensions
{
	public static class ServiecsExtensions
	{
		//Unit Of Work
		public static void AddUnitOfWork(this IServiceCollection services)
		{
			services.AddScoped<IUnitOfWork, UnitOfWork>();
		}

		//RepoBase
		public static void AddRepoBase(this IServiceCollection services)
		{
			services.AddScoped(typeof(IRepoBase<>), typeof(RepoBase<>));
		}

		//BLL Services
		public static void AddBLLServices(this IServiceCollection services)
		{
			services.Scan(scan => scan
					.FromAssemblies(Assembly.Load("ConsultingKoiFish.BLL"))
					.AddClasses(classes => classes.Where(type => type.Namespace == $"ConsultingKoiFish.BLL.Services.Implements" && type.Name.EndsWith("Service")))
					.AsImplementedInterfaces()
					.WithScopedLifetime());
		}

		//Add BackGround Service
		public static void AddBackGroundService(this IServiceCollection services)
		{
			services.AddHostedService<PurchasedPackageExpirationChecker>();
		}

		//add auto mapper
		public static void AddMapper(this IServiceCollection services)
		{
			services.AddAutoMapper(typeof(MappingProfile));
		}

		//Seed Data
		public static async Task SeedData(this IServiceProvider serviceProvider)
		{
			var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			var adminEmail = "minhtam14231204@gmail.com";
			var adminUserName = "Admin";
			var adminPhoneNumber = "0942775673";
			var adminPassword = "ThisIsAdmin123456@";

			// Seed Roles
			if (!await roleManager.RoleExistsAsync("Admin"))
			{
				await roleManager.CreateAsync(new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "ADMIN" });
			}

			if (!await roleManager.RoleExistsAsync("Member"))
			{
				await roleManager.CreateAsync(new IdentityRole() { Name = "Member", ConcurrencyStamp = "2", NormalizedName = "MEMBER" });
			}

			// Seed Admin User
			if (await userManager.FindByEmailAsync(adminEmail) == null)
			{
				var adminUser = new ApplicationUser
				{
					Email = adminEmail,
					UserName = adminUserName,
					PhoneNumber = adminPhoneNumber,
					EmailConfirmed = true
				};

				var result = await userManager.CreateAsync(adminUser, adminPassword);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, "Admin");
				}
			}
		}

		//Add Cloud Dinary
		public static void AddCloudDinary(this IServiceCollection services, IConfiguration configuration)
		{
			var cloudinaryUrl = configuration["CloudinaryUrl"];
			var cloudinaryAccount = new CloudinaryDotNet.Account(cloudinaryUrl);
			services.AddSingleton(new Cloudinary(cloudinaryAccount));
		}
	}
}
