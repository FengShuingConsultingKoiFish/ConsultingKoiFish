using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ConsultingKoiFish.DAL.Entities;

namespace ConsultingKoiFish.DAL
{
	internal class ConsultingKoiFishContextFactory : IDesignTimeDbContextFactory<ConsultingKoiFishContext>
	{
		public ConsultingKoiFishContext CreateDbContext(string[] args)
		{
			var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../ConsultingKoiFish.API");
			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(basePath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			var builder = new DbContextOptionsBuilder<ConsultingKoiFishContext>();
			var connectionString = configuration.GetConnectionString("ConsultingKoiFish");

			builder.UseSqlServer(connectionString);

			// Use the constructor without UserManager and RoleManager
			return new ConsultingKoiFishContext(builder.Options);
		}
	}


}
