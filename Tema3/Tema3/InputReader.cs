using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		}

		public void ReadInput()
		{
			string line;

			using (StreamReader file = new System.IO.StreamReader(FilePath))
			{
				bool start = false;
				while ((line = file.ReadLine()) != null)
				{
					if (line.Contains("EOF"))
						break;


					if (start)
					{
						var node = line.Trim().Split(' ');
						Nodes.Add(int.Parse(node[0]), (double.Parse(node[1]), double.Parse(node[2])));
					}
					else
					{
						if (line == "NODE_COORD_SECTION")
						{
							start = true;
						}
					}
				}
			}
		}
	}
}
