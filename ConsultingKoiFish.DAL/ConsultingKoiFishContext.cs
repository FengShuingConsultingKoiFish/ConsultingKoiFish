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
	public partial class ConsultingKoiFishContext : IdentityDbContext<IdentityUser>
	{
		public ConsultingKoiFishContext()
		{

		}
		public ConsultingKoiFishContext(DbContextOptions<ConsultingKoiFishContext> options) : base(options)
		{

		}

		#region DbSet

		public virtual DbSet<UserDetail> UserDetails { get; set; }
		public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

		#endregion


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
				IConfigurationRoot configuration = builder.Build();
				optionsBuilder.UseSqlServer(configuration.GetConnectionString("ConsultingKoiFish"));
			}
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.HasDefaultSchema("dbo");
			SeedRoles(modelBuilder);

			modelBuilder.Entity<IdentityUser>(entity => { entity.ToTable(name: "User"); });
			modelBuilder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable(name: "UserRoles"); });
			modelBuilder.Entity<IdentityRole>(entity => { entity.ToTable(name: "Role"); });
			modelBuilder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable(name: "UserClaim"); });
			modelBuilder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable(name: "UserLogin"); });
			modelBuilder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable(name: "UserToken"); });
			modelBuilder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable(name: "RoleClaim"); });

			modelBuilder.Entity<UserDetail>()
			.HasOne(ud => ud.User)
			.WithOne() // Không cần truy xuất ngược từ User về UserDetail
			.HasForeignKey<UserDetail>(ud => ud.UserId)
			.OnDelete(DeleteBehavior.ClientSetNull)
			.HasConstraintName("FK_UserDetail_User");

			modelBuilder.Entity<RefreshToken>(entity =>
			{
				entity.ToTable(name: "RefreshTokens");
				entity.HasOne(r => r.User)
					  .WithOne()
					  .HasForeignKey<RefreshToken>(r => r.UserId)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_RefreshToken_User");
				entity.Property(x => x.UserId).HasMaxLength(450);
			});

			OnModelCreatingPartial(modelBuilder);
		}
		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

		private static void SeedRoles(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<IdentityRole>().HasData
				(
					new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "ADMIN" },
					new IdentityRole() { Name = "Member", ConcurrencyStamp = "2", NormalizedName = "MEMBER" }
				);
		}
	}

}
