using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmlSoft.Sequence.Model
{
	public class Sequence : IDisposable, ISequence
	{
		private ISequenceModel _context;
		private static object _locker = new object();
		private int _curent;
		private int _id;
		private string _template;

		/// <summary>
		/// Класс счетчик непосредственно
		/// </summary>
		/// <param name="context"></param>
		public Sequence(ISequenceModel context, string template, int id, int curent)
		{
			_id = id;
			_curent = curent;
			_template = template;
			_context = context ?? throw new ArgumentNullException( nameof(context));
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

		// Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
		public void Dispose()
		{
			Dispose(true);
		}


		void CheckDispose()
		{
			if (disposedValue)
				throw new ObjectDisposedException(nameof(Sequence));
		}

		#endregion

		private string DoVerb( string Verb )
		{
			switch(Verb)
			{
				case "[ГОД]":
					return DateTime.Now.Year.ToString("0000");

				default:
					return Verb;
			}
		}

		public override string ToString()
		{
			if( string.IsNullOrEmpty(_template) )
				return Current.ToString();

			var ret = string.Copy(_template);

			var verbs = Regex.Matches(_template, @"\[.*?\]");

			foreach (Match verb in verbs)
			{
				int length = verb.Value.Length;
				if (length > 2)
				{
					string temp1 = string.Empty.PadRight(length - 2, '0');
					if (verb.Value.Substring(1, length - 2) == temp1)
					{
						ret = ret.Replace(verb.ToString(), _curent.ToString(temp1));
						continue;
					}
				}
				ret = ret.Replace(verb.ToString(), DoVerb(verb.ToString()));
			}

			return ret;
		}

		public int Current
		{
			get => _curent;
			private set { _curent = value; }
		}

		public string CurrentValue()
		{
			CheckDispose();

			return ToString();
		}

		public string NextValue()
		{
			CheckDispose();

			lock(_locker)
			{
				var dat = _context.Set<Data.Sequence>().First(x => x.Id == _id);
				int NextVal = dat.CurrentValue + dat.Increment;
				if (dat.MaxValue != null)
				{
					if (NextVal > dat.MaxValue)
					{
						if (!dat.Cycling)
							throw new Exception("Max value");

						NextVal = dat.MinValue + NextVal - ( dat.MaxValue ?? 0) - 1;
					}
				}

				dat.CurrentValue = NextVal;
				_context.SaveChanges();

				Current = NextVal;
			}

			return CurrentValue();
		}
	}
}
