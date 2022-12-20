using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Tema3.IdentityPermutation;
using static Tema3.GeneticAlgorithm;
using static Tema3.EvalCycle;

namespace Tema3
{
	public class SimulatedAnnealing
	{
		public static (int[] min_point, double min) Run(Dictionary<int, (double x, double y)> Nodes, int populationSize)
		{

			Random random = new Random();
			double T_Stop = 0.0000000001;
			double T = 501.982;
			int k = 0;


			(int[] individual, double minEval) minTuple;
			var population = GetRandomPopulation(populationSize, Nodes.Count);

			long[] evalPopulation = EvaluatePopulation(Nodes, population);
			
			minTuple.individual = population[evalPopulation.ToList().IndexOf(evalPopulation.Min())];
			minTuple.minEval = evalPopulation.Min();
			var curIdentity = Encode(minTuple.individual);

			int individLen = minTuple.individual.Length;

			do
			{
				int iterations = 0;
				var newIdentity = new int[individLen];
				curIdentity.CopyTo(newIdentity, 0);

				do
				{
                    int index = random.Next(0, newIdentity.Length);
                    int aux = newIdentity[index];
					int newVal = random.Next(1, newIdentity.Length - index);
                    
					newIdentity[index] = newVal;

                    int[] neighbour = Decode(newIdentity);
					var min_local = EvaluateCycle(Nodes, neighbour);

					if (min_local < minTuple.minEval)
					{
						minTuple.minEval = min_local;
						minTuple.individual = neighbour;
						curIdentity[index] = newVal;
					}
					else if (random.NextDouble() < Math.Exp(-Math.Abs(min_local - minTuple.minEval) / T))
					{
						minTuple.minEval = min_local;
						minTuple.individual = neighbour;
						curIdentity[index] = newVal;
					}
					else
					{
						newIdentity[index] = aux;
					}
					iterations++;

				} while (iterations < 1000);

				++k;
				T *= 0.98;

			} while (T > T_Stop);

			return minTuple;

		}
	}
}
