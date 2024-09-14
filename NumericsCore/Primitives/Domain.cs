
using System;
using NumericsCore.Primitives;
using NumericsCore.Utils;

namespace Numerics.Primitives;

// this constrains the significant figures of a measurement, unit/not is the minimum tick size, and range is the max possible.
// Feels like this should just be the unit chosen - that works from min tick size, but how to specify a max value? On the trait I guess?
// the trait then needs to 'know' how measurable it is, and the units are calibrated to that, which seems overspecified
// (eg 'length' knows a nanometer is min and a light year max, cms and inches calibrate to this. Hmm, no).
// So each 'situation' has sig-fig/precision metadata. Working in metal units vs working in wood units. A metal length trait and a wood length trait, convertible.
// This is what domains are. BasisFocal size(s), min tick size, max ranges, confidence/probability data at limits of measure (gaussian precision etc).
// E.g. changing the domain 'tolerance' could change neat writing into messy.

// Min size is tick size. BasisFocal is start/end point (only one focal allowed for a unit). MinMaxFocal is bounds in ticks. todo: add conversion methods etc.
public class Domain : IEquatable<Domain>
{
    public Trait? Trait { get; }
    public Focal BasisFocal { get; }
    private Focal LimitsFocal { get; }
    private Domain(Focal basisFocal, Focal limitsFocal) : this(Trait.WorkingTrait, basisFocal, limitsFocal) { }
    public Domain(Trait? trait, Focal basisFocal, Focal limitsFocal)
    {
        Trait = trait;
        BasisFocal = basisFocal;
        LimitsFocal = limitsFocal;
    }

    #region Properties
    public Polarity Polarity => 
        BasisFocal.Direction > 0 ? Polarity.Aligned :
        BasisFocal.Direction < 0 ? Polarity.Inverted : Polarity.None;

    public Number BasisNumber => new(this, BasisFocal);
    public Number LimitsNumber => new(this, LimitsFocal);
    public long TickSize { get; protected set; } = 1;
    public long BasisLength => BasisFocal.Length;
    public long AbsBasisLength => BasisFocal.AbsLength;
    public int Direction => BasisFocal.Direction;
    public bool IsInverted => BasisFocal.Direction == -1;
    public long AbsLimitsSize => LimitsFocal.AbsLength;
    public bool BasisIsReciprocal => Math.Abs(TickSize) > BasisFocal.AbsLength;
    public double TickToBasisRatio => TickSize / BasisFocal.NonZeroTickLength;
    public bool IsZero(Number num) => num.StartTick == BasisFocal.StartTick && num.EndTick == BasisFocal.StartTick;
    public bool IsOne(Number num) => num.StartTick == BasisFocal.StartTick && num.EndTick == BasisFocal.EndTick;
    // IsTickLessThanBasis, IsBasisInMinmax, IsTiling, IsClamping, IsInvertable, IsNegateable, IsPoly, HasTrait
    #endregion
    #region Transformations
    private Domain? _inverse;
    public Domain Inverse
    {
        get
        {
            if ((_inverse == null))
            {
                _inverse = new Domain(Trait, BasisFocal.FlipAroundFirstClone(), LimitsFocal.InvertClone());
                _inverse._inverse = this;
            }
            return _inverse;        
        }
    }
    public Number MapToDomain(Number value)
    {
        Number result = value;
        if(value.Domain != this)
        {
            var ratio = AbsBasisLength / (double)value.Domain.AbsBasisLength;
            var (valueStart, valueEnd) = value.Domain.RawTicksFromZero(value);
            var start = (long)(valueStart * ratio);
            var end = (long)(valueEnd * ratio);
            result = CreateNumberRaw(start, end);
            //var start = value.StartValue; // need to change focals, otherwise the signs will change on inverting
            //var end = value.EndValue;
            //var focal = FocalFromDecimalRaw(start, end);
            //result = new(this, focal);
        }
        return result;
    }
    public static Domain CommonDomain(Domain left, Domain right)
    {
        var result = left;
        if (left != right)
        {
            var maxBasis = left.AbsBasisLength >= right.AbsBasisLength ? left.BasisFocal : right.BasisFocal;
            var maxLimits = left.AbsLimitsSize >= right.AbsLimitsSize ? left.LimitsFocal : right.LimitsFocal;
            result = new Domain(maxBasis, maxLimits);
        }
        return result;
    }
    //public double DecimalValue(long tick)
    //{
    //    var clamped =
    //        (tick < LimitsFocal.FirstTick) ? LimitsFocal.FirstTick :
    //        (tick > LimitsFocal.LastTick) ? LimitsFocal.LastTick : tick;
    //    var offset = clamped - BasisFocal.FirstTick;
    //    var result = offset / (double)BasisFocal.NonZeroTickLength;
    //    return result;
    //}

