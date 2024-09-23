using ConsultingKoiFish.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL
{
	public class ConsultingKoiFishContext : IdentityDbContext<IdentityUser>
	{
        public ConsultingKoiFishContext(DbContextOptions<ConsultingKoiFishContext> options) : base(options)
        {
            
        }

		#region DbSet

		public virtual DbSet<UserDetail> UserDetails { get; set; }

		#endregion


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
				IConfigurationRoot configuration = builder.Build();
				optionsBuilder.UseSqlServer(configuration.GetConnectionString("OnDemandTutor"));
			}
		}
	}
}
