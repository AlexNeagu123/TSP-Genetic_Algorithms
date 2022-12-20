using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tema3.GeneticAlgorithm;
using static Tema3.IdentityPermutation;

namespace Tema3
{
	public abstract class BaseMutation
	{
		public abstract int[] MutateIndividual(int[] individual, double mutateProbability);

		public void MutatePopulation(List<int[]> population, double mutateProbability)
		{
			// Apelez Inversion Mutation. Pentru mutatia initiala stergi prefixul IV
			for (int i = 0; i < population.Count; i++)
				population[i] = MutateIndividual(population[i], mutateProbability);
		}
	}

	public class NormalMutation : BaseMutation
	{
		public override int[] MutateIndividual(int[] individual, double mutateProbability)
		{
			int[] permutation = Encode(individual);

			for (int i = 0; i < individual.Length; i++)
				if (random.NextDouble() <= mutateProbability)
					permutation[i] = random.Next(1, permutation.Length - i);


			return Decode(permutation);
		}

		public override string ToString()
		{
			return "Normal Mutation";
		}
	}

	public class IVMutation : BaseMutation
	{
		public override int[] MutateIndividual(int[] individual, double mutateProbability)
		{
			if (random.NextDouble() > mutateProbability)
				return individual;

			int len = individual.Length;
			int[] rem = new int[len];

			int invLen = random.Next(2, len + 1);
			int pos = random.Next(0, len - invLen + 1);
			int newLen = 0;

			for (int i = 0; i < len; i++)
			{
				if (i < pos || i > pos + invLen - 1)
					rem[newLen++] = individual[i];
			}

			int insertPos = random.Next(Math.Max(0, pos - 1), Math.Min(pos + 2, newLen + 1));
            int finalLength = 0;

			int[] permutation = new int[len];
			
			for(int i = 0; i <= newLen; i++)
			{
				if(i == insertPos)
				{
					for(int j = 0; j < invLen; j++)
						permutation[finalLength++] = individual[pos + invLen - 1 - j];
				} 
				if(i != newLen)
					permutation[finalLength++] = rem[i];	
			}

			return permutation;
		}

		public override string ToString()
		{
			return "IV Mutation";
		}
	}
}
