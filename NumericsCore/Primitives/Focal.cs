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
    public long TickLength => AbsTickLength * AbsDirection;
    public long AbsTickLength => Math.Abs(LastTick - FirstTick); // can't have zero length (that would be null/no focus, and this is a focal)
    public int Direction => FirstTick < LastTick ? 1 : FirstTick > LastTick ? -1 : 0; // zero is unknown
    public int AbsDirection => LastTick >= FirstTick ? 1 : -1; // default to positive direction when unknown
    public virtual long InvertedEndPosition => FirstTick - TickLength;

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
    public static Focal operator ++(Focal value) => new(value.FirstTick + 1, value.LastTick + 1);
    public static Focal operator +(Focal value) => new(value.FirstTick, value.LastTick);

    #endregion
    #region Subtract
    public static Focal Subtract(Focal left, Focal right) => left - right;
    public static Focal operator -(Focal left, Focal right) => new(left.FirstTick - right.FirstTick, left.LastTick - right.LastTick);
    static Focal ISubtractionOperators<Focal, Focal, Focal>.operator -(Focal left, Focal right) => left - right;

    public static Focal operator --(Focal value) => new(value.FirstTick - 1, value.LastTick - 1);
    public static Focal operator -(Focal value) => new(-value.FirstTick, -value.LastTick);
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
    #region Limits
    public static Focal MaxValue => new(long.MaxValue, long.MaxValue);
    public static Focal MinValue => new(long.MinValue, long.MinValue);

    #endregion


    public Focal Reverse() => new Focal(LastTick, FirstTick);
    public Focal Forward() => Direction >= 0 ? Clone() : Reverse();
    public Focal FlipAroundStart() => new Focal(FirstTick, InvertedEndPosition);
    public Focal Negate() => new Focal(-FirstTick, -LastTick);
    public Focal Expand(long multiple) => new(FirstTick * multiple, LastTick * multiple);
	public Focal Contract(long divisor) => new(FirstTick / divisor, LastTick / divisor);
    public Focal GetOffset(long offset) => new(FirstTick + offset, LastTick + offset);

    public static Focal Zero => new Focal(0, 0);
    public static Focal One => new Focal(0, 1);

    #region Equality
    public Focal Clone() => new (FirstTick, LastTick);
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
