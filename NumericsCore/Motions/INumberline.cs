using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Motions.Units;

namespace NumericsCore.Motions;

// The conceptual primitive is a Property. This can be a simple symmetric number, or something more complex underneath.
// A path is a Property if you only care about start to end, but if you care about both X and Y it is two properties.
// Any branch is going to be two properties. Options or branches in a property are always defined by a joint and a new property.
// Properties can have multiple paths, but they will be sequential. Branches are parallel.
// There is an internal 'path', and the Number is a section of it. So a path is a number line, and a segment is a value on it.

// OK, a numberline is what was a domain. It is different because it can be composed of multiple properties that are interpreted together (path, book pages, whatever).
// It can have a multi dimensional basis, Like color defined with 3 properties (or a colorspace), and the numberline a path through it. The final Number is a section of that.
public interface INumberline : INumber
{
    IUnit Unit { get; }
    Focal Basis { get; }
    NumberPrimitive MinMax { get; } // Created with Unit MinMax and Basis, represents the calibreated numberline

    long TotalTicks { get; }
    long ZeroPoint { get; }
    List<Step> Steps { get; } // parallel ops

	List<Joint> Joints { get; } // maybe this isn't defined or stored in a property, but probably needs to be lookup-able.
        // need 'equation' to predict future points. For known equations the data can be generated rather than stored in steps.
}
public class Numberline : INumberline
{
	public IUnit Unit { get; }
	public Focal Basis { get; }
	public NumberPrimitive MinMax { get; }
    public long TotalTicks => MinMax.Unit.Resolution.Length;
    public long ZeroPoint => MinMax.Basis.StartTick;
	public List<Step> Steps { get; } = new List<Step>();
	public List<Joint> Joints { get; } = new List<Joint>();
	public Numberline(IUnit unit, Focal basis)
	{
		Unit = unit;
        Basis = basis;
        MinMax = new NumberPrimitive(unit, basis);
	}

}
public class XYPath : Numberline
{
    public static IUnit _defaultUnit { get; private set; } = new XYUnit();
    public XYPath(params Step[] steps) : base(_defaultUnit, Focal.One)
    {
        Steps.AddRange(steps);
    }
    public void AddStep(Step step)
    {
        Steps.Add(step);
    }
    public Portion GetPortion(Reference reference)
    {
        return new Portion(this, reference);
    }
}
