using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Motions
{
    // The conceptual primitive is a Property. This can be a simple symmetric number, or something more complex underneath.
    // A path is a Property if you only care about start to end, but if you care about both X and Y it is two properties.
    // Any branch is going to be two properties. Options or branches in a property are always defined by a joint and a new property.
    // Properties can have multiple paths, but they will be sequential. Branches are parallel.
    // There is an internal 'path', and the Number is a section of it. So a path is a number line, and a segment is a value on it.

    // OK, a numberline is what was a domain. It is different because it can be composed of multiple properties that are interpreted together (path, book pages, whatever).
    // It can have a multi dimensional basis, Like color defined with 3 properties (or a colorspace), and the numberline a path through it. The final Number is a section of that.
    public interface INumberline : INumber
    {
        long TotalTicks { get; }
        SymmetricNumber Number { get; }
        long ZeroPoint { get; }
        List<Joint> Joints { get; } // maybe this isn't defined or stored in a property, but probably needs to be lookup-able.
    }

    public class NumberlineBase : INumberline
    {
        public SymmetricNumber Number { get; private set; }
        public long TotalTicks { get; private set; }
        public long ZeroPoint { get; private set; }
        public List<Joint> Joints { get; private set; } = new List<Joint>();

        public NumberlineBase(SymmetricNumber number)
        {
            Number = number;
        }
    }
    public class Path : NumberlineBase
    {
        public static readonly Trait Trait = new Trait("2D Path");
        public static readonly Focal Resolution = new Focal(-1000, 1000);
        public Path() : base(new SymmetricNumber(Trait, Focal.One, Resolution))
        {
        }
    }
    public class Portion(INumberline numberline, Number segment)
    {
        public INumberline Numberline { get; } = numberline;
        public Number Segment { get; } = segment;
    }
}
