using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tema3.GeneticAlgorithm;

namespace Tema3
{
	public abstract class BaseSelection
	{
		public abstract List<int[]> Select(List<int[]> population, int populationSize, long[] eval);
	}

	public class RouletteSelection : BaseSelection
	{
		public override List<int[]> Select(List<int[]> population, int populationSize, long[] eval)
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

	public class TournSelection : BaseSelection
	{
		public override List<int[]> Select(List<int[]> population, int populationSize, long[] eval)
		{
			List<int[]> newPopulation = new List<int[]>(populationSize);
			bool[] taken = new bool[population.Count];

			for (int i = 0; i < populationSize; i++)
			{
				for (int j = 0; j < population.Count; j++)
					taken[j] = false;

				var candidates = new List<(double cost, int ind)>(8);

				for (int step = 0; step < 8; step++)
				{
					int rand = random.Next(0, populationSize);

					while (taken[rand])
						rand = random.Next(0, populationSize);

					candidates.Add((eval[rand], rand));
					taken[rand] = true;
				}

				candidates.Sort((cand1, cand2) => cand1.cost.CompareTo(cand2.cost));
				newPopulation.Add(population[candidates[0].ind]);
			}

			return newPopulation;
		}
	}
}
