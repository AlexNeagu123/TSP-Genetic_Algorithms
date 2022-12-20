using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Tema3
{
	public class InputReader
	{
		public string FilePath { get; private init; }
		public Dictionary<int, (double x, double y)> Nodes { get; private init; }

		public InputReader(string filePath)
		{
			FilePath = filePath;
			Nodes = new();

			string line;

			using StreamReader file = new System.IO.StreamReader(FilePath);
			
			while ((line = file.ReadLine().Trim()) != null)
			{
				if (line.Contains("EOF"))
					break;

				line = line.Trim();
				line = Regex.Replace(line, @"\s+", " ");

				var node = line.Split(' ');
				
				if (node.Length < 3)
					continue;

				NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

				//Console.WriteLine((int.Parse(node[0]) + " " + (Double.Parse(node[1], NumberStyles.Any, nfi) + " " +
				//	Double.Parse(node[2], NumberStyles.Any, nfi))));

				Nodes.Add(int.Parse(node[0]), (Double.Parse(node[1], NumberStyles.Any, nfi), 
					Double.Parse(node[2], NumberStyles.Any, nfi)));
			
			}

			file.Close();
		}
	}
}
