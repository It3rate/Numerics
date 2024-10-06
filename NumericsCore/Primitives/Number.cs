using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NumericsCore.Interfaces;
using NumericsCore.Primitives;
using NumericsCore.Sequencer;
using NumericsCore.Expressions;
using NumericsCore.Utils;

namespace Numerics.Primitives;


public class Number :
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
    private Focal? _basis;

    public Number BasisNumber { get; }
    public Focal Focal { get; }
    public long TickSize { get; protected set; } = 1;
    public Domain Domain => _tlDomain ?? BasisNumber.Domain;
    public Focal BasisFocal => BasisNumber.Focal;
    public double StartValue
    {
        get
        {
            if (StartLandmark != null)
            {
                EnsureLandmarks();
            }
            return -RawTickValue(Focal.StartTick);
        }
    }
    public double EndValue
    {
        get
        {
            if (EndLandmark != null)
            {
                EnsureLandmarks();
            }
            return RawTickValue(Focal.EndTick);
        }
    }
    // allow start and end landmarks to adjust the focals when reference changes.
    private IValueRef? StartLandmark;
    private IValueRef? EndLandmark;
    public long StartTick => Focal.StartTick;
    public long EndTick => Focal.EndTick;

    private Number? _inverse;
    public Number Inverse
    {
        get
        {
            if (_inverse == null)
            {
                _inverse = new Number(BasisNumber, Focal.BasisInverse);
            }
            return _inverse!;
        }
    }

    public Number(Number basisNumber, Focal focal)
    {
        BasisNumber = basisNumber;
        Focal = focal;
        _history.Add(new(_curMS, StartValue, EndValue));
    }
    public Number(Number basisNumber, IValueRef startLandmark, IValueRef endLandmark)
    {
        BasisNumber = basisNumber;
        StartLandmark = startLandmark;
        EndLandmark = endLandmark;
        Focal = new Focal(0, 0);
        EnsureLandmarks();
    }

    private Domain? _tlDomain;
    private Number(Domain domain, Focal basisFocal)
    {
        _tlDomain = domain;
        Focal = basisFocal;
        BasisNumber = this;
    }
    public static Number CreateDomainNumber(Domain domain, Focal focal)
    {
        var basisNumber = new Number(domain, focal);
        return basisNumber;
    }

    #region Mutations
    private long _curMS => Runner.Instance.CurrentMS;
    private List<ValueAtTime> _history = new System.Collections.Generic.List<ValueAtTime>();
    public ValueAtTime? CurrentHistoricalValue() => _history.Count > 0 ? _history[_history.Count - 1] : null;
    public ValueAtTime? PreviousHistoricalValue() => _history.Count > 1 ? _history[_history.Count - 2] : null;
    public bool SetValues(double startValue, double endValue)
    {
        // allow changes by optionally recording old values and timestamping. This allows history to be preserved for trend analysis, and rewind. Need not be perfect (forgetting allowed)
        // maybe count accesses as well as changes if that helps understanding. Or access with an expected value to create defaults and understand differences.
        var result = true;
        if (StartLandmark != null || EndLandmark != null)
        {
            result = false; // only raw values controlled by this instance can be set, not references.
        }
        else
        {
            var startTick = TickValueInverted(startValue);
            var endTick = TickValueAligned(endValue);
            if (startTick != Focal.StartTick || endTick != Focal.EndTick)
            {
                Focal.StartTick = startTick;
                Focal.EndTick = endTick;
                _history.Add(new(_curMS, StartValue, EndValue));
            }
        }
        return result;
    }
    public bool SetStartValue(double startValue)
    {
        var result = true;
        if (StartLandmark != null)
        {
            result = false;
        }
        else
        {
            var tick = TickValueInverted(startValue);
            if (tick != Focal.StartTick)
            {
                Focal.StartTick = tick;
                _history.Add(new(_curMS, StartValue, EndValue));
            }
        }
        return result;
    }
    public bool SetEndValue(double endValue)
    {
        var result = true;
        if (EndLandmark != null)
        {
            result = false;
        }
        else
        {
            var tick = TickValueAligned(endValue);
            if (tick != Focal.EndTick)
            {
                Focal.EndTick = tick;
                _history.Add(new(_curMS, StartValue, EndValue));
            }
        }
        return result;
    }
    //public void IncrementStartTick() => Focal.StartTick += BasisDirection;
    //public void DecrementStartTick() => Focal.StartTick -= BasisDirection;
    //public void IncrementEndTick() => Focal.EndTick += BasisDirection;
    //public void DecrementEndTick() => Focal.EndTick -= BasisDirection;
    private void EnsureLandmarks()
    {
        if (StartLandmark != null && EndLandmark != null) // both must be set
        {
            if (StartLandmark.NeedsUpdate)
            {
                Focal.StartTick = TickValueInverted(StartLandmark.Value);
                //StartLandmark.NeedsUpdate = false;
            }
            if (EndLandmark.NeedsUpdate)
            {
                Focal.EndTick = TickValueAligned(EndLandmark.Value);
                //EndLandmark.NeedsUpdate = false;
            }
        }
    }
    public double AsBasisTValue(double t) => (EndValue - StartValue) * t + StartValue; // number is basis, so 0 is startValue, 1 is endValue.
    public PRange GetRange() => GetRange(this);
    #endregion
    #region Truths
    public bool IsZero => StartTick == BasisFocal.StartTick && EndTick == BasisFocal.StartTick;
    public bool IsOne => StartTick == BasisFocal.StartTick && EndTick == BasisFocal.EndTick;
    public bool IsNegativeDirection => Focal.Direction * BasisDirection == -1;
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
    public Polarity Polarity => BasisFocal.Polarity;
    public int BasisDirection => BasisFocal.Direction;

    public long TickLength => Focal.Length;
    public long AbsTickLength => Focal.AbsLength;
    public long BasisLength => BasisFocal.Length;
    public long AbsBasisLength => BasisFocal.AbsLength;
    public bool BasisIsReciprocal => Math.Abs(TickSize) > BasisFocal.AbsLength && BasisFocal.AbsLength != 0;
    public Number Length => new Number(Domain, new(0, Focal.Length));
    public Number StartPortion => new Number(Domain, new(0, Focal.StartTick));
    public Number EndPortion => new Number(Domain, new(0, Focal.EndTick));
    #endregion

    #region Funcs

    public static Func<Number, Number, Number> ADD = (left, rightIn) =>
    {
        var right = left.MapToDomain(rightIn);
        var (leftStart, leftEnd) = left.RawTicksFromZero();
        var (rightStart, rightEnd) = right.RawTicksFromZero();
        var bf = left.BasisFocal;
        left.Focal.StartTick = bf.StartTick + (leftStart + rightStart);
        left.Focal.EndTick = bf.StartTick + (leftEnd + rightEnd);
        return left;
    };
    public static Func<Number, Number, Number> SUBTRACT = (left, rightIn) =>
    {
        var right = left.MapToDomain(rightIn);
        var (leftStart, leftEnd) = left.RawTicksFromZero();
        var (rightStart, rightEnd) = right.RawTicksFromZero();
        var bf = left.BasisFocal;
        left.Focal.StartTick = bf.StartTick + (leftStart - rightStart);
        left.Focal.EndTick = bf.StartTick + (leftEnd - rightEnd);
        return left;
    };
    public static Func<Number, Number, Number> MULTIPLY = (left, rightIn) =>
    {
        var right = left.MapToDomain(rightIn);
        var (leftStart, leftEnd) = left.SignedTicksFromZero();
        var (rightStart, rightEnd) = right.SignedTicksFromZero();
        var iVal = leftStart * rightEnd + leftEnd * rightStart;
        var rVal = leftEnd * rightEnd - leftStart * rightStart;
        var len = left.AbsBasisLength;
        var bf = left.BasisFocal;
        left.Focal.StartTick = bf.StartTick - (iVal / len) * bf.Direction;
        left.Focal.EndTick = bf.StartTick + (rVal / len) * bf.Direction;
        return left;
    };
    public static Func<Number, Number, Number> DIVIDE = (left, rightIn) =>
    {
        var right = left.MapToDomain(rightIn);
        var (leftStart, leftEnd) = left.SignedTicksFromZero();
        var (rightStart, rightEnd) = right.SignedTicksFromZero();

        long iVal;
        long rVal;
        var absLen = left.AbsBasisLength;
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
        var bf = left.BasisFocal;
        left.Focal.StartTick = bf.StartTick - iVal * bf.Direction;
        left.Focal.EndTick = bf.StartTick + rVal * bf.Direction;
        return left;
    };
    public static Func<Number, Number, Number> POW => (Number value, Number power) =>
    {
        if (power.IsZero || value.IsZero)
        {
            return value.One;
        }
        // todo: this is temp. Correct polarity, use binomial, account for resolution
        var v = value.GetRange();
        var p = power.GetRange();
        var presult = PRange.Pow(v, p);
        var result = presult.ToNumber(value);
        value.Focal.StartTick = result.StartTick;
        value.Focal.EndTick = result.EndTick;
        return value;
        //double valueReal = value.m_real;
        //double valueImaginary = value.m_imaginary;
        //double powerReal = power.m_real;
        //double powerImaginary = power.m_imaginary;

        //double rho = Abs(value);
        //double theta = Math.Atan2(valueImaginary, valueReal);
        //double newRho = powerReal * theta + powerImaginary * Math.Log(rho);

        //double t = Math.Pow(rho, powerReal) * Math.Pow(Math.E, -powerImaginary * theta);

        //return new Number(t * Math.Cos(newRho), t * Math.Sin(newRho));
    };

    public static Func<Number, Number> PLUS_PLUS = (left) =>
    {
        left.Focal.Add(left.Domain.DefaultBasisNumber.Focal);
        return left;
    };

    public static Func<Number, Number> MINUS_MINUS = (left) =>
    {
        left.Focal.Subtract(left.Domain.DefaultBasisNumber.Focal);
        return left;
    };
    public static Func<Number, Number> PLUS = (left) => { return left; };
    public static Func<Number, Number> MINUS = (left) => { left.Focal.Negate(); return left; };
    //public static Func<Number, Number> INVERT = (left) => { var temp = left.StartTick; left.StartTick = left.EndTick; left.EndTick = temp; return left; };
    #endregion

    #region Add
    public Number Add(Number right) => ADD(this, right);
    public static Number operator +(Number left, Number right) => ADD(left.Clone(), right);
    public static Number Add(Number left, Number right) => ADD(left.Clone(), right);
    static Number IAdditionOperators<Number, Number, Number>.operator +(Number left, Number right) => ADD(left.Clone(), right);

    public Number Increment() => PLUS_PLUS(this);
    public Number IncrementEndTick() { Focal.EndTick += BasisDirection; return this; }
    public Number Plus() => PLUS(this);
    public static Number operator +(Number value) => PLUS(value);
    public Number PlusPlus() => PLUS_PLUS(this);
    public static Number operator ++(Number value) => PLUS_PLUS(value);

    #endregion
    #region Subtract
    public Number Subtract(Number right) => SUBTRACT(this, right);
    public static Number operator -(Number left, Number right) => SUBTRACT(left.Clone(), right);
    public static Number Subtract(Number left, Number right) => SUBTRACT(left.Clone(), right);
    static Number ISubtractionOperators<Number, Number, Number>.operator -(Number left, Number right) => SUBTRACT(left.Clone(), right);

    public Number Decrement() => MINUS_MINUS(this);
    public Number DecrementEndTick() { Focal.EndTick -= BasisDirection; return this; }

    public Number Minus() => MINUS(this);
    public static Number operator -(Number value) => MINUS(value);
    public Number MinusMinus() => MINUS_MINUS(this);
    public static Number operator --(Number value) => MINUS_MINUS(value);
    #endregion
    #region Multiply
    public Number Multiply(Number right) => MULTIPLY(this, right);
    public static Number operator *(Number left, Number right)  => MULTIPLY(left.Clone(), right);
    public static Number Multiply(Number left, Number right) => MULTIPLY(left.Clone(), right);
    static Number IMultiplyOperators<Number, Number, Number>.operator *(Number left, Number right) => MULTIPLY(left.Clone(), right);

    #endregion
    #region Divide
    public Number Divide(Number right) => DIVIDE(this, right);
    public static Number Divide(Number left, Number right) => DIVIDE(left.Clone(), right);
    public static Number operator /(Number left, Number right) => DIVIDE(left.Clone(), right);
    #endregion
    #region Pow
    public Number Pow(Number power) => POW(this, power);
    public static Number operator ^(Number value, Number power) => POW(value.Clone(), power);
	public static Number Pow(Number value, Number power) => POW(value.Clone(), power);
	public Number Squared() => this * this;
	public Number Sqrt() => throw new NotImplementedException();
	#endregion
	#region Polarity
	public bool IsAligned => Polarity == Polarity.Aligned;
    public bool IsInverted => Polarity == Polarity.Inverted;
    public bool HasPolarity => Polarity.HasPolarity();
    public virtual bool IsPolarityEqual(Number num) => Polarity == num.Polarity;

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

    #region Identities
    public Number AdditiveIdentity => new Number(BasisNumber, new Focal(BasisFocal.StartTick, BasisFocal.StartTick));
    public Number MultiplicativeIdentity => new Number(BasisNumber, BasisFocal);
    public Number Zero => new(BasisNumber, new Focal(BasisFocal.StartTick, BasisFocal.StartTick));
    public Number One => new(BasisNumber, BasisFocal.Clone());
    public Number MinusOne => new(BasisNumber, BasisFocal.CloneToBasisInverse());
    public Number One_i => new(BasisNumber, BasisFocal.CloneToBasisInverse().Invert());
    public Number MinusOne_i => new(BasisNumber, BasisFocal.InvertClone());
	public Number Half => new(BasisNumber, new Focal(BasisFocal.StartTick, BasisFocal.EndTick / 2));
	public Number Two => new(BasisNumber, new Focal(BasisFocal.StartTick, BasisFocal.EndTick * 2));
	#endregion
	#region Transforms
	public static Number operator ~(Number value) => value.MirrorStart();
    public Number Negate()
    {
        var (startTicks, endTicks) = RawTicksFromZero();
        return new (BasisNumber, new(-startTicks, -endTicks));
    }
    public Number Reverse()
    {
        var (startTicks, endTicks) = RawTicksFromZero();
        return new(BasisNumber, new(endTicks, startTicks));
    }
    public Number ReverseNegate()
    {
        var (startTicks, endTicks) = RawTicksFromZero();
        return new(BasisNumber, new(-endTicks, -startTicks));
    }
    public Number Invert() => new(BasisNumber, Focal.InvertClone());
    public Number InvertNegate() => new(BasisNumber, Focal.InvertClone().Negate());
    public Number MirrorStart() // inverted Conjugate
    {
        var (startTicks, endTicks) = RawTicksFromZero();
        return new(BasisNumber, new(-startTicks, endTicks));
    }
    public Number MirrorEnd() // aligned conjugate
    {
        var (startTicks, endTicks) = RawTicksFromZero();
        return new(BasisNumber, new(startTicks, -endTicks));
    }

    private long TicksFromZero(long tick) => tick - BasisFocal.StartTick;
    private long TicksFromZeroDirected(long tick) => (tick - BasisFocal.StartTick) * BasisDirection;
    private (long, long) RawTicksFromZero() => (TicksFromZero(StartTick), TicksFromZero(EndTick));
    private (long, long) SignedTicksFromZero() => (-TicksFromZeroDirected(StartTick), TicksFromZeroDirected(EndTick));
    #endregion
    #region Conversions
    public long TickValueAligned(double value)
    {
        var result = (long)(BasisFocal.StartTick + (value * BasisFocal.Length));
        // todo: Clamp to limits, account for basis direction.
        return result;
    }
    public long TickValueInverted(double value)
    {
        var result = (long)(BasisFocal.StartTick - (value * BasisFocal.Length));
        // todo: Clamp to limits, account for basis direction.
        return result;
    }
    public Focal FocalFromDecimalRaw(double startValue, double endValue) =>
    new Focal(TickValueAligned(startValue), TickValueAligned(endValue));
    public Focal FocalFromDecimalSigned(double startValue, double endValue) =>
        new Focal(TickValueInverted(startValue), TickValueAligned(endValue));

    public (double, double) RawValues(Number num)
    {
        var absLen = (double)num.BasisFocal.AbsLength;
        if (absLen == 0)
        {
            return (0, 0);
        }
        else
        {
            return( TicksFromZeroDirected(num.StartTick) / (double)num.BasisFocal.AbsLength,
                    TicksFromZeroDirected(num.EndTick) / (double)num.BasisFocal.AbsLength);
        }
    }
    public (double, double) SignedValues(Number num)
    {
        var absLen = (double)num.BasisFocal.AbsLength;
        if (absLen == 0)
        {
            return (0, 0);
        }
        else
        {
            return (-TicksFromZeroDirected(num.StartTick) / (double)num.BasisFocal.AbsLength,
                     TicksFromZeroDirected(num.EndTick) / (double)num.BasisFocal.AbsLength);
        }
    }
    public PRange GetRange(Number num)
    {
        var (start, end) = SignedValues(num);
        if (BasisIsReciprocal)
        {
            start = Math.Round(start) * num.BasisFocal.AbsLength;
            end = Math.Round(end) * num.BasisFocal.AbsLength;
        }
        return new PRange(start, end, num.Polarity);
    }
    public double RawTickValue(long tick) => TicksFromZeroDirected(tick) / (double)BasisFocal.AbsLength;


    public double AlignedValueAtT(Number num, double t)
    {
        double result;
        if (t == 0)
        {
            result = RawTickValue(num.StartTick);
        }
        else if (t == 1)
        {
            result = RawTickValue(num.EndTick);
        }
        else
        {
            var start = RawTickValue(num.StartTick);
            var end = RawTickValue(num.EndTick);
            result = (end - start) * t + start;
        }
        return result;
    }

    public Number MapToDomain(Number value)
    {
        Number result = value;
        if (value.BasisNumber != BasisNumber)
        {
            if (value.AbsBasisLength != 0)
            {
                var ratio = AbsBasisLength / (double)value.AbsBasisLength;
                var (valueStart, valueEnd) = value.RawTicksFromZero();
                var start = (long)(valueStart * ratio);
                var end = (long)(valueEnd * ratio);
                result = new(BasisNumber, new(BasisFocal.StartTick + start, BasisFocal.StartTick + end));
            }
            else
            {
                result = new(BasisNumber, value.Focal.Clone());
            }
        }
        return result;
    }
    #endregion

    #region Equality
    public Number Clone() => new Number(BasisNumber, Focal.Clone());
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
                Focal.Equals(this.Focal, value.Focal) &&
                BasisNumber.Focal == value.BasisNumber.Focal && // want to avoid giant recursion on equality tests, as there is a basisNumber heirarchy
                TickSize == value.TickSize
                );
    }
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = BasisNumber.GetHashCode() * 13 + Focal.GetHashCode() * 17 ^ ((int)Polarity + 27) * 397 + (int)TickSize * 31;// + (IsValid ? 77 : 33);
            return hashCode;
        }
    }
	#endregion

	public static readonly Number SCALAR_ZERO = new Number(Domain.SCALAR_DOMAIN, Focal.Zero);
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
