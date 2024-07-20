
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
    public Trait Trait { get; protected set; }
    public Focal BasisFocal { get; protected set; }
    public Focal MinMaxFocal { get; set; }
    public long TickSize { get; protected set; } = 1;
    public bool BasisIsReciprocal => Math.Abs(TickSize) > BasisFocal.AbsTickLength;
    public double TickToBasisRatio => TickSize / BasisFocal.NonZeroTickLength;

    public Domain(Trait trait, Focal basisFocal, Focal minMaxFocal)
    {
        Trait = trait;
        BasisFocal = basisFocal;
        MinMaxFocal = minMaxFocal;
    }
    public Domain Duplicate()
    {
        var result = new Domain(Trait, BasisFocal, MinMaxFocal);
        return result;
	}
	public Number AdditiveIdentity => new Number(this, new Focal(BasisFocal.StartTick, BasisFocal.StartTick));
	public Number MultiplicativeIdentity => new Number(this, BasisFocal);
}
