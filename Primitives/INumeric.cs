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

    public interface Numeric<T> where T : 
		Numeric<T>, 
		IAdditionOperators<T, T, T>,
		ISubtractionOperators<T, T, T>,
        IMultiplyOperators<T, T, T>,
        IDivisionOperators<T, T, T>, 
        IAdditiveIdentity<T, T>,
        IMultiplicativeIdentity<T, T>,
        IIncrementOperators<T>,
        IDecrementOperators<T>,
        IUnaryNegationOperators<T, T>,
        IUnaryPlusOperators<T, T>,
        IMinMaxValue<T>
    {
        long Start { get; set; }
        long End { get; set; }
        long Length => End - Start;

    }

    public struct Focal : 
		Numeric<Focal>,
        IAdditionOperators<Focal, Focal, Focal>,
        ISubtractionOperators<Focal, Focal, Focal>,
        IMultiplyOperators<Focal, Focal, Focal>,
        IDivisionOperators<Focal, Focal, Focal>,
        IAdditiveIdentity<Focal, Focal>,
        IMultiplicativeIdentity<Focal, Focal>,
        IIncrementOperators<Focal>,
        IDecrementOperators<Focal>, 
        IUnaryNegationOperators<Focal, Focal>,
        IUnaryPlusOperators<Focal, Focal>,
        IMinMaxValue<Focal>
    {
        public long Start { get; set; }
        public long End { get; set; }
        public long Length => End - Start;

        public Focal(long start, long end)
        {
            Start = start;
            End = end;
        }

        #region Add
        public static Focal Add(Focal left, Focal right) => left + right;
        public static Focal operator +(Focal left, Focal right) => new(left.Start + right.Start, left.End + right.End);
        static Focal IAdditionOperators<Focal, Focal, Focal>.operator +(Focal left, Focal right) => left + right;

        public static Focal AdditiveIdentity => new (0, 0); 
        public static Focal operator ++(Focal value) => new (value.Start + 1, value.End + 1);
        public static Focal operator +(Focal value) => new(value.Start, value.End);

        #endregion
        #region Subtract
        public static Focal Subtract(Focal left, Focal right) => left - right;
        public static Focal operator -(Focal left, Focal right) => new(left.Start - right.Start, left.End - right.End);
        static Focal ISubtractionOperators<Focal, Focal, Focal>.operator -(Focal left, Focal right) => left - right;

        public static Focal operator --(Focal value) => new(value.Start - 1, value.End - 1);
        public static Focal operator -(Focal value) => new(-value.Start, -value.End);
        #endregion
        #region Multiply
        public static Focal Multiply(Focal left, Focal right)
        {
            return left * right;
        }
        public static Focal operator *(Focal left, Focal right) => new(
            left.Start * right.End + left.End * right.Start, 
            left.End * right.End - left.Start * right.Start);

        static Focal IMultiplyOperators<Focal, Focal, Focal>.operator *(Focal left, Focal right) => left * right;
        public static Focal MultiplicativeIdentity => new (0, 1);

        #endregion
        #region Divide
        public static Focal Divide(Focal left, Focal right)
        {
            return left / right;
        }

        public static Focal operator /(Focal left, Focal right)
        {
            Focal result;
            double leftEnd = left.End;
            double leftStart = left.Start;
            double rightEnd = right.End;
            double rightStart = right.Start;
            if (Math.Abs(rightStart) < Math.Abs(rightEnd))
            {
                double num = rightStart / rightEnd;
                result = new Focal(
                    (long)((leftStart - leftEnd * num) / (rightEnd + rightStart * num)), 
                    (long)((leftEnd + leftStart * num) / (rightEnd + rightStart * num)));
            }
            else
            {
                double num1 = rightEnd / rightStart;
                result = new Focal(
                    (long)((-leftEnd + leftStart * num1) / (rightStart + rightEnd * num1)),
                    (long)((leftStart + leftEnd * num1) / (rightStart + rightEnd * num1)));
            }
            return result;
        }
        static Focal IDivisionOperators<Focal, Focal, Focal>.operator /(Focal left, Focal right) =>  left / right;

        #endregion
        #region MinMax
        public static Focal MaxValue => new(long.MaxValue, long.MaxValue);
        public static Focal MinValue => new(long.MinValue, long.MinValue);

        #endregion
        // IsFractional, IsInverted, IsNegative, IsNormalized, IsZero, IsOne, IsZeroStart, IsPoint, IsOverflow, IsUnderflow
        // IsLessThanBasis, IsGrowable, IsBasisLength, IsMin, HasMask, IsArray, IsMultiDim, IsCalculated, IsRandom
        // Domain: IsTickLessThanBasis, IsBasisInMinmax, IsTiling, IsClamping, IsInvertable, IsNegateable, IsPoly, HasTrait


    }

    //   public abstract class Numeric :
    //INumeric,
    //   IAdditionOperators<Numeric, Numeric, Numeric>
    //   {
    //       public long Start { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //       public long End { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //       public long Length { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    //       public static Numeric operator +(Numeric left, Numeric right)
    //       {
    //           throw new NotImplementedException();
    //       }
    //   }
    public interface INumeric
    {
        long Start { get; set; }
        long End { get; set; }
        long Length { get; }
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
		IsNegaitveOverflow
		IsPositiveUnderflow
		IsResolutionIntact (has an uneven division occoured)
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
		DivRem(quotient, remainder) // IbinaryInteger
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
}
