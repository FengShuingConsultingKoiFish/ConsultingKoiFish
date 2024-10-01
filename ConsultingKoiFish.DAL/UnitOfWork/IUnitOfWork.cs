using ConsultingKoiFish.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.UnitOfWork
{
	public interface IUnitOfWork : IDisposable
	{
		IRepoBase<T> GetRepo<T>() where T : class;
		Task SaveChangesAsync();
		Task BeginTransactionAsync();
		Task CommitTransactionAsync();
		Task RollBackAsync();
		Task<bool> SaveAsync();
	}
}
