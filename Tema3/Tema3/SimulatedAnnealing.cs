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
		//public static (int[] min_point, double min) Run(Dictionary<int, (double x, double y)> Nodes, int populationSize)
		//{
		//	Random random = new Random();
		//	double T_Stop = 0.0000000001;
		//	double T = 501.982;
		//	double T0 = T;
		//	int k = 0;


		//	(int[] individual, double minEval) minTuple;
		//	var population = GetRandomPopulation(populationSize, Nodes.Count);

		//	double[] evalPopulation = EvaluatePopulation(Nodes, population);
		//	minTuple.individual = population[evalPopulation.ToList().IndexOf(evalPopulation.Min())];
		//	minTuple.minEval = evalPopulation.Min();

		//	do
		//	{
		//		var XX_bits = new bool[X_bits.Length];
		//		X_bits.CopyTo(XX_bits, 0);

		//		int iterations = 0;

		//		do
		//		{
		//			int i = random.Next(X_bits.Length);

		//			XX_bits[i] = !XX_bits[i];

		//			var min_point_local = Conversion.FromBitsToDouble(XX_bits, function.SearchDomain, dimensions);
		//			var min_local = function.EvaluateFunction(min_point_local);

		//			if (min_local < minTuple.min)
		//			{
		//				minTuple.min = min_local;
		//				minTuple.min_point = min_point_local;
		//				X_bits[i] = XX_bits[i];
		//			}
		//			else if (random.NextDouble() < Math.Exp(-Math.Abs(min_local - minTuple.min) / T))
		//			{
		//				minTuple.min = min_local;
		//				minTuple.min_point = min_point_local;
		//				X_bits[i] = XX_bits[i];
		//			}
		//			else
		//			{
		//				XX_bits[i] = !XX_bits[i];
		//			}

		//			iterations++;


		//		} while (iterations < 10000);

		//		++k;
		//		T = T0 * Math.Pow(0.98, k);

		//	} while (T > T_Stop);

		//	return minTuple;
		//}
	}
}
