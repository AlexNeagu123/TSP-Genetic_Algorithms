using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tema3
{
	public static class GeneticAlgorithm
	{
		static Random random = new();


		public static (int[] individ, double value) Run(Dictionary<int, (double x, double y)> Nodes, int maxT, int populationSize, double mutationProbability, double crossoverProbability)
		{
			int t = 0;

			List<double> mutationProb;
			var population = GetRandomPopulation(populationSize, Nodes.Count);

			var eval = EvalCycle.EvaluatePopulation(Nodes, population);

			while (t < maxT)
			{
				population = Select(population, populationSize, eval);
				MutatePopulation(population, mutationProbability);
				CrossoverPopulation(population, crossoverProbability);
				eval = EvalCycle.EvaluatePopulation(Nodes, population);
				++t;
			}

			double min = eval.Min();

			return (population[eval.ToList().IndexOf(min)] ,min);
		}
		
		public static List<int[]> GetRandomPopulation(int popSize, int individSize)
		{
			var population = new List<int[]>();

			for (int i = 0; i < popSize; i++)
			{
				population.Add(new int[individSize]);
				for (int j = 0; j < individSize; j++)
				{
					population[i][j] = j + 1;
				}
			}


			for (int i = 0; i < popSize; i++)
			{
				population[i] = population[i].OrderBy(x => random.Next()).ToArray();
			}

			return population;
		}

		public static int[] IdentityPermutation(int[] individual)
		{
			List<int> identity = new(individual.Length);
			int[] permutation = new int[individual.Length];

			for (int i = 0; i < individual.Length; i++)
			{
				identity.Add(i + 1);
			}

			for (int i = 0; i < individual.Length; i++)
			{
				permutation[i] = identity.IndexOf(individual[i]) + 1;
				identity.Remove(individual[i]);
			}

			return permutation;
		}

		public static int[] IdentityPermutationDecode(int[] permutation)
		{
			List<int> identity = new(permutation.Length);
			int[] individual = new int[permutation.Length];

			for (int i = 0; i < permutation.Length; i++)
			{
				identity.Add(i + 1);
			}

			for (int i = 0; i < permutation.Length; i++)
			{
				individual[i] = identity[permutation[i] - 1];
				identity.RemoveAt(permutation[i] - 1);
			}

			return individual;
		}

		public static int[] MutateIndividual(int[] individual, double mutateProbability)
		{
			int[] permutation = IdentityPermutation(individual);

			for (int i = 0; i < individual.Length; i++)
				if (random.NextDouble() <= mutateProbability)
				{
					int mut = random.Next(1, permutation.Length - i);
					permutation[i] = mut;
				}

			return IdentityPermutationDecode(permutation);
		}

		public static void MutatePopulation(List<int[]> population, double mutateProbability)
		{
			for (int i = 0; i < population.Count; i++)
			{
				population[i] = MutateIndividual(population[i], mutateProbability);
			}
		}

		private static (int[], int[]) CrossoverParents((int[] p1, int[] p2) parents)
		{
			(int[] d1, int[] d2) descendants = (new int[parents.p1.Length], new int[parents.p2.Length]);
			parents.p1.CopyTo(descendants.d1, 0);
			parents.p2.CopyTo(descendants.d2, 0);

			int position = 1 + random.Next(0, parents.p1.Length - 3);

			for (int i = position; i < parents.p1.Length; ++i)
			{
				descendants.d1[i] = parents.p2[i];
				descendants.d2[i] = parents.p1[i];
			}

			return descendants;
		}

		public static void CrossoverPopulation(List<int[]> population, double crossoverProb)
		{
			var list = new List<(int[] chromosome, double prob)>(population.Count);

			for (int i = 0; i < population.Count; ++i)
				list.Add((population[i], random.NextDouble()));

			list.Sort((a, b) => a.prob.CompareTo(b.prob));


			for (int i = 0; i < (list.Count - 1); i += 2)
			{
				if (list[i].prob >= crossoverProb)
					break;

				if (list[i + 1].prob >= crossoverProb)
					if (random.NextDouble() >= 0.5)
						break;

				var ch1 = IdentityPermutation(list[i].chromosome);
				var ch2 = IdentityPermutation(list[i + 1].chromosome);

				var descendants = CrossoverParents((ch1, ch2));
				population.Add(IdentityPermutationDecode(descendants.Item1));
				population.Add(IdentityPermutationDecode(descendants.Item2));
			}
		}


		public static List<int[]> Select(List<int[]> population, int populationSize, double[] eval)
		{
			double[] evalNorm = new double[population.Count];
			List<int[]> newPopulation = new List<int[]>(populationSize);
			double[] p = new double[population.Count];
			double[] q = new double[population.Count + 1];


			double max = eval.Max();
			double min = eval.Min();

			for (int i = 0; i < population.Count; i++)
				evalNorm[i] = (max - eval[i]) / (max - min + 0.000001) + 0.01;


			double T = 0;
			for (int i = 0; i < population.Count; i++)
				T += evalNorm[i];


			for (int i = 0; i < population.Count; i++)
				p[i] = evalNorm[i] / T;

			q[0] = 0;
			for (int i = 0; i < population.Count; i++)
				q[i + 1] = q[i] + p[i];

			q[^1] = 1;


			double maxNorm = evalNorm.Max();
			double avgNorm = evalNorm.Average();


			var l = new List<(int[] individual, double evalNorm)>(population.Count);
			for (int i = 0; i < population.Count; i++)
				l.Add((population[i], evalNorm[i]));

			l = l.OrderByDescending(x => x.evalNorm).ToList();

			int ii = 0;
			for (ii = 0; ii < (populationSize * 5) / 100; ii++)
				newPopulation.Add(l[ii].individual);


			for (; ii < populationSize; ii++)
			{
				double rand = random.NextDouble();

				for (int j = 0; j < population.Count; j++)
					if (q[j] < rand && rand <= q[j + 1])
						newPopulation.Add(population[j]);
			}

			return newPopulation;
		}
	}
}
