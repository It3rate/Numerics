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
}
public enum Ops
{
    None,
    Add,
    Subtract,
    Multiply,
    Divide,
    Pow,
    BoolOp,
    Comparison,
    MinMax,
    Delay,
    IncDec,
    IncDecUnit,
    Invert,
    Shift,

    Area,
    Length,

}