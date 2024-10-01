﻿using ConsultingKoiFish.DAL.Entities;
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
	public partial class ConsultingKoiFishContext : IdentityDbContext<ApplicationUser>
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
		public virtual DbSet<Zodiac> Zodiacs { get; set; }
		public virtual DbSet<UserZodiac> UserZodiacs { get; set; }
		public virtual DbSet<KoiCategory> KoiCategories { get; set; }
		public virtual DbSet<KoiBreed> KoiBreeds { get; set; }
		public virtual DbSet<KoiBreedZodiac> KoiBreedZodiacs { get; set; }
		public virtual DbSet<PondCategory> PondCategories { get; set; }
		public virtual DbSet<Pond> Ponds { get; set; }
		public virtual DbSet<PondZodiac> PondZodiacs { get; set; }
		public virtual DbSet<UserPond> UserPonds { get; set; }

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
			//SeedRoles(modelBuilder);

			modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "ApplicationUser"); });
			modelBuilder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable(name: "UserRoles"); });
			modelBuilder.Entity<IdentityRole>(entity => { entity.ToTable(name: "Role"); });
			modelBuilder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable(name: "UserClaim"); });
			modelBuilder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable(name: "UserLogin"); });
			modelBuilder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable(name: "UserToken"); });
			modelBuilder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable(name: "RoleClaim"); });

			modelBuilder.Entity<UserPond>(entity =>
			{
				entity.HasKey(up => up.Id);

				entity.Property(up => up.PondName)
					.IsRequired()
					.HasMaxLength(256);

				entity.Property(up => up.Description)
					.HasColumnType("nvarchar(max)");

				entity.Property(up => up.Image)
					.HasColumnType("nvarchar(max)");

				entity.Property(up => up.UserId)
					.HasColumnType("nvarchar(450)");

				entity.Property(up => up.ScoreDetail)
					.HasColumnType("nvarchar(max)");

				// Configure relationship with ApplicationUser
				entity.HasOne(up => up.User)
					.WithMany(u => u.UserPonds)
					.HasForeignKey(up => up.UserId)
					.OnDelete(DeleteBehavior.ClientSetNull);
			});

			modelBuilder.Entity<PondZodiac>(entity =>
			{
				entity.HasKey(pz => pz.Id);

				// Relationship with Pond
				entity.HasOne(pz => pz.Pond)
					.WithMany(p => p.PondZodiacs)
					.HasForeignKey(pz => pz.PondId)
					.OnDelete(DeleteBehavior.ClientSetNull);

				// Relationship with Zodiac
				entity.HasOne(pz => pz.Zodiac)
					.WithMany(z => z.PondZodiacs)
					.HasForeignKey(pz => pz.ZodiacId)
					.OnDelete(DeleteBehavior.ClientSetNull);
			});

			modelBuilder.Entity<Pond>(entity =>
			{
				entity.HasKey(p => p.Id);

				entity.Property(p => p.Name)
					.IsRequired()
					.HasMaxLength(100);

				entity.Property(p => p.Description)
					.HasColumnType("nvarchar(max)");

				entity.Property(p => p.Image)
					.HasColumnType("nvarchar(max)");

				// Configure relationship
				entity.HasOne(p => p.PondCategory)
					.WithMany(pc => pc.Ponds)
					.HasForeignKey(p => p.PondCategoryId)
					.OnDelete(DeleteBehavior.ClientSetNull);
			});

			modelBuilder.Entity<PondCategory>(entity =>
			{
				entity.HasKey(pc => pc.Id);

				entity.Property(pc => pc.Name)
					.IsRequired()
					.HasMaxLength(100);

				entity.Property(pc => pc.Description)
					.HasColumnType("nvarchar(max)");
			});

			modelBuilder.Entity<KoiBreedZodiac>(entity =>
			{
				entity.HasKey(kbz => kbz.Id);

				// Relationship with KoiBreed
				entity.HasOne(kbz => kbz.KoiBreed)
					.WithMany(kb => kb.KoiBreedZodiacs)
					.HasForeignKey(kbz => kbz.KoiBreedId)
					.OnDelete(DeleteBehavior.ClientSetNull);

				// Relationship with Zodiac
				entity.HasOne(kbz => kbz.Zodiac)
					.WithMany(z => z.KoiBreedZodiacs)
					.HasForeignKey(kbz => kbz.ZodiacId)
					.OnDelete(DeleteBehavior.ClientSetNull);
			});

			modelBuilder.Entity<KoiBreed>(entity =>
			{
				entity.HasKey(kb => kb.Id);

				entity.Property(kb => kb.Name)
					.IsRequired()
					.HasMaxLength(100);

				entity.Property(kb => kb.Colors)
					.HasMaxLength(150);

				entity.Property(kb => kb.Pattern)
					.HasMaxLength(50);

				entity.Property(kb => kb.Description)
					.HasColumnType("nvarchar(max)");

				entity.Property(kb => kb.Image)
					.HasColumnType("nvarchar(max)");

				// Configure relationship
				entity.HasOne(kb => kb.KoiCategory)
					.WithMany(kc => kc.KoiBreeds)
					.HasForeignKey(kb => kb.KoiCategoryId)
					.OnDelete(DeleteBehavior.ClientSetNull);
			});

			modelBuilder.Entity<KoiCategory>(entity =>
			{
				entity.HasKey(kc => kc.Id);

				entity.Property(kc => kc.Name)
					.IsRequired()
					.HasMaxLength(100);

				entity.Property(kc => kc.Description)
					.HasColumnType("nvarchar(max)");
			});

			modelBuilder.Entity<Zodiac>(entity =>
			{
				entity.HasKey(z => z.Id);
				entity.Property(z => z.ZodiacName)
						.IsRequired()
						.HasMaxLength(100);
			});

			modelBuilder.Entity<UserZodiac>(entity =>
			{
				entity.HasKey(uz => uz.Id);

				entity.HasOne(uz => uz.User)
					.WithOne(u => u.UserZodiac)
					.HasForeignKey<UserZodiac>(uz => uz.UserId)
					.OnDelete(DeleteBehavior.ClientSetNull);

				entity.HasOne(uz => uz.Zodiac)
					.WithMany(z => z.UserZodiacs)
					.HasForeignKey(uz => uz.ZodiacId)
					.OnDelete(DeleteBehavior.ClientSetNull);
			});

			modelBuilder.Entity<UserDetail>(entity =>
			{
				entity.ToTable(name: "UserDetails");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.UserId).HasMaxLength(450);
				entity.HasOne(ud => ud.User)
						.WithOne(u => u.UserDetail)
						.HasForeignKey<UserDetail>(ud => ud.UserId)
						.OnDelete(DeleteBehavior.ClientSetNull)
						.HasConstraintName("FK_UserDetail_ApplicationUser");
			});

			modelBuilder.Entity<RefreshToken>(entity =>
			{
				entity.ToTable(name: "RefreshTokens");
				entity.HasOne(r => r.User)
					  .WithOne()
					  .HasForeignKey<RefreshToken>(r => r.UserId)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_RefreshToken_ApplicationUser");
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
