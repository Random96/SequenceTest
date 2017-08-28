using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmlSoft.Sequence

{
	/// <summary>
	/// repository of Sequence
	/// </summary>
	public class SequenceRepository : IDisposable
	{
		private ISequenceModel _context;

		/// <summary>
		///  IoC-like public constructor
		/// </summary>
		/// <param name="context"></param>
		public SequenceRepository(ISequenceModel context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public ISequence GetSequence( string Name )
		{
			CheckDispose();

			var ret = _context.Set<Data.Sequence>().AsNoTracking().FirstOrDefault(x => x.Name == Name);

			if (ret == null)
				return null;

			// in real project - we shud use AutoMapper
			return new Model.Sequence(_context, ret.Name, ret.Template, ret.Id);
		}

		public ISequence CreateRepository(string sequenceName, string template, int minValue, int? maxValue, int increment, bool isCicling)
		{
			var dat = new Data.Sequence()
			{
				Name = sequenceName,
				Template = template,
				MinValue = minValue,
				MaxValue = maxValue,
				Increment = increment,
				Cycling = isCicling,
				CurrentValue = minValue
			};

			_context.Set<Data.Sequence>().Add(dat);
			_context.SaveChanges();

			// in real project - we shud use AutoMapper
			return new Model.Sequence(_context, dat.Name, dat.Template, dat.Id);
		}

		#region IDisposable Support
		private bool disposedValue = false; // Для определения избыточных вызовов

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				_context = null;

				disposedValue = true;
			}
		}
		public void Dispose()
		{
			Dispose(true);
		}

		void CheckDispose()
		{
			if (disposedValue)
				throw new ObjectDisposedException(nameof(SequenceRepository));
		}
		#endregion
	}
}
