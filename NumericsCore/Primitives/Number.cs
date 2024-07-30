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
    IUnaryPlusOperators<Number, Number>,
    IPolarityOperators<Number, Number, Number>
//IMinMaxValue<Number>

{
    public Focal Focal { get; private set; }
    public Domain Domain { get; private set; }
	public Polarity Polarity { get; set; }
    public long FirstTick
    {
        get => Focal.FirstTick;
        set => Focal.FirstTick = value; 
    }
    public long LastTick 
    {
        get => Focal.LastTick;
        set => Focal.LastTick = value;
    }
    private long _iTick
    {
        get => IsAligned? FirstTick : LastTick;
        set { if (IsAligned) { FirstTick = value; } else { LastTick = value; } }
    }
    private long _rTick 
    {
        get => IsAligned ? LastTick : FirstTick;
        set { if (IsAligned) { LastTick = value; } else { FirstTick = value; } }
    }
    public PRange Value => GetRange();
    public double StartValue => GetRange().Start;// -Domain.DecimalValue(StartTick);
    public double EndValue => GetRange().End;//Domain.DecimalValue(EndTick);

    public long TickLength => Focal.TickLength;

    public long AbsTickLength => Focal.AbsTickLength;

    // IsFractional, IsInverted, IsNegative, IsNormalized, IsZero, IsOne, IsZeroStart, IsPoint, IsOverflow, IsUnderflow
    // IsLessThanBasis, IsGrowable, IsBasisLength, IsMin, HasMask, IsArray, IsMultiDim, IsCalculated, IsRandom
    // Domain: IsTickLessThanBasis, IsBasisInMinmax, IsTiling, IsClamping, IsInvertable, IsNegateable, IsPoly, HasTrait
    // scale

    public Number(Domain domain, Focal focal, Polarity polarity = Polarity.Aligned)
    {
        Domain = domain;
        Focal = focal;
		Polarity = polarity;
	}
	#region Add
	public static Number operator +(Number left, Number right)
	{
		var aligned = left.Domain.AlignedDomain(right);
        var offset = left.Domain.BasisFocal.FirstTick;
        var (l0, l1, r0, r1) = GetAdditiveFocals(left, aligned, offset);
        Number result = new(left.Domain, new((l0 + r0) + offset, (l1 + r1) + offset), left.Polarity);
        return result;
    }
	public static Number Add(Number left, Number right) => left + right;
	static Number IAdditionOperators<Number, Number, Number>.operator +(Number left, Number right) => left + right;

	public static Number operator +(Number value) => new(value.Domain, value.Focal);
	public static Number operator ++(Number value) => new(value.Domain, value.Focal++);
	public Number AdditiveIdentity => Domain.AdditiveIdentity;

	#endregion
	#region Subtract
	public static Number Subtract(Number left, Number right) => left - right;
	public static Number operator -(Number left, Number right)
    {
        var aligned = left.Domain.AlignedDomain(right);
        var offset = left.Domain.BasisFocal.FirstTick;
        var (l0, l1, r0, r1) = GetAdditiveFocals(left, aligned, offset);
        Number result = new (left.Domain, new((l0 - r0) + offset, (l1 - r1) + offset), left.Polarity);
        return result;
    }
    static Number ISubtractionOperators<Number, Number, Number>.operator -(Number left, Number right) => left - right;

	public static Number operator --(Number value) => new(value.Domain, value.Focal--);
	public static Number operator -(Number value) => new(value.Domain, -value.Focal);
	#endregion
	#region Multiply
	public static Number Multiply(Number left, Number right)
	{
		return left * right;
	}
	public static Number operator *(Number left, Number right)
    {
        var aligned = left.Domain.AlignedDomain(right);
        var len = (double)left.Domain.BasisFocal.TickLength;
        var offset = left.Domain.BasisFocal.FirstTick;
        var (l0, l1, r0, r1) = GetStretchFocals(left, aligned, offset, false);

        var start = (long)((l0 * r1 + l1 * r0) / len);
        var end = (long)((l1 * r1 - l0 * r0) / len);

        Number result;
        if (left.Polarity == Polarity.Aligned && right.Polarity == Polarity.Aligned)// + + 
        {
            result = new(left.Domain, new(-start + offset, end + offset), Polarity.Aligned);
        }
        else if (left.Polarity == Polarity.Aligned && right.Polarity == Polarity.Inverted)// + ~ 
        {
            result = new(left.Domain, new(-end + offset, -start + offset), Polarity.Inverted);
        }
        else if (left.Polarity == Polarity.Inverted && right.Polarity == Polarity.Aligned)// ~ +
        {
            result = new(left.Domain, new(end + offset, -start + offset), Polarity.Inverted);
        }
        else // if (left.Polarity == Polarity.Inverted && right.Polarity == Polarity.Inverted)// ~ ~ 
        {
            result = new(left.Domain, new(-start + offset, -end + offset), Polarity.Aligned);
        }
        return result;
    }


    static Number IMultiplyOperators<Number, Number, Number>.operator *(Number left, Number right) => left * right;
	public Number MultiplicativeIdentity => Domain.MultiplicativeIdentity;

    #endregion
    #region Divide
    public static Number Divide(Number left, Number right)
	{
		return left / right;
	}
	public static Number operator /(Number left, Number right)
    {
        var aligned = left.Domain.AlignedDomain(right);
        var dir = left.Domain.BasisFocal.NonZeroDirection;
        var len = left.Domain.BasisFocal.TickLength;
        var offset = left.Domain.BasisFocal.FirstTick;
        var leftDir = left.PolarityDirection;
        var rightDir = right.PolarityDirection;

        var (l0, l1, r0, r1) = GetStretchFocals(left, aligned, offset, true);
        var denom = (double)(r0 * r0 + r1 * r1);
        long start = (long)((l1 * r0 - l0 * r1) / denom * len);
        long end = (long)((l0 * r0 + l1 * r1) / denom * len);

        Number result;
        if (left.Polarity == Polarity.Aligned && right.Polarity == Polarity.Aligned)// + + 
        {
            result = new(left.Domain, new(start + offset, end + offset), Polarity.Aligned);
        }
        else if (left.Polarity == Polarity.Aligned && right.Polarity == Polarity.Inverted)// + ~ 
        {
            result = new(left.Domain, new(-start + offset, end + offset), Polarity.Inverted);
        }
        else if (left.Polarity == Polarity.Inverted && right.Polarity == Polarity.Aligned)// ~ +
        {
            result = new(left.Domain, new(end + offset, start + offset), Polarity.Inverted);
        }
        else // if (left.Polarity == Polarity.Inverted && right.Polarity == Polarity.Inverted)// ~ ~ 
        {
            result = new(left.Domain, new(-start + offset, -end + offset), Polarity.Aligned);
        }
        return result;
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
    public int PolarityDirection => Polarity.Direction();
    public int BasisDirection => Domain.BasisFocal.Direction;
    public bool IsBasisPositive => BasisDirection == 1;
    public int PositiveTickDirection => Domain.BasisFocal.Direction * PolarityDirection;
    public bool HasPolairty => Polarity.HasPolarity();
    public bool IsAligned => Polarity == Polarity.Aligned;
    public bool IsInverted => !IsAligned;
    public bool HasPolarity => Polarity == Polarity.Aligned || Polarity == Polarity.Inverted;
    public virtual bool IsPolarityEqual(Number num) => Polarity == num.Polarity;
    public virtual bool IsDirectionEqual(Number num) => PositiveTickDirection == num.PositiveTickDirection;
    public static Number operator ~(Number value) => value.InvertPolarityAndDirection();
    public Number InvertPolarity() => new(Domain, Focal, Polarity.Invert());
    public Number InvertDirection() => new(Domain, Focal.FlipAroundFirst(), Polarity);
    public Number InvertPolarityAndDirection() => new(Domain, Focal.FlipAroundFirst(), Polarity.Invert());
    public Number Reverse() => new(Domain, Focal.Reverse(), Polarity);
    public Number Negate()
    {
        var offset = Domain.BasisFocal.FirstTick;
        return new(Domain, new Focal(
            offset - (FirstTick - offset), 
            offset - (LastTick - offset)), Polarity);
    }

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
            l0 = -(left.FirstTick - offset);
            l1 = (left.LastTick - offset);
        }
        else
        {
            l0 = -(left.LastTick - offset);
            l1 = (left.FirstTick - offset);
        }

        if (reciprocal == true && left.Polarity == Polarity.Aligned && right.Polarity == Polarity.Inverted)
        {
            r0 = -(right.LastTick - offset);
            r1 = (right.FirstTick - offset);
        }
        else
        {
            r0 = -(right.FirstTick - offset);
            r1 = (right.LastTick - offset);
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

        var l0 = (left.FirstTick - offset);
        var l1 = (left.LastTick - offset);
        long r0, r1;
        if (left.Polarity == right.Polarity)
        {
            r0 = (right.FirstTick - offset);
            r1 = (right.LastTick - offset);
        }
        else
        {
            r0 = (right.LastTick - offset);
            r1 = (right.FirstTick - offset);
        }

        return (l0, l1, r0, r1);
    }
    #endregion

    #region Ranges
    //public double DecimalValue(long tick) => Domain.DecimalValue(tick);
    public long TickValue(double value) => Domain.TickValue(value);

    public PRange GetRange()
    {
        var basis = Domain.BasisFocal;
        var len = (double)basis.NonZeroTickLength;// * Polarity.ForceValue(); //AlignedNonZeroLength(isAligned);// 
        var start = (FirstTick - basis.FirstTick) / len;
        var end = (LastTick - basis.FirstTick) / len;
        if (Domain.BasisIsReciprocal)
        {
            start = Math.Round(start) * Math.Abs(len);
            end = Math.Round(end) * Math.Abs(len);
        }
        var result = IsAligned ? new PRange(-start, end, IsAligned) : new PRange(start, -end, !IsAligned);
        return result;
    }
    public Number ValueAtT(double startT, double endT)
    {
        return new(Domain, Focal.FocalFromTs(startT, endT, IsInverted), Polarity);
    }

    #endregion
    #region Equality
    public bool IsZero => FirstTick == Domain.BasisFocal.FirstTick && LastTick == Domain.BasisFocal.FirstTick;
    public bool IsOne => FirstTick == Domain.BasisFocal.FirstTick && LastTick == Domain.BasisFocal.LastTick;
    public Number Clone() => new Number(Domain, Focal.Clone(), Polarity);
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
            result = $"{pol}({start}s{midSign}{end}e)";
        }
        return result;
    }
}
