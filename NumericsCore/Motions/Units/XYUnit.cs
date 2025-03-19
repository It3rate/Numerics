using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Motions.Units;

public class XYUnit : IUnit
{
	public string Name => "XY";
	public int SubUnitCount => 2;
	public IUnit[] SubUnits => new[] { XUnit, YUnit };
	public Focal Limits { get; private set; }

	public IUnit XUnit { get; } = new DefaultUnit("X", new Focal(-10000000, 10000000));
	public IUnit YUnit { get; } = new DefaultUnit("Y", new Focal(-10000000, 10000000));
	public Focal XDenominator { get; private set; }
	public Focal YDenominator { get; private set; }

	public XYUnit()
	{
		XDenominator = new Focal(-1000, 1000);
		YDenominator = XDenominator.Clone();
		Limits = XDenominator.Clone();
	}
}
