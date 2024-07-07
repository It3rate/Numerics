using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Numerics.Primitives
{
    // Eventually the core number will be nbit aware and take advantage of simD/gpu capabilities.
    //public interface IPosition {}

    public abstract class Numeric<T> where T : 
		Numeric<T>, 
		IAdditionOperators<T, T, T>
    {
        public long Start { get; set; }
        public long End { get; set; }
        public long Length { get; protected set; }

        public abstract void Add(T other);

        public static T operator +(Numeric<T> left, Numeric<T> right)
        {
            var result = (T)left.MemberwiseClone();
            result.Add((T)right);
            return result;
        }
    }

    public class Focal : 
		Numeric<Focal>, 
		IAdditionOperators<Focal, Focal, Focal>
    {
        public override void Add(Focal other)
        {
            this.Start += other.Start;
            this.End += other.End;
            this.Length += other.Length;
        }
        public static Focal operator +(Focal left, Focal right)
        {
            var result = (Focal)left.MemberwiseClone();
            result.Add(right);
            return result;
        }

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
        long Length { get; protected set; }
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
