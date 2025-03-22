using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Motions;

public class SymmetricNumber : INumber // can be number with unit, or two 'tick only' measurements
{
    public INumberline Numberline {get;}
    public IUnit Unit => Numberline.Unit;
	public Focal MinMax => Unit.Limits;
    public Focal Basis => Numberline.Basis;
    public Focal TopFocal { get; }

    public long ZeroOffset { get; } = 0; // running average, or unobstructed average. Calculated on Numberline.

    public Tuple<long, long, long, long> TickSet => new Tuple<long, long, long, long>(A_TR, B_TL, C_BR, D_BL);
    private long A_TR => TopFocal.EndTick - ZeroOffset;
    private long B_TL => -(TopFocal.StartTick - ZeroOffset); // positive is always in the direction of the unit
    private long C_BR => Basis.EndTick - ZeroOffset;
    private long D_BL => -(Basis.StartTick - ZeroOffset);

    public bool IsPositiveDirection => StartValue >= EndValue;
    // landmark is two focals and a double?

    public Number Segment => throw new NotImplementedException();

    public SymmetricNumber(IUnit unit, Focal topFocal, Focal bottomFocal, long zeroOffset = 0)
    {
        Numberline = new Numberline(unit, bottomFocal);
        TopFocal = topFocal;
        ZeroOffset = zeroOffset;
    }
    public SymmetricNumber(IUnit unit, long start, long end, long unitLength)
    {
		Numberline = new Numberline(unit, new Focal(-unitLength, unitLength));
        TopFocal = new Focal(start, end);
    }
    public SymmetricNumber(IUnit unit, long start, long endUnit, long startUnit, long end, long zeroOffset = 0)
	{
		Numberline = new Numberline(unit, new Focal(endUnit + zeroOffset, startUnit + zeroOffset));
        TopFocal = new Focal(start + zeroOffset, end + zeroOffset);
    }

    public double StartValue => B_TL / (double)D_BL;
    public double EndValue => A_TR / (double)C_BR;
	public double Length => EndValue - StartValue;
	public double AbsLength => Math.Abs(Length);
	public double StartValueAtPosition(long pos)
	{
		var input = -(pos - ZeroOffset);
		return input / (double)D_BL;
	}
	public double EndValueAtPosition(long pos)
	{
		var input = pos - ZeroOffset;
		return input / (double)C_BR;
	}

	public double InteriorSample(Random rnd)
	{
		return rnd.Next(0, (int)AbsLength) + StartValue;
	}
	public long[] GetLengthSet(BitMask mask)
    {
        List<long> result = new List<long>();
        if (mask.GetBit1()) { result.Add(A_TR); }
        if (mask.GetBit2()) { result.Add(B_TL); }
        if (mask.GetBit3()) { result.Add(C_BR); }
        if (mask.GetBit4()) { result.Add(D_BL); }
        return result.ToArray();
    }
    public long GetSum(BitMask mask) =>  GetLengthSet(mask).Sum();
	public long GetProduct(BitMask mask) => GetLengthSet(mask).Aggregate((long)1, (acc, next) => acc * next);
	public long GetQuotient(BitMask mask) => GetLengthSet(mask).Aggregate((long)1, (acc, next) => (long)(acc * (1.0 / next)));
	public long GetSumDifference(BitMask numerator, BitMask denominator) => GetSum(numerator) - GetSum(denominator);
	public long GetProductDifference(BitMask numerator, BitMask denominator) => GetProduct(numerator) - GetProduct(denominator);
	public long GetQuotientSum(BitMask numerator, BitMask denominator) => GetQuotient(numerator) + GetQuotient(denominator);
	public double GetSumRatio(BitMask numerator, BitMask denominator) => GetSum(numerator) / (double)GetSum(denominator);
    public double GetProductRatio(BitMask numerator, BitMask denominator) => GetProduct(numerator) - (double)GetProduct(denominator);

	public long[] GetLengths => [A_TR, B_TL, C_BR, D_BL];
    public long[] GetAreas => [
        A_TR * C_BR, 
        B_TL * C_BR, 
        D_BL * A_TR, 
        B_TL * D_BL]; // AB and CD are missing because not combining with self.
    public double[] GetRatios => // Assumes top is values, bottom is units. [Start, unit ratio, value ratio, End]
        [D_BL / (double)B_TL,
         C_BR / (double)B_TL,
         A_TR / (double)D_BL,
         A_TR / (double)C_BR];

    // Inverse is rearrange focals?
    // four interpretations by changing Identity

    // transform values of focals
    // points of interest for calculating areas etc
    // truths, is zero, equal units, equal resolution etc
    // properties: length, tick length. All transform recipies?
    // FUNCS for transforms and common operations
    // Comparisons
    // Conversions, align resolution, units etc
    // equality, tostring etc
    public SymmetricNumber Interpolate(double start, double end, bool clamp = true)
    {
        start = clamp ? Math.Max(start, 0) : start;
        end = clamp ? Math.Min(end, 1) : end;
        var len = TopFocal.Length;
        var startTick = TopFocal.StartTick + start * len;
        var endTick = TopFocal.StartTick + end * len;
        var result = new SymmetricNumber(Unit, new Focal((long)startTick, (long)endTick), Basis);
        return result;
    }
}
