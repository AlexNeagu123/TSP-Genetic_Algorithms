using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema3
{
	public static class EvalCycle
	{
		public static double EvaluateCycle(Dictionary<int, (double x, double y)> Nodes, int[] permutation)
		{
			double cost = 0;

			for (int i = 0; i < permutation.Length - 1; i++)
			{
				cost += ComputeDistance(Nodes[permutation[i]], Nodes[permutation[i+1]]);
			}

			cost += ComputeDistance(Nodes[permutation[^1]], Nodes[permutation[0]]);

			return cost;
		}

		public static double ComputeDistance((double x, double y) node1, (double x, double y) node2)
		{
			return Math.Sqrt(Math.Pow(node1.x - node2.x, 2) + Math.Pow(node1.y - node2.y, 2));
		}
	}
}
