using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Numerics.Primitives
{
    // Eventually the core number will be nbit aware and take advantage of simD/gpu capabilities.
    //public interface IPosition {}
    // Join as parallel domains, vs perpendicular (contour vs grid).
    public interface IMeasurable
    {
        long FirstTick { get; set; }
        long LastTick { get; set; }
        long TickLength { get; }
        long AbsTickLength { get; }
    }

    public interface Measurable<T> where T :
        Measurable<T>,
        IAdditionOperators<T, T, T>,
        ISubtractionOperators<T, T, T>,
        IAdditiveIdentity<T, T>,
        IIncrementOperators<T>,
        IDecrementOperators<T>,
        IUnaryNegationOperators<T, T>,
        IUnaryPlusOperators<T, T>,
        IMinMaxValue<T>
    {
    }

    /*
INumber<long> 
 Clamp, 
 CopySign, 
 Max, 
 Min, 
 Sign
INumberBase<long>
IAdditionOperators<TSelf, TSelf, TSelf>,
IAdditiveIdentity<TSelf, TSelf>,
IDecrementOperators<TSelf>,
IDivisionOperators<TSelf, TSelf, TSelf>,
IEquatable<TSelf>,
IEqualityOperators<TSelf, TSelf, bool>,
IIncrementOperators<TSelf>,
IMultiplicativeIdentity<TSelf, TSelf>,
IMultiplyOperators<TSelf, TSelf, TSelf>,
ISubtractionOperators<TSelf, TSelf, TSelf>,
IUnaryPlusOperators<TSelf, TSelf>,
IUnaryNegationOperators<TSelf, TSelf>,

    start (zero)
    end (one)
    resolution (ticks, starts as one)
    Abs ('right' pointing value)
    AbsPolarity (positive right time line)
    IsSegment (has length, include assumed zero start?)
    IsDual ('complex number', doesn't start at origin)
    IsEvenTicks (start and end can be divided into existing resolution?)
    IsEven (start and end values are even)
    IsOddTicks
    IsOdd
    IsBasis
    IsMinmax
    IsInteger (basis division leaves zero ticks)
    IsOverflow
    IsUnderflow
    IsNegativeOverflow
    IsPositiveUnderflow
    IsResolutionIntact (has an uneven division occurred)
    InPositive
    InNegative
    InPositivePolarity
    InNegativePolarity
IBinaryInteger<long>
IBinaryNumber<long>
IBitwiseOperators<long long long>
    Not // binary
    IsPow2
    Log2
    Rol, Rol,
    DivRem(quotient, remainder) // IBinaryInteger
IComparisonOperators<long long bool>
IModulusOperators<long long long>
IShiftOperators<long ong>
IMinMaxValue<long>
ISignedNumber<long>
    bit masks, set bits, endian, leading/trailing Zeros, shortestBitLen
    + - * / ! ~ %,--,++,=,==,>,>=,<,<=,negate,flip
    >>,<<,>>>,rol, ror
    Min(a, b)

    Max(a, b)

    bool ops !&|^ (plus 12)

IComparable<long>
 IsEqual, GetHash
        */
}
