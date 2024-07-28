﻿using System;
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
    private long _iTick //=> IsAligned ? FirstTick : LastTick;
    {
        get => IsAligned? FirstTick : LastTick;
        set { if (IsAligned) { FirstTick = value; } else { LastTick = value; } }
    }
    private long _rTick //=> IsAligned ? LastTick : FirstTick;
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
	public static Number Add(Number left, Number right) => left + right;
	public static Number operator +(Number left, Number right)
	{
		var convertedRight = left.Domain.AlignedDomain(right);
        var offset = left.Domain.BasisFocal.FirstTick;
        var dir = left.Domain.BasisFocal.NonZeroDirection;
        var result = left.Clone();
        result._iTick += convertedRight._iTick * dir - offset;
        result._rTick += convertedRight._rTick * dir - offset;
        return result;
    }
	static Number IAdditionOperators<Number, Number, Number>.operator +(Number left, Number right) => left + right;

	public static Number operator ++(Number value) => new(value.Domain, value.Focal++);
	public static Number operator +(Number value) => new(value.Domain, value.Focal);
	public Number AdditiveIdentity => Domain.AdditiveIdentity;

	#endregion
	#region Subtract
	public static Number Subtract(Number left, Number right) => left - right;
	public static Number operator -(Number left, Number right)
    {
        var convertedRight = left.Domain.AlignedDomain(right);
        var offset = left.Domain.BasisFocal.FirstTick;
        var dir = left.Domain.BasisFocal.NonZeroDirection;
        var result = left.Clone();
        result._iTick = (left._iTick - convertedRight._iTick * dir) + offset;
        result._rTick = (left._rTick - convertedRight._rTick * dir) + offset;
        return result;

        //var offset = left.Domain.BasisFocal.StartTick;
        //return new(left.Domain, new Focal(
        //    (left.Focal.StartTick - num.Focal.StartTick) + offset,
        //    (left.Focal.EndTick - num.Focal.EndTick) + offset
        //    ));
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
        var (l0, l1, r0, r1) = GetScalarFocals(left, aligned, offset, false);

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

    //public static Number operator *(Number left, Number right)
    //{
    //    var aligned = left.Domain.ConvertNumber(right);
    //    var dir = left.Domain.BasisFocal.NonZeroDirection;
    //    var len = (double)left.Domain.BasisFocal.TickLength;
    //    var offset = left.Domain.BasisFocal.FirstTick;
    //    var leftOffset = left.Focal.GetOffset(-offset);
    //    if (left.IsInverted)
    //    {
    //        leftOffset = leftOffset.Reverse(); // todo: clarify this in code.
    //    }
    //    var rightOffset = aligned.Focal.GetOffset(-offset);

    //    var raw = new Number(left.Domain, new(
    //        (long)((leftOffset.FirstTick * rightOffset.LastTick + leftOffset.LastTick * rightOffset.FirstTick) / len),
    //        (long)((leftOffset.LastTick * rightOffset.LastTick - leftOffset.FirstTick * rightOffset.FirstTick) / len)),
    //        left.Polarity);

    //    Number? result = raw.SolvePolarityWith(right);
    //    if (left.IsInverted)
    //    {
    //        result = new Number(result.Domain, result.Focal.Negate(), result.Polarity);// todo: clarify this in code.
    //    }
    //    result = new Number(result.Domain, result.Focal.GetOffset(offset), result.Polarity);
    //    return result;
    //}

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

        var (l0, l1, r0, r1) = GetScalarFocals(left, aligned, offset, true);
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
    #region Polarity
    public int PolarityDirection => Polarity.Direction();
    public bool IsAligned => Polarity == Polarity.Aligned;
    public bool HasPolarity => Polarity == Polarity.Aligned || Polarity == Polarity.Inverted;
    public bool IsInverted => !IsAligned;
    public int Direction => Domain.BasisFocal.Direction * PolarityDirection;
    public static Number operator ~(Number value) => value.InvertPolarityAndDirection();
    public Number InvertPolarity() => new(Domain, Focal, Polarity.Invert());
    public Number InvertDirection() => new(Domain, Focal.FlipAroundFirst(), Polarity);
    public Number InvertPolarityAndDirection() => new(Domain, Focal.FlipAroundFirst(), Polarity.Invert());

    private static (long, long, long, long) GetScalarFocals(Number left, Number right, long offset, bool reciprocal)
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
    #endregion

    #region Conversions
    //public double DecimalValue(long tick) => Domain.DecimalValue(tick);
    public long TickValue(double value) => Domain.TickValue(value);

    public PRange GetRange()//Focal numFocal, Focal basis, bool isReciprocal, bool isAligned)
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
        //start = IsAligned ? -start : start;
        //end = IsAligned ? end : -end;
        var result = IsAligned ? new PRange(-start, end, IsAligned) : new PRange(start, -end, !IsAligned);
        return result;
    }

    #endregion
    #region Equality
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
