using ConsultingKoiFish.BLL.Helpers.Mapper;
using ConsultingKoiFish.DAL.Repositories;
using ConsultingKoiFish.DAL.UnitOfWork;
using System.Reflection;

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

		//add auto mapper
		public static void AddMapper(this IServiceCollection services)
		{
			services.AddAutoMapper(typeof(MappingProfile));
		}
	}
}
