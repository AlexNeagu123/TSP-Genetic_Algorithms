using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tema3.GeneticAlgorithm;
using static Tema3.IdentityPermutation;

namespace Tema3
{
	public abstract class BaseCrossover
	{
		public abstract void CrossoverPopulation(List<int[]> population, double crossoverProb);
	}

	public class NormalCrossover : BaseCrossover
	{
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
		public override void CrossoverPopulation(List<int[]> population, double crossoverProb)
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


				var ch1 = Encode(list[i].chromosome);
				var ch2 = Encode(list[i + 1].chromosome);
				var descendants = CrossoverParents((ch1, ch2));
				population.Add(Decode(descendants.Item1));
				population.Add(Decode(descendants.Item2));
			}
		}
	}

	public class PMXCrossover : BaseCrossover
	{
		private static int[] CrossoverParents((int[] p1, int[] p2) parents)
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

			for (int i = cut1; i <= cut2; ++i)
			{
				if (taken[parents.p2[i]])
					continue;

				int ind = map2[descendant[i]];
				while (ind >= cut1 && ind <= cut2)
					ind = map2[descendant[ind]];

				descendant[ind] = parents.p2[i];
			}

			for (int i = 0; i < len; ++i)
			{
				if (descendant[i] == 0)
					descendant[i] = parents.p2[i];
			}

			return descendant;
		}
		public override void CrossoverPopulation(List<int[]> population, double crossoverProb)
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
				var descendant = CrossoverParents((list[i].chromosome, list[i + 1].chromosome));
				population.Add(descendant);

			}
		}
	}

	public class ERXCrossover : BaseCrossover
	{
		private int[] CrossoverParents((int[] p1, int[] p2) parents)
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
				HashSet<int> neighbours = new()
				{
					pos1 == 0 ? parents.p1[len - 1] : parents.p1[pos1 - 1],
					pos2 == 0 ? parents.p2[len - 1] : parents.p2[pos2 - 1],
					pos1 == len - 1 ? parents.p1[0] : parents.p1[pos1 + 1],
					pos2 == len - 1 ? parents.p2[0] : parents.p2[pos2 + 1]
				};

				foreach (var neighbour in neighbours)
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

			while (offset.Count < len)
			{
				(int size, int city) nxtCity = (2000000, -1);

				foreach (var neig in connectedCities[curCity])
				{
					if (visited[neig])
						continue;

					int edges = 0;

					foreach (var neigEdges in connectedCities[neig])
					{
						if (!visited[neigEdges])
							edges++;
					}

					if (edges < nxtCity.size)
						nxtCity = (edges, neig);
				}

				if (nxtCity.city == -1)
				{
					for (int candidate = 1; candidate <= len; candidate++)
					{
						if (visited[candidate])
							continue;

						int edges = 0;
						foreach (var neigEdge in connectedCities[candidate])
						{
							if (!visited[neigEdge])
								edges++;
						}

						if (edges < nxtCity.size)
							nxtCity = (edges, candidate);
					}
				}

				if (nxtCity.city == -1)
					break;

				curCity = nxtCity.city;
				offset.Add(curCity);
				visited[curCity] = true;

			}

			return offset.ToArray();
		}
		public override void CrossoverPopulation(List<int[]> population, double crossoverProb)
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


				//Metoda ERX de crossover 
				var descendant = CrossoverParents((list[i].chromosome, list[i + 1].chromosome));
				population.Add(descendant);

			}
		}
	}
}