    public Number Negate(Number num)
    {
        var (startTicks, endTicks) = RawTicksFromZero(num);
        return CreateNumberRaw(-startTicks, -endTicks);
    }
    public Number Reverse(Number num)
    {
        var (startTicks, endTicks) = RawTicksFromZero(num);
        return CreateNumberRaw(endTicks, startTicks);
    }
    public Number ReverseNegate(Number num)
    {
        var (startTicks, endTicks) = RawTicksFromZero(num);
        return CreateNumberRaw(-endTicks, -startTicks);
    }
    public Number Invert(Number num) => new(num.Domain.Inverse, num.Focal);
    public Number InvertNegate(Number num) => new(num.Domain.Inverse, num.Focal.NegateClone());
    public Number MirrorStart(Number num) // inverted Conjugate
    {
        var (startTicks, endTicks) = RawTicksFromZero(num);
        return CreateNumberRaw(-startTicks, endTicks);
    }
    public Number MirrorEnd(Number num) // aligned conjugate
    {
        var (startTicks, endTicks) = RawTicksFromZero(num);
        return CreateNumberRaw(startTicks, -endTicks);
    }
    #endregion
    #region Number Creation
    public Number CreateNumberRaw(long startTicks,long endTicks)
    {
        return new Number(this, new Focal(
            BasisFocal.StartTick + startTicks,
            BasisFocal.StartTick + endTicks));
    }
    public Number CreateNumberSigned(long startTicks, long endTicks)
    {
        return new Number(this, new Focal(
            BasisFocal.StartTick - startTicks * Direction,
            BasisFocal.StartTick + endTicks * Direction));
    }
    public Number CreateNumberFromTs(double startT, double endT) =>
        new(this, BasisFocal.FocalFromTs(-startT, endT));
    public Number AdditiveIdentity => new Number(this, new Focal(BasisFocal.StartTick, BasisFocal.StartTick));
    public Number MultiplicativeIdentity => new Number(this, BasisFocal);
    public Number Zero => new(this, new Focal(BasisFocal.StartTick, BasisFocal.StartTick));
    public Number One => new(this, BasisFocal.Clone());
    public Number MinusOne => new(this, BasisFocal.FlipAroundFirstClone());
    public Number One_i => new(this, BasisFocal.FlipAroundFirstClone().InvertClone());
    public Number MinusOne_i => new(this, BasisFocal.InvertClone());
    #endregion
    #region Conversions
    public long TicksFromZero(long tick) => tick - BasisFocal.StartTick;
    public long TicksFromZeroDirected(long tick) => (tick - BasisFocal.StartTick) * Direction;
    public (long, long) RawTicksFromZero(Number num) => (TicksFromZero(num.StartTick), TicksFromZero(num.EndTick));
    public (long, long) SignedTicksFromZero(Number num) => (-TicksFromZeroDirected(num.StartTick), TicksFromZeroDirected(num.EndTick));
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

    public (double, double) RawValues(Number num) => (
        TicksFromZeroDirected(num.StartTick) / (double)BasisFocal.AbsLength,
        TicksFromZeroDirected(num.EndTick) / (double)BasisFocal.AbsLength);
    public (double, double) SignedValues(Number num) => (
        -TicksFromZeroDirected(num.StartTick) / (double)BasisFocal.AbsLength,
        TicksFromZeroDirected(num.EndTick) / (double)BasisFocal.AbsLength);
    public PRange GetRange(Number num)
    {
        var (start, end) = SignedValues(num);
        if (BasisIsReciprocal)
        {
            start = Math.Round(start) * BasisFocal.AbsLength;
            end = Math.Round(end) * BasisFocal.AbsLength;
        }
        return new PRange(start, end, num.Polarity);
    }
    public double RawTickValue(long tick) => TicksFromZeroDirected(tick) / (double)BasisFocal.AbsLength;

        
    public double AlignedValueAtT(Number num, double t)
    {
        double result;
        if(t == 0)
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
    #endregion

    #region Equality
    public Domain Clone() => new Domain(Trait, BasisFocal.Clone(), LimitsFocal.Clone());
    public override bool Equals(object? obj)
    {
        return Equals(obj as Domain);
    }

    public bool Equals(Domain? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Trait == other.Trait &&
               EqualityComparer<Focal>.Default.Equals(BasisFocal, other.BasisFocal) &&
               EqualityComparer<Focal>.Default.Equals(LimitsFocal, other.LimitsFocal);
    }
    public static bool operator ==(Domain? left, Domain? right)
    {
        return EqualityComparer<Domain>.Default.Equals(left, right);
    }
    public static bool operator !=(Domain? left, Domain? right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (Trait?.GetHashCode() ?? 0);
            hash = hash * 23 + BasisFocal.GetHashCode();
            hash = hash * 23 + LimitsFocal.GetHashCode();
            return hash;
        }
    }
    #endregion
}
