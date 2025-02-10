using Numerics.Primitives;
using NumericsCore.Primitives;

namespace NumericsCore.Motions;

public enum BitSymmetry
{
    Stop = 0,           // 0000

    Identity = 1,       // 0001
    NegateValue = 2,    // 0010
    NegateUnit = 4,     // 0100
    Invert = 8,         // 1000

    MoveRight = 5,      // 0101
    MoveLeft = 10,      // 1010

    LineRight = 9,      // 1001
    LineLeft = 6,       // 0110

    Split = 3,          // 0011
    Join = 12,          // 1100

    ReverseLeft = 14,   // 1110
    ForwardRight = 13,  // 1101
    ReverseRight = 11,  // 1011
    ForwardLeft = 7,    // 0111

    Continue = 15,      // 1111
}

public enum BitMask
{
    None = 0, // 0000

    A = 1,    // 0001
    B = 2,    // 0010
    C = 4,    // 0100
    D = 8,    // 1000

    AC = 5,   // 0101
    BD = 10,  // 1010

    AD = 9,   // 1001
    BC = 6,   // 0110

    AB = 3,   // 0011
    CD = 12,  // 1100

    BCD = 14,  // 1110
    ACD = 13,  // 1101
    ABD = 11,  // 1011
    ABC = 7,   // 0111

    ABCD = 15, // 1111
}
public static class BitMaskExtension
{
    public static bool GetBit1(this BitMask bits) => ((int)bits & 0x01) != 0;
    public static bool GetBit2(this BitMask bits) => ((int)bits & 0x02) != 0;
    public static bool GetBit3(this BitMask bits) => ((int)bits & 0x04) != 0;
    public static bool GetBit4(this BitMask bits) => ((int)bits & 0x08) != 0;
	public static long[] GetPositions(this BitMask bits, long[] positions)
	{
		var result = new List<long>();
		if (positions.Length > 0 && GetBit1(bits)) { result.Add(positions[0]); }
		if (positions.Length > 1 && GetBit2(bits)) { result.Add(positions[1]); }
		if (positions.Length > 2 && GetBit3(bits)) { result.Add(positions[2]); }
		if (positions.Length > 3 && GetBit4(bits)) { result.Add(positions[3]); }
		return result.ToArray();
	}
	public static long CombinePositions(this BitMask bits, Ops op, long[] positions)
	{
        var pos = GetPositions(bits, positions);
        long result = pos.Length > 0 ? pos[0] : 0;
        if (pos.Length > 1)
        {
            switch (op)
            {
                case Ops.Add:
                    result = pos.Sum();
                    break;
                case Ops.Subtract:
                    result = pos.Skip(1).Aggregate(pos[0], (acc, num) => acc - num);
                    break;
                case Ops.Multiply:
                    result = pos.Aggregate(1L, (acc, num) => acc * num);
                    break;
                case Ops.Divide:
                    result = (long)pos.Skip(1).Aggregate((double)pos[0], (double acc, long num) => acc / (double)num);
                    break;
                case Ops.Pow:
                    result = pos.Skip(1).Aggregate(pos[0], (acc, num) => (long)Math.Pow(acc, num));
					break;
				case Ops.Min:
					result = pos.Min();
					break;
				case Ops.Max:
					result = pos.Max();
					break;
			}
        }
        return result;
	}
}
public enum Ops // todo: need an order to operations, as in multiplication is a pow of addition etc.
{
    None,
    Add,
    Subtract,
    Multiply,
    Divide,
    Pow,
    BoolOp,
    Comparison,
    Min,
	Max,
	MinMax,
	Delay,
    IncDec,
    IncDecUnit,
    Invert,
    Shift,

    Area,
    Length,

}