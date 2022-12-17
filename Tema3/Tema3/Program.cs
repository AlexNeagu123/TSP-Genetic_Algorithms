using Tema3;

namespace Tema3
{
	class Program
	{
		static void Main(string[] args)
		{
			var inputReader = new InputReader("InputFiles\\eil51.tsp");
			inputReader.ReadInput();


			//var best = GeneticAlgorithm.Run(inputReader.Nodes, 2000, 200, 0.01, 0.90);
            var best = GeneticAlgorithm.RunAdaptive(inputReader.Nodes, 2000, 200, 0.90, 0.03, 0.1);

            foreach (var item in best.individ)
			{
				Console.Write(item + " ");
			}
			Console.WriteLine();
			Console.WriteLine(best.value);

		}
	}
}