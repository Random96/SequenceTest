using System;
using System.Data.Entity;
using System.Linq;

namespace EmlSoft.Sequence
{
	/// <summary>
	/// Database EF context
	/// </summary>
	public class SequenceModel : DbContext, ISequenceModel
	{		
		/// <summary>
		/// default Public constructor
		/// </summary>
		public SequenceModel()
			: base("name=SequenceModel")
		{
		}

		public virtual DbSet<Data.Sequence> Sequences { get; set; }
	}
}