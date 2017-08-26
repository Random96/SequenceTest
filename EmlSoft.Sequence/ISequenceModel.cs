using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmlSoft.Sequence
{
	/// <summary>
	/// Database model interface
	/// </summary>
	public interface ISequenceModel
	{
		DbSet<T> Set<T>() where T : class;

		Task<int> SaveChangesAsync();

		int SaveChanges();
	}
}
