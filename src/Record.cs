using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema3
{
	public class Record
	{
		public string FileName { get; set; }
		public double AvgValue { get; set; }
		public double SdValue { get; set; }
		public double BestValue { get; set; }
		public double AvgTime { get; set; }
		public double K1Prob { get; set; }
		public double CrossProb { get; set; }
		public string MutType { get; set; }
		public string CrossType { get; set; }
	}
}
