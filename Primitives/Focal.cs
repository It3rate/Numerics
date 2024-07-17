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
    IAdditionOperators<Focal, Focal, Focal>,   
    ISubtractionOperators<Focal, Focal, Focal>,
    IAdditiveIdentity<Focal, Focal>,
    //IMultiplyOperators<Focal, Focal, Focal>,
    //IDivisionOperators<Focal, Focal, Focal>,
    //IMultiplicativeIdentity<Focal, Focal>,
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

    public long TickLength => AbsTickLength * AbsDirection;
    public long AbsTickLength => Math.Abs(EndTick - StartTick) + 1; // can't have zero length (that would be null/no focus, and this is a focal)
    public int Direction => TickLength > 0 ? 1 : TickLength < 0 ? -1 : 0; // zero is unknown
    public int AbsDirection => TickLength >= 0 ? 1 : -1; // default to positive direction when unknown
    public bool IsPositiveDirection => Direction > 0;
    public virtual long InvertedEndPosition => StartTick - AbsTickLength;

    public Focal(long start, long end)
    {
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
    /*
    #region Multiply
    public static Focal Multiply(Focal left, Focal right)
    {
        return left * right;
    }
    public static Focal operator *(Focal left, Focal right) => new(
        left.Start * right.End + left.End * right.Start,
        left.End * right.End - left.Start * right.Start);

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
        double leftEnd = left.End;
        double leftStart = left.Start;
        double rightEnd = right.End;
        double rightStart = right.Start;
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
    */

    #region MinMax
    public static Focal MaxValue => new(long.MaxValue, long.MaxValue);
    public static Focal MinValue => new(long.MinValue, long.MinValue);

    #endregion


    // IsFractional, IsInverted, IsNegative, IsNormalized, IsZero, IsOne, IsZeroStart, IsPoint, IsOverflow, IsUnderflow
    // IsLessThanBasis, IsGrowable, IsBasisLength, IsMin, HasMask, IsArray, IsMultiDim, IsCalculated, IsRandom
    // Domain: IsTickLessThanBasis, IsBasisInMinmax, IsTiling, IsClamping, IsInvertable, IsNegateable, IsPoly, HasTrait
    // scale


}
