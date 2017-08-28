using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EmlSoft.Sequence.Model
{
	public class Sequence : IDisposable, ISequence
	{
		private ISequenceModel _context;
		private static object _dictMutex = new Object();

		private static Dictionary<string, object> _locker = new Dictionary<string, object>();
		private static Dictionary<string, int> _curent = new Dictionary<string, int>();
		private int _id;
		private string _template;
		private string _name;

		/// <summary>
		/// Класс счетчик непосредственно
		/// </summary>
		/// <param name="context"></param>
		public Sequence(ISequenceModel context, string name, string template, int id)
		{
			_id = id;
			_template = template;
			_name = name ?? throw new ArgumentNullException(nameof(name));
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
						ret = ret.Replace(verb.ToString(), Current.ToString(temp1));
						continue;
					}
				}
				ret = ret.Replace(verb.ToString(), DoVerb(verb.ToString()));
			}

			return ret;
		}

		private int GetCurent()
		{
			if (_curent.TryGetValue(_name, out int ret))
				return ret;

			lock ( _dictMutex )
			{
				var db = _context.Set<Data.Sequence>().AsNoTracking().First(x => x.Id == _id);
				_curent.Add(_name, db.CurrentValue);
				return db.CurrentValue;
			}
		}

		private void SetCurent(int Value)
		{
			lock (_dictMutex)
			{
				_curent[_name] = Value;
			}
		}

		public int Current
		{
			get => GetCurent();

			private set => SetCurent(value);
		}

		public string CurrentValue()
		{
			CheckDispose();

			return ToString();
		}

		public string NextValue()
		{
			CheckDispose();

			object lockObject = null;
			lock(_dictMutex)
			{				
				if (!_locker.TryGetValue(_name, out lockObject))
				{
					lockObject = new Mutex();
					_locker.Add(_name, lockObject);
				}
			}

			lock (lockObject)
			{
				var dat = _context.Set<Data.Sequence>().First(x => x.Id == _id);
				int NextVal = Current + dat.Increment;
				if (dat.MaxValue != null)
				{
					if (NextVal > dat.MaxValue)
					{
						if (!dat.Cycling)
							throw new Exception("Max value");

						NextVal = dat.MinValue + NextVal - (dat.MaxValue ?? 0) - 1;
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
