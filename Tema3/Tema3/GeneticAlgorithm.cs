using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema3
{
	public static class GeneticAlgorithm
	{
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

			Random rnd = new();

			for (int i = 0; i < popSize; i++)
			{
				population[i] = population[i].OrderBy(x => rnd.Next()).ToArray();
			}

			return population;
		}

		public static void MutationPopulation(List<int[]> population)
		{

		}
	}
}
