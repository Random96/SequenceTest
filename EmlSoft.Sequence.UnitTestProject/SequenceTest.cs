using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EmlSoft.Sequence.UnitTestProject
{
	public class SequenceTest
	{
		private void GetNum(List<string> List )
		{
			using (var context = new SequenceModel())
			{
				// find sequence
				using (var rep = new SequenceRepository(context))
				{
					var sequence = rep.GetSequence(SequenceName);
					if (sequence == null)
						sequence = rep.CreateRepository(SequenceName, "ABC[ГОД][00000]-44", 1, 99999, 1, true);

					for (int i = 0; i < 1000; ++i)
						List.Add( sequence.NextValue() );
				}
			}
		}

		[Fact]
		public void SequenceTestMethd()
		{
			List<string> [] res = new List<string>[10];

			var thds = new Task[10];

			for( int i=0; i<10; ++i )
			{
				var q = new List<string>();
				res[i] = q;
				thds[i] = Task.Factory.StartNew( ()=>GetNum(q) );
			}

			Task.WaitAll(thds);

			var result = new HashSet<string>();

			for ( int i = 0; i <10; ++i )
			{
				foreach (var s in res[i])
					Assert.Equal<bool>(result.Add(s), true);
			}

		}

		const string SequenceName = "Test";
	}
}
