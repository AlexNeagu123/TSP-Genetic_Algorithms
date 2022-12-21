using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tema3.IdentityPermutation;
using static Tema3.GeneticAlgorithm;
using static Tema3.EvalCycle;
using static Tema3.GeneticAlgorithmAdaptive;
using Microsoft.VisualBasic;

namespace Tema3
{
	//TO DO
	public static class HillClimbing
	{
		public static (int[] individ, double value) Run(Dictionary<int, (double x, double y)> Nodes, int populationSize, int iterations)
		{
			Random random = new Random();

			(int[] individual, double minEval) minTuple;
			var population = GetRandomPopulation(populationSize, Nodes.Count);

			long[] evalPopulation = EvaluatePopulation(Nodes, population);
			minTuple.individual = population[evalPopulation.ToList().IndexOf(evalPopulation.Min())];
			minTuple.minEval = evalPopulation.Min();


			for (var t = 0; t < iterations; t++)
			{
				bool local = false;
				(int[] individual, double minEval) minTuple_loc = minTuple;

				do
				{
					(int[] individual, double minEval) minNeighbor;
					minNeighbor.individual = FirstImprovement(Nodes, minTuple_loc.individual);
					minNeighbor.minEval = EvaluateCycle(Nodes, minNeighbor.individual);


					if (minTuple_loc.minEval > minNeighbor.minEval)
						minTuple_loc = minNeighbor;
					else
						local = true;

				} while (!local);


				if (minTuple.minEval > minTuple_loc.minEval)
					minTuple = minTuple_loc;
			}

			return minTuple;

		}


		public static (int[] individ, double value) RunHillAfterAG(Dictionary<int, (double x, double y)> Nodes, (int iterations, int maxT, int populationSize, double crossoverProbability, double k1, double k2, BaseMutation mutation, BaseCrossover crossover) Genetic)
		{

			(int[] individual, double minEval) minTuple = RunAdaptive(Nodes, Genetic.maxT, Genetic.populationSize, Genetic.crossoverProbability, Genetic.k1, Genetic.k2, Genetic.mutation, Genetic.crossover);


			bool local = false;
			(int[] individual, double minEval) minTuple_loc = minTuple;

			do
			{
				(int[] individual, double minEval) minNeighbor;
				minNeighbor.individual = FirstImprovement(Nodes, minTuple_loc.individual);
				minNeighbor.minEval = EvaluateCycle(Nodes, minNeighbor.individual);


				if (minTuple_loc.minEval > minNeighbor.minEval)
					minTuple_loc = minNeighbor;
				else
					local = true;

			} while (!local);


			if (minTuple.minEval > minTuple_loc.minEval)
				minTuple = minTuple_loc;


			return minTuple;

		}
		public static int[] FirstImprovement(Dictionary<int, (double x, double y)> Nodes, int[] individual)
		{
			int[] permutation = Encode(individual);

			for (int i = 0; i < 1000; i++)
			{
				int index = random.Next(0, permutation.Length);
				int aux = permutation[index];
				permutation[index] = random.Next(1, permutation.Length - index);
				int[] neighbour = Decode(permutation);
				permutation[index] = aux;
				if (EvaluateCycle(Nodes, neighbour) < EvaluateCycle(Nodes, individual))
					return neighbour;
			}

			return Decode(permutation);
		}
	}
}
