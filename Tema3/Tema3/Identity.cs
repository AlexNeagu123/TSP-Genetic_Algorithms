using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema3
{
	public static class IdentityPermutation
	{
		public static int[] Encode(int[] individual)
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

		public static int[] Decode(int[] permutation)
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
	}
}
