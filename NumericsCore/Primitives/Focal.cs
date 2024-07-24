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

    public long StartTick
    {
        get => _positions[0];
        set
        {
            _positions[0] = value;
        }
    }
    public long EndTick
    {
        get => _positions[_positions.Length - 1];
        set
        {
             _positions[_positions.Length - 1] = value;
        }
    }

    public long NonZeroTickLength => TickLength == 0 ? 1 : TickLength;
    public long TickLength => AbsTickLength * AbsDirection;
    public long AbsTickLength => Math.Abs(EndTick - StartTick); // can't have zero length (that would be null/no focus, and this is a focal)
    public int Direction => StartTick < EndTick ? 1 : StartTick > EndTick ? -1 : 0; // zero is unknown
    public int AbsDirection => EndTick >= StartTick ? 1 : -1; // default to positive direction when unknown
    public virtual long InvertedEndPosition => StartTick - TickLength;

    public bool IsZeroAnchored => StartTick == 0;
    public bool IsZero => StartTick == 0 && EndTick == 0;
    public bool IsPositiveDirection => Direction > 0;
    public bool IsNegativeDirection => Direction < 0;
    public bool IsPoint => StartTick == EndTick;

    public Focal(long start, long end)
    {
        _positions = new long[2];
        StartTick = start;
        EndTick = end;
    }



    #region Add
    public static Focal Add(Focal left, Focal right) => left + right;
    public static Focal operator +(Focal left, Focal right) => new(left.StartTick + right.StartTick, left.EndTick + right.EndTick);
    static Focal IAdditionOperators<Focal, Focal, Focal>.operator +(Focal left, Focal right) => left + right;

    public static Focal AdditiveIdentity => new(0, 0);
    public static Focal operator ++(Focal value) => new(value.StartTick + 1, value.EndTick + 1);
    public static Focal operator +(Focal value) => new(value.StartTick, value.EndTick);

    #endregion
    #region Subtract
    public static Focal Subtract(Focal left, Focal right) => left - right;
    public static Focal operator -(Focal left, Focal right) => new(left.StartTick - right.StartTick, left.EndTick - right.EndTick);
    static Focal ISubtractionOperators<Focal, Focal, Focal>.operator -(Focal left, Focal right) => left - right;

    public static Focal operator --(Focal value) => new(value.StartTick - 1, value.EndTick - 1);
    public static Focal operator -(Focal value) => new(-value.StartTick, -value.EndTick);
    #endregion
    #region Multiply
    public static Focal Multiply(Focal left, Focal right)
    {
        return left * right;
    }
    public static Focal operator *(Focal left, Focal right) => new(
        left.StartTick * right.EndTick + left.EndTick * right.StartTick,
        left.EndTick * right.EndTick - left.StartTick * right.StartTick);

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
        double leftEnd = left.EndTick;
        double leftStart = left.StartTick;
        double rightEnd = right.EndTick;
        double rightStart = right.StartTick;
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


    public Focal Reverse() => new Focal(EndTick, StartTick);
    public Focal Forward() => Direction >= 0 ? Clone() : Reverse();
    public Focal FlipAroundStart() => new Focal(StartTick, InvertedEndPosition);
    public Focal Negate() => new Focal(-StartTick, -EndTick);
    public Focal Expand(long multiple) => new(StartTick * multiple, EndTick * multiple);
	public Focal Contract(long divisor) => new(StartTick / divisor, EndTick / divisor);
    public Focal GetOffset(long offset) => new(StartTick + offset, EndTick + offset);

    public static Focal Zero => new Focal(0, 0);
    public static Focal One => new Focal(0, 1);

    #region Equality
    public Focal Clone() => new (StartTick, EndTick);
    public override bool Equals(object? obj)
    {
        return obj is Focal other && Equals(other);
    }
    public bool Equals(Focal? value)
    {
        return ReferenceEquals(this, value) || StartTick.Equals(value.StartTick) && EndTick.Equals(value.EndTick);
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
            var hashCode = StartTick.GetHashCode();
            hashCode = (hashCode * 397) ^ EndTick.GetHashCode();
            return hashCode;
        }
    }
    #endregion

    public override string ToString() => $"[{StartTick} : {EndTick}]";
}
