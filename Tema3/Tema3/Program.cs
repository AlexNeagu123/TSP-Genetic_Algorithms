using Tema3;

namespace Tema3
{
	class Program
	{
		static void Main(string[] args)
		{
			var inputReader = new InputReader("InputFiles\\st70.tsp");
			inputReader.ReadInput();

			foreach (var node in inputReader.Nodes)
			{
				Console.WriteLine(node);
			}

			
		}
	}
}