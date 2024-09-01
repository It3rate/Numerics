using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NumericsCore.Interfaces;
using NumericsCore.Primitives;
using NumericsCore.Utils;

namespace Numerics.Primitives;


public class Number:
    Numeric<Number>,
    IValue,
    IAdditionOperators<Number, Number, Number>,
    ISubtractionOperators<Number, Number, Number>,
    IMultiplyOperators<Number, Number, Number>,
    IDivisionOperators<Number, Number, Number>,
    IIncrementOperators<Number>,
    IDecrementOperators<Number>,
    IUnaryNegationOperators<Number, Number>,
    IUnaryPlusOperators<Number, Number>,
    IPolarityOperators<Number, Number, Number>
    //IMinMaxValue<Number>

{
    public Focal Focal { get; }
    public Domain Domain { get; }
    public double StartValue => Domain.ValueAtT(this, 0);
    public double EndValue => Domain.ValueAtT(this, 1);
    public double ValueAtT(double t) => Domain.ValueAtT(this, t);

    public Number(Domain domain, Focal focal)
    {
        Domain = domain;
        Focal = focal;
    }

    #region Conversions
    public PRange GetRange() => Domain.GetRange(this);
    #endregion

    #region Truths
    public bool IsZero => Domain.IsZero(this);
    public bool IsOne => Domain.IsOne(this);
    public bool IsNegativeDirection => Focal.Direction * Domain.Direction == -1;
    public bool IsZeroStart => StartValue == 0;
    public bool IsPoint => StartTick == EndTick;
    public bool IsSmallerThanBasis => Math.Abs(StartValue) < 1 && Math.Abs(EndValue) < 1;
    public bool IsBasisPermutation => // 1, -1, i, -i
        (StartValue == 0 && Math.Abs(EndValue) == 1) || 
        (EndValue == 0 && Math.Abs(StartValue) == 1);
    public bool IsFractional => StartValue != (int)StartValue || EndValue != (int)EndValue;
    // IsNormalized, IsOverflow, IsUnderflow IsGrowable, IsMin, HasMask, IsArray, IsMultiDim, IsCalculated, IsRandom

    #endregion
    #region Properties
    public Polarity Polarity => Domain.Polarity;
    public long StartTick => Focal.StartTick;
    public long EndTick => Focal.EndTick;

    public long TickLength => Focal.Length;
    public long AbsTickLength => Focal.AbsLength;
    #endregion
    #region Add
    public static Number operator +(Number left, Number rightIn)
    {
        var right = left.Domain.MapToDomain(rightIn);
        var (leftStart, leftEnd) = left.Domain.RawTicksFromZero(left);
        var (rightStart, rightEnd) = left.Domain.RawTicksFromZero(rightIn);
        return left.Domain.CreateNumberRaw(leftStart + rightStart, leftEnd + rightEnd);

    }
    public Number Add(Number right) => this + right;
    public static Number Add(Number left, Number right) => left + right;
    static Number IAdditionOperators<Number, Number, Number>.operator +(Number left, Number right) => left + right;

    public static Number operator +(Number value) => new(value.Domain, value.Focal);
    public static Number operator ++(Number value) => new(value.Domain, value.Focal + value.Domain.BasisNumber.Focal);
    public Number AdditiveIdentity => Domain.AdditiveIdentity;

    #endregion
    #region Subtract
    public Number Subtract(Number right) => this - right;
    public static Number Subtract(Number left, Number right) => left - right;
    public static Number operator -(Number left, Number rightIn)
    {
        var right = left.Domain.MapToDomain(rightIn);
        var (leftStart, leftEnd) = left.Domain.RawTicksFromZero(left);
        var (rightStart, rightEnd) = left.Domain.RawTicksFromZero(rightIn);
        return left.Domain.CreateNumberRaw(leftStart - rightStart, leftEnd - rightEnd);
    }
    static Number ISubtractionOperators<Number, Number, Number>.operator -(Number left, Number right) => left - right;

    public static Number operator --(Number value) => new(value.Domain, value.Focal - value.Domain.BasisNumber.Focal);
    public static Number operator -(Number value) => new(value.Domain, -value.Focal);
    #endregion
    #region Multiply
    public Number Multiply(Number right) => this * right;
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
    public Number Divide(Number right) => this / right;
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
    public Number Pow(Number power) => this ^ power;
    public static Number operator ^(Number value, Number power)
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

    public static Number Pow(Number value, Number power)  => value ^ power;
    #endregion
    #region Polarity
    public int BasisDirection => Domain.Direction;
    public bool IsAligned => Polarity == Polarity.Aligned;
    public bool IsInverted => Polarity == Polarity.Inverted;
    public bool HasPolarity => Polarity.HasPolarity();
    public virtual bool IsPolarityEqual(Number num) => Polarity == num.Polarity;
    public Number Negate() => Domain.Negate(this);
    public Number InvertPolarity() => new (Domain.Inverse, Focal);
    public Number InvertPolarityAndFocal()=> new(Domain.Inverse, Focal.Negate());

    #endregion
    #region Transforms
    public Number Reverse() => Domain.Reverse(this);
    public Number ReverseNegate() => Domain.ReverseNegate(this);
    public static Number operator ~(Number value) => value.MirrorStart(); // conjugate on i is the 2D mirror operation (flip vertically)
    public Number MirrorStart() => Domain.MirrorStart(this);// inverted Conjugate, mirror operation
    public Number MirrorEnd() => Domain.MirrorEnd(this);
    #endregion
    #region Comparisons Bools
    public static bool operator >(Number left, Number right) =>
        CompareFocals.GreaterThan(left.Focal, right.Focal) != null;
    public static bool operator >=(Number left, Number right) =>
        CompareFocals.GreaterThanOrEqual(left.Focal, right.Focal) != null;
    public static bool operator <(Number left, Number right) =>
        CompareFocals.LessThan(left.Focal, right.Focal) != null;
    public static bool operator <=(Number left, Number right) =>
        CompareFocals.LessThanOrEqual(left.Focal, right.Focal) != null;
    public bool IsMatching(Number right) =>
        CompareFocals.Matching(Focal, right.Focal) != null;
    public bool IsContaining(Number right) =>
        CompareFocals.Containing(Focal, right.Focal) != null;
    public bool IsContainedBy(Number right) =>
        CompareFocals.ContainedBy(Focal, right.Focal) != null;
    public bool IsGreaterThan(Number right) =>
        CompareFocals.GreaterThan(Focal, right.Focal) != null;
    public bool IsGreaterThanOrEqual(Number right) =>
        CompareFocals.GreaterThanOrEqual(Focal, right.Focal) != null;
    public bool IsGreaterThanAndEqual(Number right) =>
        CompareFocals.GreaterThanAndEqual(Focal, right.Focal) != null;
    public bool IsLessThan(Number right) =>
        CompareFocals.LessThan(Focal, right.Focal) != null;
    public bool IsLessThanOrEqual(Number right) =>
        CompareFocals.LessThanOrEqual(Focal, right.Focal) != null;
    public bool IsLessThanAndEqual(Number right) =>
        CompareFocals.LessThanAndEqual(Focal, right.Focal) != null;

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
            var start = val.Start == 0 ? "0" :
                val.Start == 1 ? "" :
                val.Start == -1 ? "-" :
                $"{val.Start:0.##}";

            var end = val.End == 0 ? "0" : $"{val.End:0.##}";
            result = $"{pol}({start}i{midSign}{end})";
        }
        return result;
    }
}
