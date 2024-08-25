using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NumericsCore.Primitives;
using NumericsCore.Utils;

namespace Numerics.Primitives;


public interface Numeric<T> where T :
    Numeric<T>,
    IAdditionOperators<T, T, T>,
    ISubtractionOperators<T, T, T>,
    IMultiplyOperators<T, T, T>,
    IDivisionOperators<T, T, T>,
    IIncrementOperators<T>,
    IDecrementOperators<T>,
    IUnaryNegationOperators<T, T>,
    IUnaryPlusOperators<T, T>
    //IMinMaxValue<T>
{
    Number AdditiveIdentity { get; }
    Number MultiplicativeIdentity { get; }
}
public class Number :
    Numeric<Number>,
    IAdditionOperators<Number, Number, Number>,
    ISubtractionOperators<Number, Number, Number>,
    IMultiplyOperators<Number, Number, Number>,
    IDivisionOperators<Number, Number, Number>,
    IIncrementOperators<Number>,
    IDecrementOperators<Number>,
    IUnaryNegationOperators<Number, Number>,
    IUnaryPlusOperators<Number, Number>
    //IPolarityOperators<Number, Number, Number>
//IMinMaxValue<Number>

{
    public Focal Focal { get; private set; }
    public Domain Domain { get; private set; }
    public Polarity Polarity => Domain.Polarity;
    public long StartTick => Focal.StartTick;
    public long EndTick => Focal.EndTick;
    public PRange Value => GetRange();
    public double StartValue => GetRange().Start;
    public double EndValue => GetRange().End;

    public long TickLength => Focal.TickLength;
    public long AbsTickLength => Focal.AbsTickLength;
    public Focal AlignedFocal => IsAligned ? Focal : ~Focal;

    // IsFractional, IsInverted, IsNegative, IsNormalized, IsZero, IsOne, IsZeroStart, IsPoint, IsOverflow, IsUnderflow
    // IsLessThanBasis, IsGrowable, IsBasisLength, IsMin, HasMask, IsArray, IsMultiDim, IsCalculated, IsRandom
    // Domain: IsTickLessThanBasis, IsBasisInMinmax, IsTiling, IsClamping, IsInvertable, IsNegateable, IsPoly, HasTrait
    // scale

    public Number(Domain domain, Focal focal)
    {
        Domain = domain;
        Focal = focal;
    }


    //public Number Invert() => Domain.CreateNumber(StartTick, true, EndTick, true, Polarity.Invert());
    //public Number Aligned => IsAligned ? this : Invert();
    //public Number Inverted => IsInverted ? this : Invert();
    //public Number ConvertToPolarity(Polarity target) => target == Polarity ? this : Invert();
    public Number Reverse() => Domain.Reverse(this);
    public Number ReverseNegate() => Domain.ReverseNegate(this);
    public Number MirrorStart() => Domain.MirrorStart(this);// inverted Conjugate, mirror operation
    public Number MirrorEnd() => Domain.MirrorEnd(this);

    #region Add
    public static Number operator +(Number left, Number rightIn)
    {
        var right = left.Domain.MapToDomain(rightIn);
        var (leftStart, leftEnd) = left.Domain.RawTicksFromZero(left);
        var (rightStart, rightEnd) = left.Domain.RawTicksFromZero(rightIn);
        return left.Domain.CreateNumberRaw(leftStart + rightStart, leftEnd + rightEnd);

    }
    public static Number Add(Number left, Number right) => left + right;
    static Number IAdditionOperators<Number, Number, Number>.operator +(Number left, Number right) => left + right;

    public static Number operator +(Number value) => new(value.Domain, value.Focal);
    public static Number operator ++(Number value) =>
        value.Domain.CreateNumberRaw(
            value.Focal.StartTick,
            value.Focal.EndTick + value.Domain.TickSize);
    public Number AdditiveIdentity => Domain.AdditiveIdentity;

    #endregion
    #region Subtract
    public static Number Subtract(Number left, Number right) => left - right;
    public static Number operator -(Number left, Number rightIn)
    {
        var right = left.Domain.MapToDomain(rightIn);
        var (leftStart, leftEnd) = left.Domain.RawTicksFromZero(left);
        var (rightStart, rightEnd) = left.Domain.RawTicksFromZero(rightIn);
        return left.Domain.CreateNumberRaw(leftStart - rightStart, leftEnd - rightEnd);
    }
    static Number ISubtractionOperators<Number, Number, Number>.operator -(Number left, Number right) => left - right;

    public static Number operator --(Number value) => new(value.Domain, value.Focal--);
    public static Number operator -(Number value) => new(value.Domain, -value.Focal);
    #endregion
    #region Multiply
    public static Number Multiply(Number left, Number right) =>  left * right;
    public static Number operator *(Number left, Number rightIn)
    {
        var right = left.Domain.MapToDomain(rightIn);
        var (leftStart, leftEnd) = left.Domain.SignedTicksFromZero(left);
        var (rightStart, rightEnd) = right.Domain.SignedTicksFromZero(right);
        var iVal = leftStart * rightEnd + leftEnd * rightStart;
        var rVal = leftEnd * rightEnd - leftStart * rightStart;
        var len = left.Domain.AbsBasisLength;
        return left.Domain.CreateNumberSigned(iVal / len, rVal / len);
    }


    static Number IMultiplyOperators<Number, Number, Number>.operator *(Number left, Number right) => left * right;
    public Number MultiplicativeIdentity => Domain.MultiplicativeIdentity;

    #endregion
    #region Divide
    public static Number Divide(Number left, Number right) => left / right;
    public static Number operator /(Number left, Number rightIn)
    {
        var right = left.Domain.MapToDomain(rightIn);
        var (leftStart, leftEnd) = left.Domain.SignedTicksFromZero(left);
        var (rightStart, rightEnd) = right.Domain.SignedTicksFromZero(right);

        long iVal;
        long rVal;
        var absLen = left.Domain.AbsBasisLength;
        if (Math.Abs(rightStart) < Math.Abs(rightEnd))
        {
            double num = rightStart / (double)rightEnd;
            iVal = (long)((leftStart - leftEnd * num) / (rightEnd + rightStart * num) * absLen);
            rVal = (long)((leftEnd + leftStart * num) / (rightEnd + rightStart * num) * absLen);
        }
        else
        {
            double num1 = rightEnd / (double)rightStart;
            iVal = (long)((-leftEnd + leftStart * num1) / (rightStart + rightEnd * num1) * absLen);
            rVal = (long)((leftStart + leftEnd * num1) / (rightStart + rightEnd * num1) * absLen);
        }
        return left.Domain.CreateNumberSigned(iVal, rVal);
    }
    #endregion
    #region Pow

    public static Number Pow(Number value, Number power)
    {
        if (power.IsZero)
        {
            return value.Domain.One;
        }

        if (value.IsZero)
        {
            return value.Domain.One;
        }
        // todo: this is temp. Correct polarity, use binomial, account for resolution
        var v = value.GetRange();
        var p = power.GetRange();
        var presult = PRange.Pow(v, p);
        var result = presult.ToNumber(value.Domain);
        return result;

        //double valueReal = value.m_real;
        //double valueImaginary = value.m_imaginary;
        //double powerReal = power.m_real;
        //double powerImaginary = power.m_imaginary;

        //double rho = Abs(value);
        //double theta = Math.Atan2(valueImaginary, valueReal);
        //double newRho = powerReal * theta + powerImaginary * Math.Log(rho);

        //double t = Math.Pow(rho, powerReal) * Math.Pow(Math.E, -powerImaginary * theta);

        //return new Number(t * Math.Cos(newRho), t * Math.Sin(newRho));
    }

    public static Number operator ^(Number left, Number right) => left ^ right;
    #endregion
    #region Polarity
    public int BasisDirection => Domain.Direction;
    public bool IsBasisPositive => BasisDirection == 1;
    public bool IsAligned => Polarity == Polarity.Aligned;
    public bool IsInverted => Polarity == Polarity.Inverted;
    public bool HasPolarity => Polarity.HasPolarity();
    public virtual bool IsPolarityEqual(Number num) => Polarity == num.Polarity;
    public static Number operator ~(Number value) => value.MirrorStart(); // todo: establish this meaning, maybe invert
    public Number Negate() => Domain.Negate(this);

    private static (long, long, long, long) GetStretchFocals(Number left, Number right, long offset, bool reciprocal)
    {
        // This gets the focal positions to multiply or divide.
        // The reason they are different for * and \ is division has an extra reciprocal when dividing by an inverted number.
        // 1/2i on the right parameter is inverted compared to 1/2 as the i flips perspective. After this multiply is normal.
        // The results are processed and still need to be converted to focals for aligned or inverted results, so this is handled in the methods.

        // Multiply (First Last, start, end)
        // ++ FL FL -s+e
        // +~ LF FL -e+s
        // ~+ LF FL +e-s
        // ~~ FL FL -s-e

        // Divide (First Last, start, end)
        // ++ FL FL +s+e
        // +~ LF LF -s+e
        // ~+ LF FL +e+s
        // ~~ FL FL -s-e

        long l0, l1, r0, r1;
        if (left.Polarity == right.Polarity)
        {
            l0 = -(left.StartTick - offset);
            l1 = (left.EndTick - offset);
        }
        else
        {
            l0 = -(left.EndTick - offset);
            l1 = (left.StartTick - offset);
        }

        if (reciprocal == true && left.Polarity == Polarity.Aligned && right.Polarity == Polarity.Inverted)
        {
            r0 = -(right.EndTick - offset);
            r1 = (right.StartTick - offset);
        }
        else
        {
            r0 = -(right.StartTick - offset);
            r1 = (right.EndTick - offset);
        }

        return (l0, l1, r0, r1);
    }
    private static (long, long, long, long) GetAdditiveFocals(Number left, Number right, long offset)
    {
        // This gets the focal positions to add or subtract, returns in the left context.
        // Add (First Last)
        // ++ FL FL
        // +~ FL LF
        // ~+ FL LF
        // ~~ FL FL

        var l0 = (left.StartTick - offset);
        var l1 = (left.EndTick - offset);
        long r0, r1;
        if (left.Polarity == right.Polarity)
        {
            r0 = (right.StartTick - offset);
            r1 = (right.EndTick - offset);
        }
        else
        {
            r0 = (right.EndTick - offset);
            r1 = (right.StartTick - offset);
        }

        return (l0, l1, r0, r1);
    }
    #endregion

    #region Ranges
    public PRange GetRange() => Domain.GetRange(this);
    #endregion

    #region Comparisons
    public static bool operator >(Number left, Number right) =>
        CompareFocals.GreaterThan(left.Focal, right.Focal) != null;
    public static bool operator >=(Number left, Number right) =>
        CompareFocals.GreaterThanOrEqual(left.Focal, right.Focal) != null;
    public static bool operator <(Number left, Number right) =>
        CompareFocals.LessThan(left.Focal, right.Focal) != null;
    public static bool operator <=(Number left, Number right) =>
        CompareFocals.LessThanOrEqual(left.Focal, right.Focal) != null;
    public bool IsMatching(Number right) =>
        CompareFocals.Matching(AlignedFocal, right.Focal) != null;
    public bool IsContaining(Number right) =>
        CompareFocals.Containing(AlignedFocal, right.Focal) != null;
    public bool IsContainedBy(Number right) =>
        CompareFocals.ContainedBy(AlignedFocal, right.Focal) != null;
    public bool IsGreaterThan(Number right) =>
        CompareFocals.GreaterThan(AlignedFocal, right.Focal) != null;
    public bool IsGreaterThanOrEqual(Number right) =>
        CompareFocals.GreaterThanOrEqual(AlignedFocal, right.Focal) != null;
    public bool IsGreaterThanAndEqual(Number right) =>
        CompareFocals.GreaterThanAndEqual(AlignedFocal, right.Focal) != null;
    public bool IsLessThan(Number right) =>
        CompareFocals.LessThan(AlignedFocal, right.Focal) != null;
    public bool IsLessThanOrEqual(Number right) =>
        CompareFocals.LessThanOrEqual(AlignedFocal, right.Focal) != null;
    public bool IsLessThanAndEqual(Number right) =>
        CompareFocals.LessThanAndEqual(AlignedFocal, right.Focal) != null;

    public static Number? Matching(Number left, Number right)
    {
        Number? result = null;
        var focal = CompareFocals.Matching(left.Focal, right.Focal);
        if (focal != null)
        {
            result = new Number(left.Domain, focal);
        }
        return result;
    }
    public static Number? Containing(Number left, Number right)
    {
        Number? result = null;
        var focal = CompareFocals.Containing(left.Focal, right.Focal);
        if (focal != null)
        {
            result = new Number(left.Domain, focal);
        }
        return result;
    }
    public static Number? ContainedBy(Number left, Number right)
    {
        Number? result = null;
        var focal = CompareFocals.ContainedBy(left.Focal, right.Focal);
        if (focal != null)
        {
            result = new Number(left.Domain, focal);
        }
        return result;
    }
    public static Number? GreaterThan(Number left, Number right)
    {
        Number? result = null;
        var focal = CompareFocals.GreaterThan(left.Focal, right.Focal);
        if (focal != null)
        {
            result = new Number(left.Domain, focal);
        }
        return result;
    }
    public static Number? GreaterThanOrEqual(Number left, Number right)
    {
        Number? result = null;
        var focal = CompareFocals.GreaterThanOrEqual(left.Focal, right.Focal);
        if (focal != null)
        {
            result = new Number(left.Domain, focal);
        }
        return result;
    }
    public static Number? GreaterThanAndEqual(Number left, Number right)
    {
        Number? result = null;
        var focal = CompareFocals.GreaterThanAndEqual(left.Focal, right.Focal);
        if (focal != null)
        {
            result = new Number(left.Domain, focal);
        }
        return result;
    }
    public static Number? LessThan(Number left, Number right)
    {
        Number? result = null;
        var focal = CompareFocals.LessThan(left.Focal, right.Focal);
        if (focal != null)
        {
            result = new Number(left.Domain, focal);
        }
        return result;
    }
    public static Number? LessThanOrEqual(Number left, Number right)
    {
        Number? result = null;
        var focal = CompareFocals.LessThanOrEqual(left.Focal, right.Focal);
        if (focal != null)
        {
            result = new Number(left.Domain, focal);
        }
        return result;
    }
    public static Number? LessThanAndEqual(Number left, Number right)
    {
        Number? result = null;
        var focal = CompareFocals.LessThanAndEqual(left.Focal, right.Focal);
        if(focal != null)
        {
            result = new Number(left.Domain, focal);
        }
        return result;
    }
    #endregion
    #region Equality
    public bool IsZero => Domain.IsZero(this);
    public bool IsOne => Domain.IsOne(this);
    public Number Clone() => new Number(Domain, Focal.Clone());
    public static bool operator ==(Number? a, Number? b)
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
    public static bool operator !=(Number? a, Number? b)
    {
        return !(a == b);
    }
    public override bool Equals(object? obj)
    {
        return obj is Number other && Equals(other);
    }
    public bool Equals(Number? value)
    {
        if (value is null) { return false; }
        return ReferenceEquals(this, value) ||
                (
                Polarity == value.Polarity &&
                Focal.Equals(this.Focal, value.Focal)
                );
    }
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Focal.GetHashCode() * 17 ^ ((int)Polarity + 27) * 397;// + (IsValid ? 77 : 33);
            return hashCode;
        }
    }
    #endregion

    public override string ToString()
    {
        string result;
        var val = GetRange();
        //var vStart = Domain.DecimalValue(StartTick);
        //var vEnd = Domain.DecimalValue(EndTick);
        if (Polarity == Polarity.None)
        {
            result = $"x({val.Start:0.##}_{val.End:0.##})"; // no polarity, so just list values
        }
        else
        {
            var midSign = val.End > 0 ? " + " : " ";
            var pol = Polarity == Polarity.Inverted ? "~" : "";
            var start = Value.Start == 0 ? "0" : $"{val.Start:0.##}";
            var end = val.End == 0 ? "0" : $"{val.End:0.##}";
            result = $"{pol}({start}i{midSign}{end})";
        }
        return result;
    }
}
