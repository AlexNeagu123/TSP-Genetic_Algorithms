using Tema3;

namespace Tema3
{
	class Program
	{
		static void Main(string[] args)
		{
			var inputReader = new InputReader("InputFiles\\st70.tsp");
			inputReader.ReadInput();

			////var best = GeneticAlgorithm.Run(inputReader.Nodes, 2000, 200, 0.01, 0.90);
			//BaseSelection selection = new RouletteSelection();
			//BaseCrossover crossover = new PMXCrossover();
			//BaseMutation mutation = new IVMutation();
			//var best = GeneticAlgorithm.Run(inputReader.Nodes, 2000, 200, 0.01, 0.90, selection, mutation, crossover);

			//foreach (var item in best.individ)
			//{
			//	Console.Write(item + " ");
			//}

			//Console.WriteLine();
			//Console.WriteLine(best.value);


			////Check If The Permutation Obtained Is Valid

			//bool[] taken = new bool[best.individ.Length + 1];

			//bool good = true;

			//         foreach (var item in best.individ)
			//         {
			//	if (taken[item])
			//		good = false;

			//	taken[item] = true;
			//         }

			//if(good)
			//	Console.WriteLine("Permutation Is Valid");
			//else
			//	Console.WriteLine("Permutation Is Invalid");


			var best = HillClimbing.Run(inputReader.Nodes, 200, 2000);

			foreach (var item in best.individ)
			{
				Console.Write(item + " ");
			}

			Console.WriteLine();

			Console.WriteLine(best.value);
		}
	}
}