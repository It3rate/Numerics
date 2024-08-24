
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
    public Trait? Trait { get; protected set; }
    private Focal BasisFocal { get; set; }
    private Focal LimitsFocal { get; set; }

    public Number BasisNumber => new(this, BasisFocal);
    public Number LimitsNumber => new(this, LimitsFocal);
    public long TickSize { get; protected set; } = 1;
    public long BasisLength => BasisFocal.TickLength;
    public long AbsBasisLength => BasisFocal.AbsTickLength;
    public int Direction => BasisFocal.Direction;
    public long AbsLimitsSize => LimitsFocal.AbsTickLength;
    public bool BasisIsReciprocal => Math.Abs(TickSize) > BasisFocal.AbsTickLength;
    public double TickToBasisRatio => TickSize / BasisFocal.NonZeroTickLength;
    public Domain(Trait trait, Focal basisFocal, Focal limitsFocal)
    {
        Trait = trait;
        BasisFocal = basisFocal;
        LimitsFocal = limitsFocal;
    }
    private Domain(Focal basisFocal, Focal limitsFocal)
    {
        Trait = Trait.WorkingTrait;
        BasisFocal = basisFocal;
        LimitsFocal = limitsFocal;
    }

    public Number AlignedDomain(Number value)
    {
        Number result = value;
        if(value.Domain != this)
        {
            var start = value.StartValue;
            var end = value.EndValue;
            var focal = FocalFromValues(start, end);
            result = new(this, focal, value.Polarity);
        }
        return result;
    }
    #region Conversions
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
    public long TicksFromZero(long tick) => (tick - BasisFocal.StartTick) * BasisFocal.Direction;
    public long PositiveOffsetTick(long tick) => (BasisFocal.StartTick + tick) * BasisFocal.Direction;
    public long NegativeOffsetTick(long tick) => (BasisFocal.StartTick - tick) * BasisFocal.Direction;
    public (long, long) RawTicksFromZero(Number num)
    {
        return (TicksFromZero(num.StartTick), TicksFromZero(num.EndTick));

    }
    public (long, long) TickValuesFromZero(Number num, Polarity asPolarity)
    {
        var inversion = num.Polarity == asPolarity ? 1 : num.Polarity.Direction();
        inversion *= num.Polarity.Direction();
        return (-TicksFromZero(num.StartTick) * inversion, TicksFromZero(num.EndTick) * inversion);
    }
    public (long, long) TickValuesFromZero(Number num)
    {
        return (-TicksFromZero(num.StartTick), TicksFromZero(num.EndTick));
    }
    public Number CreateNumber(long startTick, bool invertStart, long endTick, bool invertEnd, Polarity polarity)
    {
        var start = invertStart ? NegativeOffsetTick(startTick) : PositiveOffsetTick(startTick);
        var end = invertEnd ? NegativeOffsetTick(endTick) : PositiveOffsetTick(endTick);
        return new Number(this, new Focal(start, end), polarity);
    }
    public Focal FocalFromDecimal(double start, double end)
    {
        return new Focal(
            (long)(-start * BasisFocal.TickLength),
            (long)(end * BasisFocal.TickLength));
    }
    public Focal FocalFromValues(double startValue, double endValue)
    {
        var result = new Focal(TickValue(-startValue), TickValue(endValue));
        return result;
    }
    private long TickValue(double value)
    {
        var result = (long)(value * BasisFocal.NonZeroTickLength);
        result += BasisFocal.StartTick;
        result =
            (result < LimitsFocal.StartTick) ? LimitsFocal.StartTick :
            (result > LimitsFocal.EndTick) ? LimitsFocal.EndTick : result;
        return result;
    }

    public PRange GetRange(Number num)
    {
        var len = (double)BasisFocal.NonZeroTickLength;// * Polarity.ForceValue(); //AlignedNonZeroLength(isAligned);// 
        var start = (num.StartTick - BasisFocal.StartTick) / len;
        var end = (num.EndTick - BasisFocal.StartTick) / len;
        if (BasisIsReciprocal)
        {
            start = Math.Round(start) * Math.Abs(len);
            end = Math.Round(end) * Math.Abs(len);
        }
        var result = num.IsAligned ? new PRange(-start, end, num.Polarity) : new PRange(start, -end, num.Polarity.Invert());
        return result;
    }
    #endregion

    public Domain Duplicate()
    {
        var result = new Domain(Trait, BasisFocal, LimitsFocal);
        return result;
	}
	public Number AdditiveIdentity => new Number(this, new Focal(BasisFocal.StartTick, BasisFocal.StartTick));
	public Number MultiplicativeIdentity => new Number(this, BasisFocal);


    public Number Zero => new(this, new Focal(BasisFocal.StartTick, BasisFocal.StartTick), Polarity.Aligned);
    public Number One => new(this, BasisFocal.Clone(), Polarity.Aligned);
    public Number MinusOne => new(this, BasisFocal.FlipAroundFirst(), Polarity.Aligned);
    public Number One_i => new(this, BasisFocal.FlipAroundFirst(), Polarity.Inverted);
    public Number MinusOne_i => new(this, BasisFocal.Clone(), Polarity.Inverted);
}
