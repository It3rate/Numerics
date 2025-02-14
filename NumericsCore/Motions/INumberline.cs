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
        long TotalTicks { get; }
        SymmetricNumber MinMax { get; } // Encapsulates the Steps into a single dimensional range.
        long ZeroPoint { get; }
        List<Step> Steps { get; } // parallel ops

	List<Joint> Joints { get; } // maybe this isn't defined or stored in a property, but probably needs to be lookup-able.
        // need 'equation' to predict future points. For known equations the data can be generated rather than stored in steps.
}

public abstract class NumberlineBase : INumberline
{
	public abstract IUnit Unit { get; }
    public SymmetricNumber MinMax { get; private set; }
    public long TotalTicks { get; private set; }
    public long ZeroPoint { get; private set; }
	public List<Step> Steps { get; } = new List<Step>();

	public List<Joint> Joints { get; private set; } = new List<Joint>();

	public NumberlineBase(SymmetricNumber number)
    {
        MinMax = number;
    }
}
public class XYPath : NumberlineBase
{
    public static IUnit _defaultUnit { get; private set; } = new XYUnit();
	public override IUnit Unit => _defaultUnit;
    public XYPath(params Step[] steps) : base(new SymmetricNumber(_defaultUnit, Focal.One, _defaultUnit.Denominator))
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
