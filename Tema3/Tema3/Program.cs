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


			var population = GeneticAlgorithm.GetRandomPopulation(10, inputReader.Nodes.Count);

			foreach (var individ in population)
			{
				foreach (var gene in individ)
				{
					Console.Write(gene + " ");
				}
				Console.WriteLine("\n");

				Console.WriteLine(EvalCycle.EvaluateCycle(inputReader.Nodes, individ));
			}


		}
	}
}