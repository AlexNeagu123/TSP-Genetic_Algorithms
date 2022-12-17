using Tema3;

namespace Tema3
{
	class Program
	{
		static void Main(string[] args)
		{
			var inputReader = new InputReader("InputFiles\\st70.tsp");
			inputReader.ReadInput();


			var best = GeneticAlgorithm.RunAdaptive(inputReader.Nodes, 2000, 200, 1, 0.058, 0.1);

			foreach (var item in best.individ)
			{
				Console.Write(item + " ");
			}
			Console.WriteLine();
			Console.WriteLine(best.value);

		}
	}
}