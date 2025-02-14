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
	public Focal Denominator { get; private set; }

	public IUnit XUnit { get; }
	public IUnit YUnit { get; }
	public Focal XDenominator { get; private set; }
	public Focal YDenominator { get; private set; }

	public XYUnit()
	{
		XDenominator = new Focal(-1000, 1000);
		YDenominator = XDenominator.Clone();
		Denominator = XDenominator.Clone();
	}

	public SymmetricNumber CreateNumber(Focal numerator, Focal? denominator = null)
	{
		return new SymmetricNumber(this, numerator, denominator);
	}
}
