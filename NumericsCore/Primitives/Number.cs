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
    public Focal Focal => _focal;
    private Focal _focal;
    public Domain Domain => _domain;
    private Domain _domain;
	public Polarity Polarity { get; set; }
    public double StartValue => -_domain.DecimalValue(EndTick);
    public double EndValue => _domain.DecimalValue(StartTick);
    public long StartTick 
    {
        get => IsAligned ? Focal.LastTick : Focal.FirstTick;
        set { if (IsAligned) { Focal.LastTick = value; } else { Focal.FirstTick = value; } }
    }
    public long EndTick
    {
        get => IsAligned ? Focal.FirstTick : Focal.LastTick;
        set { if (IsAligned) { Focal.FirstTick = value; } else { Focal.LastTick = value; } }
    }

    public long TickLength => _focal.TickLength;

    public long AbsTickLength => _focal.AbsTickLength;

    // IsFractional, IsInverted, IsNegative, IsNormalized, IsZero, IsOne, IsZeroStart, IsPoint, IsOverflow, IsUnderflow
    // IsLessThanBasis, IsGrowable, IsBasisLength, IsMin, HasMask, IsArray, IsMultiDim, IsCalculated, IsRandom
    // Domain: IsTickLessThanBasis, IsBasisInMinmax, IsTiling, IsClamping, IsInvertable, IsNegateable, IsPoly, HasTrait
    // scale

    public Number(Domain domain, Focal focal, Polarity polarity = Polarity.Aligned)
    {
        _domain = domain;
        _focal = focal;
		Polarity = polarity;
	}
	#region Add
	public static Number Add(Number left, Number right) => left + right;
	public static Number operator +(Number left, Number right)
	{
		var convertedRight = left.Domain.ConvertNumber(right);
        var offset = left._domain.BasisFocal.FirstTick;
        var result = left.Clone();
        result.EndTick += convertedRight.EndTick - offset;
        result.StartTick += convertedRight.StartTick - offset;
        return result;
    }
	static Number IAdditionOperators<Number, Number, Number>.operator +(Number left, Number right) => left + right;

	public static Number operator ++(Number value) => new(value._domain, value._focal++);
	public static Number operator +(Number value) => new(value._domain, value._focal);
	public Number AdditiveIdentity => _domain.AdditiveIdentity;

	#endregion
	#region Subtract
	public static Number Subtract(Number left, Number right) => left - right;
	public static Number operator -(Number left, Number right)
    {
        var convertedRight = left.Domain.ConvertNumber(right);
        var offset = left._domain.BasisFocal.FirstTick;
        var result = left.Clone();
        result.EndTick = (left.EndTick - convertedRight.EndTick) + offset;
        result.StartTick = (left.StartTick - convertedRight.StartTick) + offset;
        return result;

        //var offset = left._domain.BasisFocal.StartTick;
        //return new(left._domain, new Focal(
        //    (left._focal.StartTick - num._focal.StartTick) + offset,
        //    (left._focal.EndTick - num._focal.EndTick) + offset
        //    ));
    }
    static Number ISubtractionOperators<Number, Number, Number>.operator -(Number left, Number right) => left - right;

	public static Number operator --(Number value) => new(value._domain, value._focal--);
	public static Number operator -(Number value) => new(value._domain, -value._focal);
	#endregion
	#region Multiply
	public static Number Multiply(Number left, Number right)
	{
		return left * right;
	}
	public static Number operator *(Number left, Number right)
    {
        var aligned = left.Domain.ConvertNumber(right);
        var len = (double)left.Domain.BasisFocal.TickLength;
        var offset = left.Domain.BasisFocal.FirstTick;
        var leftOffset = left.Focal.GetOffset(-offset);
        var rightOffset = aligned.Focal.GetOffset(-offset);

        var result = new Number(left.Domain,new (
            (long)((leftOffset.FirstTick * rightOffset.LastTick + leftOffset.LastTick * rightOffset.FirstTick) / len) + offset,
            (long)((leftOffset.LastTick * rightOffset.LastTick - leftOffset.FirstTick * rightOffset.FirstTick) / len) + offset),
            left.Polarity);

        return result.SolvePolarityWith(right);
    }

	static Number IMultiplyOperators<Number, Number, Number>.operator *(Number left, Number right) => left * right;
	public Number MultiplicativeIdentity => _domain.MultiplicativeIdentity;

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
        var result =  new Number(left.Domain, focalResult.GetOffset(offset), left.Polarity);
        return result.SolvePolarityWith(right);
    }
    #endregion

    #region Polarity
    public int PolarityDirection => IsAligned ? 1 : IsInverted ? -1 : 0;
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
        else
        {
            result = Clone();
        }
        return result;
    }

    #endregion

    #region Conversions
    public double DecimalValue(long tick) => _domain.DecimalValue(tick);
    public long TickValue(double value) => _domain.TickValue(value);
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
        var vStart = Domain.DecimalValue(Focal.FirstTick);
        var vEnd = Domain.DecimalValue(Focal.LastTick);
        if (Polarity == Polarity.None)
        {
            result = $"x({vStart:0.##}_{-vEnd:0.##})"; // no polarity, so just list values
        }
        else
        {
            var midSign = vEnd > 0 ? " + " : " ";
            result = IsAligned ?
                $"({vStart:0.##}i{midSign}{vEnd:0.##}r)" :
                $"~({vEnd:0.##}r{midSign}{vStart:0.##}i)";
        }
        return result;
    }
}
