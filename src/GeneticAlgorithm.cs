using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tema3
{
	public static class GeneticAlgorithm
	{
        public static Random random = new();
        public static (int[] individ, double value) Run(Dictionary<int, (double x, double y)> Nodes, int maxT, int populationSize, double mutationProbability, double crossoverProbability, 
			BaseSelection selection, BaseMutation mutation, BaseCrossover crossover)
		{
			int t = 0;

			var population = GetRandomPopulation(populationSize, Nodes.Count);
			var eval = EvalCycle.EvaluatePopulation(Nodes, population);

			while (t < maxT)
			{
				population = selection.Select(population, populationSize, eval);
				mutation.MutatePopulation(population, mutationProbability);
				crossover.CrossoverPopulation(population, crossoverProbability);
				eval = EvalCycle.EvaluatePopulation(Nodes, population);
				++t;
			}

			long min = eval.Min();

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

	}


	public static class GeneticAlgorithmAdaptive
	{
		public static (int[] individ, long value) RunAdaptive(Dictionary<int, (double x, double y)> Nodes, int maxT, int populationSize, double crossoverProbability, double k1, double k2, BaseMutation mutation, BaseCrossover crossover)
		{
			int t = 0;

			List<double> mutationProb;
			var population = GeneticAlgorithm.GetRandomPopulation(populationSize, Nodes.Count);

			var eval = EvalCycle.EvaluatePopulation(Nodes, population);

			//for (int i = 0; i < population[0].length; i++)
			//{
			//	console.write(population[0][i] + " ");
			//}
			//console.writeline();
			//console.writeline(eval[0]);

			while (t < maxT)
			{
				(population, mutationProb) = Select(population, populationSize, eval, k1, k2);
				MutatePopulation(population, mutationProb, mutation);
				crossover.CrossoverPopulation(population, crossoverProbability);
				eval = EvalCycle.EvaluatePopulation(Nodes, population);
				++t;
			}

			long min = eval.Min();

			return (population[eval.ToList().IndexOf(min)], min);
		}

		public static void MutatePopulation(List<int[]> population, List<double> mutateProbability, BaseMutation mutation)
		{
			// Apelez Inversion Mutation. Pentru mutatia initiala stergi prefixul IV
			for (var i = 0; i < population.Count; i++)
				population[i] = mutation.MutateIndividual(population[i], mutateProbability[i]);
		}

		public static double CalculateMutationProb(double max, double average, double eval, double k1, double k2)
		{
			if (eval >= average)
				return Math.Min(k2 - 0.1, Math.Max(k1 * (max - eval) / (max - average), 0.008));
			return k2;
		}

		public static (List<int[]> population, List<double> mutationProb) Select(List<int[]> population, int populationSize, long[] eval, double k1, double k2)
		{
			double[] evalNorm = new double[population.Count];
			List<int[]> newPopulation = new List<int[]>(populationSize);
			List<double> mutationProb = new List<double>(populationSize);
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
			{
				newPopulation.Add(l[ii].individual);
				mutationProb.Add(CalculateMutationProb(maxNorm, avgNorm, l[ii].evalNorm, k1, k2));
			}

			for (; ii < populationSize; ii++)
			{
				double rand = GeneticAlgorithm.random.NextDouble();

				for (int j = 0; j < population.Count; j++)
					if (q[j] < rand && rand <= q[j + 1])
					{
						newPopulation.Add(population[j]);
						mutationProb.Add(CalculateMutationProb(maxNorm, avgNorm, evalNorm[j], k1, k2));
					}
			}

			return (newPopulation, mutationProb);
		}
	}
}
