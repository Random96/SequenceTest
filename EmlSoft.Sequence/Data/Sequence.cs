using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EmlSoft.Sequence.Data
{
    public class Sequence 
	{
		public int Id { get; set; }

		[MaxLength(50)]
		public string Name { get; set; }

		[MaxLength(250)]
		public string Template { get; set; }

		public int MinValue { get; set; }

		public int ? MaxValue { get; set; }

		public int Increment { get; set; } = 1;

		public bool Cycling { get; set; }

		public int CurrentValue { get; set; }
	}
}
