
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
    public Focal BasisFocal { get; protected set; }
    public Focal LimitsFocal { get; set; }

    public Number BasisNumber => new(this, BasisFocal);
    public Number LimitsNumber => new(this, LimitsFocal);
    public long TickSize { get; protected set; } = 1;
    public long AbsBasisSize => BasisFocal.AbsTickLength;
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
            var focal = new Focal(TickValue(start), TickValue(end));
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
            var maxBasis = left.AbsBasisSize >= right.AbsBasisSize ? left.BasisFocal : right.BasisFocal;
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

    public Focal FocalFromDecimal(double start, double end)
    {
        return new Focal(
            (long)(-start * BasisFocal.TickLength),
            (long)(end * BasisFocal.TickLength));
    }
    public long TickValue(double value)
    {
        var result = (long)(value * BasisFocal.NonZeroTickLength);
        result += BasisFocal.FirstTick;
        result =
            (result < LimitsFocal.FirstTick) ? LimitsFocal.FirstTick :
            (result > LimitsFocal.LastTick) ? LimitsFocal.LastTick : result;
        return result;
    }
    #endregion

    public Domain Duplicate()
    {
        var result = new Domain(Trait, BasisFocal, LimitsFocal);
        return result;
	}
	public Number AdditiveIdentity => new Number(this, new Focal(BasisFocal.FirstTick, BasisFocal.FirstTick));
	public Number MultiplicativeIdentity => new Number(this, BasisFocal);


    public Number Zero => new(this, new Focal(BasisFocal.FirstTick, BasisFocal.FirstTick), Polarity.Aligned);
    public Number One => new(this, BasisFocal.Clone(), Polarity.Aligned);
    public Number MinusOne => new(this, BasisFocal.FlipAroundFirst(), Polarity.Aligned);
    public Number One_i => new(this, BasisFocal.FlipAroundFirst(), Polarity.Inverted);
    public Number MinusOne_i => new(this, BasisFocal.Clone(), Polarity.Inverted);
}
