using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Numerics.Primitives;

//Focal
//FocalSequence
//FocalBlend

//Domain(basis, minmax)
//DomainBlend
//DomainCluster

//Number
//NumberSequence/Chain
//NumberBlend
//NumberGraph
//NumberCluster

public class Focal :
    IMeasurable,
    Measurable<Focal>, 
    IEquatable<Focal>,
    IAdditionOperators<Focal, Focal, Focal>,   
    ISubtractionOperators<Focal, Focal, Focal>,
    IAdditiveIdentity<Focal, Focal>,
    IMultiplyOperators<Focal, Focal, Focal>,
    IDivisionOperators<Focal, Focal, Focal>,
    IMultiplicativeIdentity<Focal, Focal>,
    IIncrementOperators<Focal>,
    IDecrementOperators<Focal>,
    IUnaryNegationOperators<Focal, Focal>,
    IUnaryPlusOperators<Focal, Focal>,
    IMinMaxValue<Focal>
{
    protected long[] _positions;

    public long FirstTick
    {
        get => _positions[0];
        set
        {
            _positions[0] = value;
        }
    }
    public long LastTick
    {
        get => _positions[_positions.Length - 1];
        set
        {
             _positions[_positions.Length - 1] = value;
        }
    }

    public long NonZeroTickLength => TickLength == 0 ? 1 : TickLength;
    public long TickLength => AbsTickLength * NonZeroDirection;
    public long AbsTickLength => Math.Abs(LastTick - FirstTick); // can't have zero length (that would be null/no focus, and this is a focal)
    public int Direction => FirstTick < LastTick ? 1 : FirstTick > LastTick ? -1 : 0; // zero is unknown
    public int NonZeroDirection => LastTick >= FirstTick ? 1 : -1; // default to positive direction when unknown
    public long InvertedFirstPosition => LastTick + TickLength;
    public long InvertedLastPosition => FirstTick - TickLength;

    public bool IsZeroAnchored => FirstTick == 0;
    public bool IsZero => FirstTick == 0 && LastTick == 0;
    public bool IsPositiveDirection => Direction > 0;
    public bool IsNegativeDirection => Direction < 0;
    public bool IsPoint => FirstTick == LastTick;

    public Focal(long start, long end)
    {
        _positions = new long[2];
        FirstTick = start;
        LastTick = end;
    }

    #region Add
    public static Focal Add(Focal left, Focal right) => left + right;
    public static Focal operator +(Focal left, Focal right) => new(left.FirstTick + right.FirstTick, left.LastTick + right.LastTick);
    static Focal IAdditionOperators<Focal, Focal, Focal>.operator +(Focal left, Focal right) => left + right;

    public static Focal AdditiveIdentity => new(0, 0);

    #endregion
    #region Subtract
    public static Focal Subtract(Focal left, Focal right) => left - right;
    public static Focal operator -(Focal left, Focal right) => new(left.FirstTick - right.FirstTick, left.LastTick - right.LastTick);
    static Focal ISubtractionOperators<Focal, Focal, Focal>.operator -(Focal left, Focal right) => left - right;
    #endregion
    #region Multiply
    public static Focal Multiply(Focal left, Focal right)
    {
        return left * right;
    }
    public static Focal operator *(Focal left, Focal right) => new(
        left.FirstTick * right.LastTick + left.LastTick * right.FirstTick,
        left.LastTick * right.LastTick - left.FirstTick * right.FirstTick);

    static Focal IMultiplyOperators<Focal, Focal, Focal>.operator *(Focal left, Focal right) => left * right;
    public static Focal MultiplicativeIdentity => new(0, 1);

    #endregion
    #region Divide
    public static Focal Divide(Focal left, Focal right)
    {
        return left / right;
    }

    public static Focal operator /(Focal left, Focal right)
    {
        Focal result;
        double leftEnd = left.LastTick;
        double leftStart = left.FirstTick;
        double rightEnd = right.LastTick;
        double rightStart = right.FirstTick;
        if (Math.Abs(rightStart) < Math.Abs(rightEnd))
        {
            double num = rightStart / rightEnd;
            result = new Focal(
                (long)((leftStart - leftEnd * num) / (rightEnd + rightStart * num)),
                (long)((leftEnd + leftStart * num) / (rightEnd + rightStart * num)));
        }
        else
        {
            double num1 = rightEnd / rightStart;
            result = new Focal(
                (long)((-leftEnd + leftStart * num1) / (rightStart + rightEnd * num1)),
                (long)((leftStart + leftEnd * num1) / (rightStart + rightEnd * num1)));
        }
        return result;
    }
    static Focal IDivisionOperators<Focal, Focal, Focal>.operator /(Focal left, Focal right) => left / right;

    #endregion
    #region Unary Ops
    public static Focal operator ++(Focal value) => new(value.FirstTick + 1, value.LastTick + 1);
    public static Focal operator +(Focal value) => new(value.FirstTick, value.LastTick);
    public static Focal operator --(Focal value) => new(value.FirstTick - 1, value.LastTick - 1);
    public static Focal operator -(Focal value) => new(-value.FirstTick, -value.LastTick);
    public static Focal operator ~(Focal value) => new(value.LastTick, value.FirstTick);
    public Focal Negate() => -this;// new Focal(-FirstTick, -LastTick);
    public Focal Invert() => ~this;// new Focal(LastTick, FirstTick);
    public Focal PositiveDirection() => Direction >= 0 ? Clone() : Invert();
    public Focal NegativeDirection() => Direction < 0 ? Clone() : Invert();
    public Focal FlipAroundFirst() => new Focal(FirstTick, InvertedLastPosition);
    #endregion
    #region Limits
    public static Focal FocalAtLimits => new Focal(long.MinValue, long.MaxValue);
    public static Focal MaxValue => new(long.MaxValue, long.MaxValue);
    public static Focal MinValue => new(long.MinValue, long.MinValue);
    public long Min => FirstTick <= LastTick ? FirstTick : LastTick;
    public long Max => FirstTick >= LastTick ? FirstTick : LastTick;

    #endregion

    public Focal GetOffset(long offset) => new(FirstTick + offset, LastTick + offset);
    public Focal FocalFromTs(double startT, double endT, bool invertEnds = false)
    {
        Focal result;
        if(invertEnds)
        {
            result = new((long)(FirstTick + TickLength * startT), (long)(FirstTick + TickLength * endT));
        }
        else
        {
            result = new((long)(LastTick + TickLength * -startT), (long)(LastTick + TickLength * -endT));
        }
        return result;
    }

    public static Focal Zero => new Focal(0, 0);
    public static Focal One => new Focal(0, 1);
    public static Focal ZeroAnchoredFocal(long ticks) { return new Focal(0, ticks); }


    #region Bool Ops
    public static long MinPosition(Focal p, Focal q) => Math.Min(p.Min, q.Min);
    public static long MaxPosition(Focal p, Focal q) => Math.Max(p.Max, q.Max);
    public static long MinStart(Focal p, Focal q) => Math.Min(p.FirstTick, q.FirstTick);
    public static long MaxStart(Focal p, Focal q) => Math.Max(p.FirstTick, q.FirstTick);
    public static long MinEnd(Focal p, Focal q) => Math.Min(p.LastTick, q.LastTick);
    public static long MaxEnd(Focal p, Focal q) => Math.Max(p.LastTick, q.LastTick);

    /// <summary>
    /// Shares an endpoint, but no overlap (meaning can only share one endpoint).
    /// </summary>
    public static bool Touches(Focal p, Focal q) => p.LastTick == q.FirstTick || p.FirstTick == q.LastTick;
    public static Focal? Intersection(Focal p, Focal q)
    {
        Focal result = null;
        var ov = Overlap(p, q);
        if (ov.TickLength != 0)
        {
            result = ov;
            if (!p.IsPositiveDirection) { ov.Invert(); }
        }
        return result;
    }
    public static Focal Overlap(Focal p, Focal q)
    {
        var start = Math.Max(p.Min, q.Min);
        var end = Math.Min(p.Max, q.Max);
        return (start >= end) ? new Focal(0, 0) : new Focal(start, end);
    }
    public static Focal Extent(Focal p, Focal q)
    {
        var start = Math.Min(p.Min, q.Min);
        var end = Math.Max(p.Max, q.Max);
        return new Focal(start, end);
    }

    // Q. Should direction be preserved in a bool operation?
    public static Focal[] Never(Focal p)
    {
        return new Focal[0];
    }
    public static Focal[] UnaryNot(Focal p)
    {
        // If p starts at the beginning of the time frame and ends at the end, A is always true and the "not A" relationship is empty
        if (p.FirstTick == 0 && p.LastTick == long.MaxValue)
        {
            return new Focal[] { };
        }
        // If p starts at the beginning of the time frame and ends before the end, the "not A" relationship consists of a single interval from p.EndTickPosition + 1 to the end of the time frame
        else if (p.FirstTick == 0)
        {
            return new Focal[] { new Focal(p.LastTick + 1, long.MaxValue) };
        }
        // If p starts after the beginning of the time frame and ends at the end, the "not A" relationship consists of a single interval from the beginning of the time frame to p.StartTickPosition - 1
        else if (p.LastTick == long.MaxValue)
        {
            return new Focal[] { new Focal(0, p.FirstTick - 1) };
        }
        // If p starts and ends within the time frame, the "not A" relationship consists of two intervals: from the beginning of the time frame to p.StartTickPosition - 1, and from p.EndTickPosition + 1 to the end of the time frame
        else
        {
            return new Focal[] { new Focal(0, p.FirstTick - 1), new Focal(p.LastTick + 1, long.MaxValue) };
        }
    }
    public static Focal[] Transfer(Focal p)
    {
        return new Focal[] { p.Clone() };
    }
    public static Focal[] Always(Focal p)
    {
        return new Focal[] { Focal.FocalAtLimits.Clone() };
    }

    public static Focal[] Never(Focal p, Focal q)
    {
        return new Focal[0];
    }
    public static Focal[] And(Focal p, Focal q)
    {
        var overlap = Overlap(p, q);
        return (overlap.TickLength == 0) ? new Focal[0] : new Focal[] { overlap };
    }
    public static Focal[] B_Inhibits_A(Focal p, Focal q)
    {
        if (p.LastTick < q.FirstTick - 1 || q.LastTick < p.FirstTick - 1)
        {
            return new Focal[] { p };
        }
        else
        {
            return new Focal[] { new Focal(p.FirstTick, q.FirstTick - 1) };
        }
    }
    public static Focal[] Transfer_A(Focal p, Focal q)
    {
        return new Focal[] { p };
    }
    public static Focal[] A_Inhibits_B(Focal p, Focal q)
    {
        if (p.LastTick < q.FirstTick - 1 || q.LastTick < p.FirstTick - 1)
        {
            return new Focal[] { q };
        }
        else
        {
            return new Focal[] { new Focal(q.FirstTick, p.FirstTick - 1) };
        }
    }
    public static Focal[] Transfer_B(Focal p, Focal q)
    {
        return new Focal[] { q };
    }
    public static Focal[] Xor(Focal p, Focal q)
    {
        // Return the symmetric difference of the two input segments as a new array of segments
        List<Focal> result = new List<Focal>();
        Focal[] andResult = And(p, q);
        if (andResult.Length == 0)
        {
            // If the segments do not intersect, return the segments as separate non-overlapping segments
            result.Add(p);
            result.Add(q);
        }
        else
        {
            // If the segments intersect, return the complement of the intersection in each segment
            Focal[] complement1 = Nor(p, andResult[0]);
            Focal[] complement2 = Nor(q, andResult[0]);
            result.AddRange(complement1);
            result.AddRange(complement2);
        }
        return result.ToArray();
    }
    public static Focal[] Or(Focal p, Focal q)
    {
        var overlap = Overlap(p, q);
        return (overlap.TickLength == 0) ? new Focal[] { p, q } : new Focal[] { Extent(p, q) };
    }
    public static Focal[] Nor(Focal p, Focal q)
    {
        // Return the complement of the union of the two input Focals as a new array of Focals
        List<Focal> result = new List<Focal>();
        Focal[] orResult = Or(p, q);
        if (orResult.Length == 0)
        {
            // If the Focals do not overlap, return both Focals as separate non-overlapping Focals
            result.Add(p);
            result.Add(q);
        }
        else
        {
            // If the Focals overlap, return the complement of the union in each Focal
            Focal[] complement1 = Nand(p, orResult[0]);
            Focal[] complement2 = Nand(q, orResult[0]);
            result.AddRange(complement1);
            result.AddRange(complement2);
        }
        return result.ToArray();
    }
    public static Focal[] Xnor(Focal p, Focal q)
    {
        // Return the complement of the symmetric difference of the two input Focals as a new array of Focals
        List<Focal> result = new List<Focal>();
        Focal[] xorResult = Xor(p, q);
        if (xorResult.Length == 0)
        {
            // If the Focals are equal, return p as a single Focal
            result.Add(p);
        }
        else
        {
            // If the Focals are not equal, return the complement of the symmetric difference in each Focal
            Focal[] complement1 = Nor(p, xorResult[0]);
            Focal[] complement2 = Nor(q, xorResult[0]);
            result.AddRange(complement1);
            result.AddRange(complement2);
        }
        return result.ToArray();
    }
    public static Focal[] Not_B(Focal p, Focal q)
    {
        return UnaryNot(q);
    }
    public static Focal[] B_Implies_A(Focal p, Focal q)
    {
        if (p.LastTick < q.FirstTick - 1 || q.LastTick < p.FirstTick - 1)
        {
            return new Focal[] { };
        }
        else
        {
            return new Focal[] { new Focal(MaxStart(p, q), MinEnd(p, q)) };
        }
    }
    public static Focal[] Not_A(Focal p, Focal q)
    {
        return UnaryNot(p);
    }
    public static Focal[] A_Implies_B(Focal p, Focal q)
    {
        if (p.LastTick < q.FirstTick - 1 || q.LastTick < p.FirstTick - 1)
        {
            return new Focal[] { };
        }
        else
        {
            return new Focal[] { new Focal(MaxStart(p, q), MinEnd(p, q)) };
        }
    }
    public static Focal[] Nandx(Focal p, Focal q)
    {
        // Return the complement of the intersection of the two input Focals as a new array of Focals
        List<Focal> result = new List<Focal>();
        Focal[] andResult = And(p, q);
        if (andResult.Length == 0)
        {
            // If the Focals do not intersect, return the union of the Focals as a single Focal
            result.Add(new Focal(MinStart(p, q), MaxEnd(p, q)));
        }
        else
        {
            // If the Focals intersect, return the complement of the intersection in each Focal
            Focal[] complement1 = Nor(p, andResult[0]);
            Focal[] complement2 = Nor(q, andResult[0]);
            result.AddRange(complement1);
            result.AddRange(complement2);
        }
        return result.ToArray();
    }
    public static Focal[] Nand(Focal p, Focal q)
    {
        Focal[] result;
        var overlap = Overlap(p, q);
        if (overlap.TickLength == 0)
        {
            result = new Focal[] { Focal.FocalAtLimits.Clone() };
        }
        else
        {
            result = new Focal[]
            {
                new Focal(long.MinValue, overlap.FirstTick),
                new Focal(overlap.LastTick, long.MaxValue)
            };
        }
        return result;
    }
    public static Focal[] Always(Focal p, Focal q)
    {
        return new Focal[] { Focal.FocalAtLimits.Clone() };
    }

    #endregion

    #region Equality
    public Focal Clone() => new(FirstTick, LastTick);
    public override bool Equals(object? obj)
    {
        return obj is Focal other && Equals(other);
    }
    public bool Equals(Focal? value)
    {
        return ReferenceEquals(this, value) || FirstTick.Equals(value.FirstTick) && LastTick.Equals(value.LastTick);
    }

    public static bool operator ==(Focal? a, Focal? b)
    {
        if (a is null && b is null)
        {
            return true;
        }
        if (a is null || b is null)
        {
            return false;
        }
        return a.Equals(b);
    }

    public static bool operator !=(Focal? a, Focal? b)
    {
        return !(a == b);
    }
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = FirstTick.GetHashCode();
            hashCode = (hashCode * 397) ^ LastTick.GetHashCode();
            return hashCode;
        }
    }
    #endregion
    public override string ToString() => $"[{FirstTick} : {LastTick}]";
}
