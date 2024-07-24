using System;
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
    public long StartTick
    {
        get => Focal.FirstTick; // IsAligned ? Focal.FirstTick : Focal.LastTick;
        set => Focal.FirstTick = value; // { if (IsAligned) { Focal.FirstTick = value; } else { Focal.LastTick = value; } }
    }
    public long EndTick 
    {
        get => Focal.LastTick; // IsAligned ? Focal.LastTick : Focal.FirstTick;
        set => Focal.LastTick = value; // { if (IsAligned) { Focal.LastTick = value; } else { Focal.FirstTick = value; } }
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
		var convertedRight = left.Domain.ConvertNumber(right);
        var offset = left.Domain.BasisFocal.FirstTick;
        var dir = left.Domain.BasisFocal.NonZeroDirection;
        var result = left.Clone();
        result.StartTick += convertedRight.StartTick * dir - offset;
        result.EndTick += convertedRight.EndTick * dir - offset;
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
        var convertedRight = left.Domain.ConvertNumber(right);
        var offset = left.Domain.BasisFocal.FirstTick;
        var dir = left.Domain.BasisFocal.NonZeroDirection;
        var result = left.Clone();
        result.StartTick = (left.StartTick - convertedRight.StartTick * dir) + offset;
        result.EndTick = (left.EndTick - convertedRight.EndTick * dir) + offset;
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
        var aligned = left.Domain.ConvertNumber(right);
        var dir = left.Domain.BasisFocal.NonZeroDirection;
        var len = (double)left.Domain.BasisFocal.TickLength;
        var offset = left.Domain.BasisFocal.FirstTick;
        var leftOffset = left.Focal.GetOffset(-offset);
        var rightOffset = aligned.Focal.GetOffset(-offset);

        var raw = new Number(left.Domain,new (
            (long)((leftOffset.FirstTick * rightOffset.LastTick + leftOffset.LastTick  * rightOffset.FirstTick) / len),
            (long)((leftOffset.LastTick  * rightOffset.LastTick - leftOffset.FirstTick * rightOffset.FirstTick) / len)),
            left.Polarity);

        Number? result = raw.SolvePolarityWith(right);
        return new Number(result.Domain, result.Focal.GetOffset(offset), result.Polarity);
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
        var aligned = left.Domain.ConvertNumber(right);

        Focal focalResult;
        var offset = left.Domain.BasisFocal.FirstTick;
        var len = left.Domain.BasisFocal.TickLength;
        var polarity = left.Polarity.SolvePolarity(right.Polarity);
        double leftEnd = left.Focal.LastTick - offset;
        double leftStart = left.Focal.FirstTick - offset;
        double rightEnd = aligned.Focal.LastTick - offset;
        double rightStart = aligned.Focal.FirstTick - offset;
        if (Math.Abs(rightStart) < Math.Abs(rightEnd))
        {
            double num = rightStart / rightEnd;
            focalResult = new Focal(
                (long)(((leftStart - leftEnd * num) / (rightEnd + rightStart * num)) * len),
                (long)(((leftEnd + leftStart * num) / (rightEnd + rightStart * num)) * len));
        }
        else
        {
            double num1 = rightEnd / rightStart;
            focalResult = new Focal(
                (long)(((-leftEnd + leftStart * num1) / (rightStart + rightEnd * num1)) * len),
                (long)(((leftStart + leftEnd * num1) / (rightStart + rightEnd * num1)) * len));
        }
        var raw =  new Number(left.Domain, focalResult, left.Polarity);
        //return result.SolvePolarityWith(right);
        Number? result = raw.SolvePolarityWith(right);
        return new Number(result.Domain, result.Focal.GetOffset(offset), result.Polarity);
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
    public Number InvertDirection() => new(Domain, Focal.FlipAroundStart(), Polarity);
    public Number InvertPolarityAndDirection() => new(Domain, Focal.FlipAroundStart(), Polarity.Invert());
    public Number SolvePolarityWith(Number right)
    {
        Number result;
        if (Polarity == Polarity.Inverted && right.Polarity == Polarity.Inverted)
        {
            result = InvertPolarityAndDirection();
        }
        else if (Polarity == Polarity.Aligned && right.Polarity == Polarity.Inverted)
        {
            result = InvertPolarity();
        }
        //if (right.Polarity == Polarity.Inverted)
        //{
        //    result = InvertPolarity();
        //}
        else
        {
            result = Clone();
        }
        return result;
    }

    #endregion

    #region Conversions
    //public double DecimalValue(long tick) => Domain.DecimalValue(tick);
    public long TickValue(double value) => Domain.TickValue(value);

    public PRange GetRange()//Focal numFocal, Focal basis, bool isReciprocal, bool isAligned)
    {
        var basis = Domain.BasisFocal;
        var len = (double)basis.NonZeroTickLength * Polarity.ForceValue(); //AlignedNonZeroLength(isAligned);// 
        var start = (StartTick - basis.FirstTick) / len;
        var end = (EndTick - basis.FirstTick) / len;
        if (Domain.BasisIsReciprocal)
        {
            start = Math.Round(start) * Math.Abs(len);
            end = Math.Round(end) * Math.Abs(len);
        }
        //start = IsAligned ? -start : start;
        //end = IsAligned ? end : -end;
        var result = new PRange(-start, end, IsAligned);
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
        //var vStart = Domain.DecimalValue(Focal.FirstTick);
        //var vEnd = Domain.DecimalValue(Focal.LastTick);
        if (Polarity == Polarity.None)
        {
            result = $"x({val.Start:0.##}_{val.End:0.##})"; // no polarity, so just list values
        }
        else
        {
            var midSign = val.End > 0 ? " + " : " ";
            result = IsAligned ?
                $"({val.Start:0.##}s{midSign}{val.End:0.##}e)" :
                $"~({val.End:0.##}s{midSign}{val.Start:0.##}e)";
        }
        return result;
    }
}
