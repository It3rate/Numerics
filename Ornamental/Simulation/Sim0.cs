using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace Ornamental.Simulation
{
	public class Sim0
	{
		public Dictionary<Number[], Number> Input { get; } = new Dictionary<Number[], Number>();
		public Dictionary<Number[], Number> Brain { get; } = new Dictionary<Number[], Number>();
	}
}
