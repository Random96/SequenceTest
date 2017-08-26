using System;
using System.Linq;
using Xunit;

namespace EmlSoft.Sequence.UnitTestProject
{
	public class SequenceTest
	{
		[Fact]
		public void SequenceTestMethd()
		{
			using (var context = new SequenceModel())
			{
				// find sequence
				using (var rep = new SequenceRepository(context))
				{
					var sequence = rep.GetSequence(SequenceName);
					if (sequence == null)
						sequence = rep.CreateRepository(SequenceName, "ABC[ГОД][00000]-44", 1, 99999, 1, true);

					var sec1 = sequence.CurrentValue();

					var sec2 = sequence.NextValue();
				}
			}
		}

		const string SequenceName = "Test";
	}
}
