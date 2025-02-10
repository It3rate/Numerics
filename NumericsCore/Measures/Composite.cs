using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Motions;

namespace NumericsCore.Measures;

public class Composite
{
	Focal[] Focals { get; set; }

	public Composite(params Focal[] focals)
	{
		Focals = focals;
	}

	public static (double, double) Ratio(Focal value, Focal unit, BitSymmetry bits)
	{
		return (unit.StartTick == 0 ? 0 : value.StartTick / (double)unit.StartTick,
				unit.EndTick == 0 ? 0 : value.EndTick / (double)unit.EndTick);
	}
	public static double Length(long start, long end) => end - start;
}
