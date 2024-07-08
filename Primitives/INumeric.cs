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
		IAdditionOperators<T, T, T>,
		ISubtractionOperators<T, T, T>,
        IMultiplyOperators<T, T, T>,
        IDivisionOperators<T, T, T>, 
        IAdditiveIdentity<T, T>,
        IMultiplicativeIdentity<T, T>,
        IIncrementOperators<T>,
        IDecrementOperators<T>

    {
        public long Start { get; set; }
        public long End { get; set; }
        public long Length => End - Start;

        public abstract void Add(T other);
        public abstract void Subtract(T other);
        public abstract void Multiply(T other);
        public abstract void Divide(T other);
    }

    public class Focal : 
		Numeric<Focal>,
        IAdditionOperators<Focal, Focal, Focal>,
        ISubtractionOperators<Focal, Focal, Focal>,
        IMultiplyOperators<Focal, Focal, Focal>,
        IDivisionOperators<Focal, Focal, Focal>,
        IAdditiveIdentity<Focal, Focal>,
        IMultiplicativeIdentity<Focal, Focal>,
        IIncrementOperators<Focal>,
        IDecrementOperators<Focal>
    {
        #region Add
        public override void Add(Focal other)
        {
            this.Start += other.Start;
            this.End += other.End;
        }
        public static Focal Add(Focal left, Focal right)
        {
            return left + right;
        }
        public static Focal operator +(Focal left, Focal right)
        {
            var result = (Focal)left.MemberwiseClone();
            result.Add(right);
            return result;
        }
        static Focal IAdditionOperators<Focal, Focal, Focal>.operator +(Focal left, Focal right)
        {
            return left + right;
        }

        public static Focal AdditiveIdentity => new Focal { Start = 0, End = 0 }; 
        public static Focal operator ++(Focal value)
        {
            var result = (Focal)value.MemberwiseClone();
            result.Start++;
            result.End++;
            return result;
        }

        #endregion
        #region Subtract
        public override void Subtract(Focal other)
        {
            this.Start -= other.Start;
            this.End -= other.End;
        }
        public static Focal Subtract(Focal left, Focal right)
        {
            return left - right;
        }
        public static Focal operator -(Focal left, Focal right)
        {
            var result = (Focal)left.MemberwiseClone();
            result.Subtract(right);
            return result;
        }
        static Focal ISubtractionOperators<Focal, Focal, Focal>.operator -(Focal left, Focal right)
        {
            return left - right;
        }
        public static Focal operator --(Focal value)
        {
            var result = (Focal)value.MemberwiseClone();
            result.Start--;
            result.End--;
            return result;
        }
        #endregion
        #region Multiply
        public override void Multiply(Focal other)
        {
            this.Start *= other.Start;
            this.End *= other.End;
        }
        public static Focal Multiply(Focal left, Focal right)
        {
            return left * right;
        }
        public static Focal operator *(Focal left, Focal right)
        {
            var result = (Focal)left.MemberwiseClone();
            result.Multiply(right);
            return result;
        }
        static Focal IMultiplyOperators<Focal, Focal, Focal>.operator *(Focal left, Focal right)
        {
            return left * right;
        }
        public static Focal MultiplicativeIdentity => new Focal { Start = 0, End = 1 };
        #endregion
        #region Divide
        public override void Divide(Focal other)
        {
            this.Start /= other.Start;
            this.End /= other.End;
        }
        public static Focal Divide(Focal left, Focal right)
        {
            return left / right;
        }
        public static Focal operator /(Focal left, Focal right)
        {
            var result = (Focal)left.MemberwiseClone();
            result.Divide(right);
            return result;
        }
        static Focal IDivisionOperators<Focal, Focal, Focal>.operator /(Focal left, Focal right)
        {
            return left / right;
        }
        #endregion
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
