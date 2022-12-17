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
        static Random random = new();
        public static (int[] individ, double value) Run(Dictionary<int, (double x, double y)> Nodes, int maxT, int populationSize, double mutationProbability, double crossoverProbability)
		{
			int t = 0;

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

		public static (int[] individ, double value) RunAdaptive(Dictionary<int, (double x, double y)> Nodes, int maxT, int populationSize, double crossoverProbability, double k1, double k2)
		{
			int t = 0;

			List<double> mutationProb;
			var population = GetRandomPopulation(populationSize, Nodes.Count);

			var eval = EvalCycle.EvaluatePopulation(Nodes, population);

			while (t < maxT)
			{
				(population, mutationProb) = Select(population, populationSize, eval, k1, k2);
				MutatePopulation(population, mutationProb);
				CrossoverPopulation(population, crossoverProbability);
				eval = EvalCycle.EvaluatePopulation(Nodes, population);
				++t;
			}

			double min = eval.Min();

			return (population[eval.ToList().IndexOf(min)], min);
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
					permutation[i] = random.Next(1, permutation.Length - i);
				

			return IdentityPermutationDecode(permutation);
		}

		public static int[] IVMutateIndividual(int[] individual, double mutateProbability)
		{
			if (random.NextDouble() > mutateProbability)
				return individual;

            int len = individual.Length;
            int[] permutation = new int[len];

			int invLen = random.Next(2, len + 1);
			int pos = random.Next(0, len - invLen + 1);

			for (int i = 0; i < len; i++)
			{
				if (i < pos || i > pos + invLen - 1)
					permutation[i] = individual[i];
				else
					permutation[i] = individual[pos + invLen - 1 - (i - pos)];
			}
            return permutation;
            
		}

		public static void MutatePopulation(List<int[]> population, List<double> mutateProbability)
		{
            // Apelez Inversion Mutation. Pentru mutatia initiala stergi prefixul IV
            for (var i = 0; i < population.Count; i++)
				population[i] = IVMutateIndividual(population[i], mutateProbability[i]);
		}

		public static void MutatePopulation(List<int[]> population, double mutateProbability)
		{
            // Apelez Inversion Mutation. Pentru mutatia initiala stergi prefixul IV
            for (int i = 0; i < population.Count; i++)
				population[i] = IVMutateIndividual(population[i], mutateProbability);
		}

        //Metoda Initiala De Crossover Decenta Pentru Orice Cazuri
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

		//Metoda PMX De Crossover, buna pentru cazuri mari
		private static int[] PMXCrossoverParents((int[] p1, int[] p2) parents)
		{
			int len = parents.p1.Length;
			
            int[] descendant = new int[len];
			Array.Fill(descendant, 0);

            bool[] taken = new bool[len + 1];
            Array.Fill(taken, false);

            int[] map2 = new int[len + 1];
            
            for (int i = 0; i < len; i++)
				map2[parents.p2[i]] = i;

			int cut1 = random.Next(0, len - 1);
			int cut2 = random.Next(cut1 + 1, len);
		
			for (int i = cut1; i <= cut2; i++)
			{
				descendant[i] = parents.p1[i];
				taken[descendant[i]] = true;
			}

			for(int i = cut1; i <= cut2; ++i)
			{
				if (taken[parents.p2[i]])
					continue;

				int ind = map2[descendant[i]];
				while(ind >= cut1 && ind <= cut2)
					ind = map2[descendant[ind]];

				descendant[ind] = parents.p2[i];
			}

			for(int i = 0; i < len; ++i)
			{
				if (descendant[i] == 0)
					descendant[i] = parents.p2[i];
			}

            return descendant;
        }

        //Metoda ERX De Crossover, foarte buna pentru cazuri mici, imposibil de folosit pentru cazuri mari
        private static int[] ERXCrossoverParents((int[] p1, int[] p2) parents)
		{
			int len = parents.p1.Length;
			var indices = new (int pos1, int pos2)[len + 1];

			for (int i = 0; i < len; i++)
			{
				(int gene1, int gene2) = (parents.p1[i], parents.p2[i]);
				indices[gene1].pos1 = i;
				indices[gene2].pos2 = i;
			}

			var connectedCities = new List<List<int>>();

			for (int i = 1; i <= len + 1; ++i)
				connectedCities.Add(new List<int>());
			
			for (int i = 1; i <= len; i++)
			{
				(int pos1, int pos2) = (indices[i].pos1, indices[i].pos2);
				HashSet<int> neighbours = new HashSet<int>();

				neighbours.Add(pos1 == 0 ? parents.p1[len - 1] : parents.p1[pos1 - 1]);
				neighbours.Add(pos2 == 0 ? parents.p2[len - 1] : parents.p2[pos2 - 1]);
				neighbours.Add(pos1 == len - 1 ? parents.p1[0] : parents.p1[pos1 + 1]);
				neighbours.Add(pos2 == len - 1 ? parents.p2[0] : parents.p2[pos2 + 1]);

				foreach(var neighbour in neighbours)
					connectedCities[i].Add(neighbour);

			}

			(int size, int city) initCity = (2000000, -1);
			bool[] visited = new bool[len + 1];

			for (int i = 1; i <= len; ++i)
			{
				visited[i] = false;
				if (connectedCities[i].Count < initCity.size)
					initCity = (connectedCities[i].Count, i);
			}

			int curCity = initCity.city;
			visited[curCity] = true;

			List<int> offset = new List<int>() { curCity };
		
			while(offset.Count < len)
			{
				(int size, int city) nxtCity = (2000000, -1);

				foreach (var neig in connectedCities[curCity])
				{
					if (visited[neig])
						continue;

					int edges = 0;

					foreach(var neigEdges in connectedCities[neig])
					{
						if (!visited[neigEdges])
							edges++;
					}
					
					if(edges < nxtCity.size)
						nxtCity = (edges, neig);
				}
                
				if (nxtCity.city == -1)
				{
					for(int candidate = 1; candidate <= len; candidate++)
					{
						if (visited[candidate])
							continue;

						int edges = 0;
						foreach(var neigEdge in connectedCities[candidate])
						{
							if (!visited[neigEdge])
								edges++;
						}

						if (edges < nxtCity.size)
							nxtCity = (edges, candidate);
					}
				}

				if(nxtCity.city == -1)
					break;

				curCity = nxtCity.city;
				offset.Add(curCity);
				visited[curCity] = true;
								
			}

			return offset.ToArray();
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


				//Metoda PMX de crossover

				//var descendant = PMXCrossoverParents((list[i].chromosome, list[i + 1].chromosome));
				//population.Add(descendant);

				//Metoda ERX de crossover 

				var descendant = ERXCrossoverParents((list[i].chromosome, list[i + 1].chromosome));
				population.Add(descendant);

				//Metoda initiala de crossover, ar trebui de mutat chestiile cu identityPermutation in functie 
	
				//var ch1 = IdentityPermutation(list[i].chromosome);
				//var ch2 = IdentityPermutation(list[i + 1].chromosome);
				//var descendants = CrossoverParents((ch1, ch2));
				//population.Add(IdentityPermutationDecode(descendants.Item1));
				//population.Add(IdentityPermutationDecode(descendants.Item2));
			}
		}

		public static double CalculateMutationProb(double max, double average, double eval, double k1, double k2)
		{
			if (eval >= average)
				return Math.Max(k1 * (max - eval) / (max - average), 0.0008);
			return k2;
		}


		public static (List<int[]> population, List<double> mutationProb) Select(List<int[]> population, int populationSize, double[] eval, double k1, double k2)
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
				double rand = random.NextDouble();

				for (int j = 0; j < population.Count; j++)
					if (q[j] < rand && rand <= q[j + 1])
					{
						newPopulation.Add(population[j]);
						mutationProb.Add(CalculateMutationProb(maxNorm, avgNorm, evalNorm[j], k1, k2));
					}
			}

			return (newPopulation, mutationProb);
		}

		public static List<int[]> TournSelect(List<int[]> population, int populationSize, double[] eval)
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
