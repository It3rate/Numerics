
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
public class Domain
{
    public Trait? Trait { get; }
    private Focal BasisFocal { get; }
    private Focal LimitsFocal { get; }
    public Polarity Polarity => 
        BasisFocal.Direction > 0 ? Polarity.Aligned :
        BasisFocal.Direction < 0 ? Polarity.Inverted : Polarity.None;

    public Number BasisNumber => new(this, BasisFocal);
    public Number LimitsNumber => new(this, LimitsFocal);
    public long TickSize { get; protected set; } = 1;
    public long BasisLength => BasisFocal.TickLength;
    public long AbsBasisLength => BasisFocal.AbsTickLength;
    public int Direction => BasisFocal.Direction;
    public long AbsLimitsSize => LimitsFocal.AbsTickLength;
    public bool BasisIsReciprocal => Math.Abs(TickSize) > BasisFocal.AbsTickLength;
    public double TickToBasisRatio => TickSize / BasisFocal.NonZeroTickLength;
    public Domain(Trait? trait, Focal basisFocal, Focal limitsFocal)
    {
        Trait = trait;
        BasisFocal = basisFocal;
        LimitsFocal = limitsFocal;
    }
    private Domain(Focal basisFocal, Focal limitsFocal) : this(Trait.WorkingTrait, basisFocal, limitsFocal) { }

    public Domain InvertedDomain() => new Domain(Trait, BasisFocal.FlipAroundFirst(), LimitsFocal.Invert());

    #region Conversions
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

    public bool IsZero(Number num) => num.StartTick == BasisFocal.StartTick && num.EndTick == BasisFocal.StartTick;
    public bool IsOne(Number num) => num.StartTick == BasisFocal.StartTick && num.EndTick == BasisFocal.EndTick;
    public long TicksFromZero(long tick) => tick - BasisFocal.StartTick;
    public long TicksFromZeroDirected(long tick) => (tick - BasisFocal.StartTick) * Direction;
    public (long, long) RawTicksFromZero(Number num) => (TicksFromZero(num.StartTick), TicksFromZero(num.EndTick));
    public (long, long) SignedTicksFromZero(Number num) => (-TicksFromZeroDirected(num.StartTick), TicksFromZeroDirected(num.EndTick));
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

    private long TickValueAligned(double value)
    {
        var result = (long)(BasisFocal.StartTick + (value * BasisFocal.TickLength));
        // todo: Clamp to limits, account for basis direction.
        return result;
    }
    private long TickValueInverted(double value)
    {
        var result = (long)(BasisFocal.StartTick - (value * BasisFocal.TickLength));
        // todo: Clamp to limits, account for basis direction.
        return result;
    }
    public Focal FocalFromDecimalRaw(double startValue, double endValue) =>
        new Focal(TickValueAligned(startValue), TickValueAligned(endValue));
    public Focal FocalFromDecimalSigned(double startValue, double endValue) =>
        new Focal(TickValueInverted(startValue), TickValueAligned(endValue));

    public (double, double) RawValues(Number num) => (
        TicksFromZeroDirected(num.StartTick) / (double)BasisFocal.AbsTickLength,
        TicksFromZeroDirected(num.EndTick) / (double)BasisFocal.AbsTickLength);
    public (double, double) SignedValues(Number num) => (
        -TicksFromZeroDirected(num.StartTick) / (double)BasisFocal.AbsTickLength,
        TicksFromZeroDirected(num.EndTick) / (double)BasisFocal.AbsTickLength);

    public PRange GetRange(Number num)
    {
        var (start, end) = SignedValues(num);
        if (BasisIsReciprocal)
        {
            start = Math.Round(start) * BasisFocal.AbsTickLength;
            end = Math.Round(end) * BasisFocal.AbsTickLength;
        }
        return new PRange(start, end, num.Polarity);
    }
    #endregion

    public Domain Duplicate()
    {
        var result = new Domain(Trait, BasisFocal, LimitsFocal);
        return result;
	}
	public Number AdditiveIdentity => new Number(this, new Focal(BasisFocal.StartTick, BasisFocal.StartTick));
	public Number MultiplicativeIdentity => new Number(this, BasisFocal);


    public Number Zero => new(this, new Focal(BasisFocal.StartTick, BasisFocal.StartTick));
    public Number One => new(this, BasisFocal.Clone());
    public Number MinusOne => new(this, BasisFocal.FlipAroundFirst());
    public Number One_i => new(this, BasisFocal.FlipAroundFirst().Invert());
    public Number MinusOne_i => new(this, BasisFocal.Invert());
}
