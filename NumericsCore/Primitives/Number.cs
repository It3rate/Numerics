using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
//IMinMaxValue<Number>

{
    public Focal Focal => _focal;
    private Focal _focal;
    public Domain Domain => _domain;
    private Domain _domain;
	public bool Polarity { get; set; } = true;
    public long StartTick { get => _focal.StartTick; set => _focal.StartTick = value; }
    public long EndTick { get => _focal.EndTick; set => _focal.EndTick = value; }

    public long TickLength => _focal.TickLength;

    public long AbsTickLength => _focal.AbsTickLength;

	public Number(Domain domain, Focal focal, bool polarity = true)
    {
        _domain = domain;
        _focal = focal;
		Polarity = polarity;
	}
	#region Add
	public static Number Add(Number left, Number right) => left + right;
	public static Number operator +(Number left, Number right)
	{
		var num = left.Domain.ConvertNumber(right);
        return new(left._domain, left._focal + num._focal);
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
        var num = left.Domain.ConvertNumber(right);
        return new(left._domain, left._focal - num._focal);
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
		var startTick = left.Focal.StartTick;
        var leftOffset = left.Focal.GetOffset(-startTick);
        var rightOffset = aligned.Focal.GetOffset(-startTick);
		var product = leftOffset * rightOffset;
        return new(left._domain, product.GetOffset(startTick));
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
        var startTick = left.Focal.StartTick;
        var leftOffset = left.Focal.GetOffset(-startTick);
        var rightOffset = aligned.Focal.GetOffset(-startTick);
        var product = leftOffset / rightOffset;
        return new(left._domain, product.GetOffset(startTick));
    }

    #endregion

    #region Conversions
    public double DecimalValue(long tick) => _domain.DecimalValue(tick);
    public long TickValue(double value) => _domain.TickValue(value);
    #endregion
    public Number Clone()
	{
		return new Number(_domain, _focal.Clone(), Polarity);
	}
}
