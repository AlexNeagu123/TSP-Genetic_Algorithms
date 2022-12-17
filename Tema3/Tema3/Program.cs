using Tema3;

namespace Tema3
{
	class Program
	{
		static void Main(string[] args)
		{
			var inputReader = new InputReader("InputFiles\\st70.tsp");
			inputReader.ReadInput();


			var best = GeneticAlgorithm.Run(inputReader.Nodes, 2000, 200, 0.1, 0.9);

			foreach (var item in best.individ)
			{
				Console.Write(item + " ");
			}
			Console.WriteLine();
			Console.WriteLine(best.value);

		}
	}
}