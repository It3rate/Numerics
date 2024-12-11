using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using static System.Net.WebRequestMethods;

namespace NumericsCore.Utils
{
    public class BitField
    {
        private const int MAX_BITS = 8;
        private long _value;
        public long Value() => _value;

        public bool GetBit(int bitPosition) => (_value & (1 << bitPosition)) != 0;
        public void SetBit(int bitPosition) => _value |= (1 << bitPosition);
        public void ClearBit(int bitPosition) =>  _value &= ~(1 << bitPosition);
        public bool[] GetBoolValues() => BitField.GetBoolValues(_value, MAX_BITS);
        public static bool[] GetBoolValues(long value, int maxBits = 4)
        {
            bool[] bits = new bool[maxBits];

            for (int i = 0; i < maxBits; i++)
            {
                bits[i] = (value & (1 << i)) != 0;
            }
            return bits;
        }
    }
    public class NumberSet
    {
        Number[] _values;
        public NumberSet(params Number[] values)
        {
            _values = values;
        }
        public Number this[int index] => _values[index];

        public NumberSet Inverse()
        {
            var vals = _values.Select(n => n.Inverse).ToArray();
            return new NumberSet(vals);
        }
    }
    public class ValuedBools
    {
        private NumberSet _value3;
        private NumberSet _value2;
        private NumberSet _value1;
        private NumberSet _value0;

        private NumberSet _inverse3;
        private NumberSet _inverse2;
        private NumberSet _inverse1;
        private NumberSet _inverse0;

        private readonly Func<NumberSet[]>[] _sets;
        private readonly Func<NumberSet[]>[] _inverseSets;

        public ValuedBools(Number[] value3, Number[] value2, Number[] value1, Number[] value0)
        {
            _value3 = new NumberSet(value3);
            _value2 = new NumberSet(value2);
            _value1 = new NumberSet(value1);
            _value0 = new NumberSet(value0);

            _inverse3 = _value3.Inverse();
            _inverse2 = _value2.Inverse();
            _inverse1 = _value1.Inverse();
            _inverse0 = _value0.Inverse();

            _sets = new Func<NumberSet[]>[]
            {
                Null, Nor, Inhibition, NotB,
                RevInhibition, NotA, Xor, Nand,
                And, Xnor, TransferA, Implication,
                TransferB, RevImplication, Or, Identity
            };
            _inverseSets = new Func<NumberSet[]>[]
            {
                Identity, Or, RevImplication, TransferB,
                Implication, TransferA, Xnor, And,
                Nand, Xor, NotA, RevInhibition,
                NotB, Inhibition, Nor, Null
            };
        }
        public static ValuedBools CreateByPoints(Number A, Number B)
        {
            var aPts = A.PointsOfIntrest();
            var bPts = B.PointsOfIntrest();

            var ffa = new Number(A.BasisNumber, aPts[0], aPts[2]);
            var ffb = new Number(B.BasisNumber, bPts[0], bPts[2]);

            var fta = new Number(A.BasisNumber, aPts[2], aPts[3]);
            var ftb = new Number(B.BasisNumber, bPts[0], bPts[2]);

            var tfa = new Number(A.BasisNumber, aPts[0], aPts[2]);
            var tfb = new Number(B.BasisNumber, bPts[2], bPts[3]);

            var tta = new Number(A.BasisNumber, aPts[2], aPts[3]);
            var ttb = new Number(B.BasisNumber, bPts[2], bPts[3]);

            return new ValuedBools([ffa, ffb], [fta, ftb], [tfa, tfb], [tta, ttb]);
        }
        public static ValuedBools CreateByPositions(Number A, Number B)
        {
            var aPts = A.PointsOfIntrest();
            var bPts = B.PointsOfIntrest();

            var ffa = new Number(A.BasisNumber, aPts[0], aPts[2]);
            var ffb = new Number(B.BasisNumber, bPts[0], bPts[2]);

            var fta = new Number(A.BasisNumber, aPts[0], aPts[3]);
            var ftb = new Number(B.BasisNumber, bPts[0], bPts[2]);

            var tfa = new Number(A.BasisNumber, aPts[0], aPts[2]);
            var tfb = new Number(B.BasisNumber, bPts[0], bPts[3]);

            var tta = new Number(A.BasisNumber, aPts[0], aPts[3]);
            var ttb = new Number(B.BasisNumber, bPts[0], bPts[3]);

            return new ValuedBools([ffa, ffb], [fta, ftb], [tfa, tfb], [tta, ttb]);
        }
        public NumberSet[] this[int index]
        {
            get
            {
                if (index < 0 || index >= _sets.Length)
                    throw new IndexOutOfRangeException("Method index out of range.");
                return _sets[index]();
            }
        }

        // None, AOnly, BOnly, Both
        public NumberSet[] Null() => new[] { _inverse3, _inverse2, _inverse1, _inverse0 }; // 0000
        public NumberSet[] Nor() => new[] { _inverse3, _inverse2, _inverse1, _value0 }; // 0001
        public NumberSet[] Inhibition() => new[] { _inverse3, _inverse2, _value1, _inverse0 }; // 0010
        public NumberSet[] NotB() => new[] { _inverse3, _inverse2, _value1, _value0 }; // 0011
        public NumberSet[] RevInhibition() => new[] { _inverse3, _value2, _inverse1, _inverse0 }; // 0100
        public NumberSet[] NotA() => new[] { _inverse3, _value2, _inverse1, _value0 }; // 0101
        public NumberSet[] Xor() => new[] { _inverse3, _value2, _value1, _inverse0 }; // 0110
        public NumberSet[] Nand() => new[] { _inverse3, _value2, _value1, _value0 }; // 0111
        public NumberSet[] And() => new[] { _value3, _inverse2, _inverse1, _inverse0 }; // 1000
        public NumberSet[] Xnor() => new[] { _value3, _inverse2, _inverse1, _value0 }; // 1001
        public NumberSet[] TransferA() => new[] { _value3, _inverse2, _value1, _inverse0 }; // 1010
        public NumberSet[] Implication() => new[] { _value3, _inverse2, _value1, _value0 }; // 1011
        public NumberSet[] TransferB() => new[] { _value3, _value2, _inverse1, _inverse0 }; // 1100
        public NumberSet[] RevImplication() => new[] { _value3, _value2, _inverse1, _value0 }; // 1101
        public NumberSet[] Or() => new[] { _value3, _value2, _value1, _inverse0 }; // 1110
        public NumberSet[] Identity() => new[] { _value3, _value2, _value1, _value0 }; // 1111


        public static Func<Number, Number> NO_SWAP_POINTS = (left) => left.Clone();
        public static Func<Number, Number> SWAP_POINTS = (left) => left.SwapPoints();

        public static Func<Number, Number> MIRROR = (left) => left.Mirror();
        public static Func<Number, Number> MIRROR_START = (left) => left.MirrorStart();
        public static Func<Number, Number> MIRROR_END = (left) => left.MirrorEnd();

        public static Func<Number, Number> NEGATE = (left) => left.Negate();
        public static Func<Number, Number> NEGATE_START = (left) => left.NegateStart();
        public static Func<Number, Number> NEGATE_END = (left) => left.NegateEnd();


        public static Func<Number, Number> SWAP_MIRROR = (left) => left.SwapAndMirror();
        public static Func<Number, Number> SWAP_NEGATE = (left) => left.SwapAndMirror();

        public static Func<Number, Number> INVERT = (left) => left.Invert();
        public static Func<Number, Number> INVERT_MIRROR = (left) => left.InvertAndMirror();

        public static Func<Number, Number, Number> ADD = (left, right) => left + right;
        public static Func<Number, Number, Number> MULTIPLY = (left, right) => left * right;
        public static Func<Number, Number, Number> LENGTH = (left, right) => left.Length + right.Length;
        public static Func<Number, Number, Number> AREA = (left, right) => left.Length * right.Length;

        public Number[] Calculate2D(Func<Number, Number, Number> function, int filter, bool activeZero = false)
        {
            var result = new List<Number>();
            var sets = activeZero ? _sets[filter]() : _inverseSets[filter]();
            var flags = BitField.GetBoolValues(filter, sets.Length);
            for (int i = 0; i < sets.Length; i++)
            {
                if(flags[i] != activeZero)
                {
                    result.Add(function(sets[i][0], sets[i][1]));
                }
            }

            return result.ToArray();
        }
        public Number Calculate1D(Func<Number, Number> function, int filter, bool invert)
        {
            throw new NotImplementedException();
        }
    }

    [Flags]
    public enum QuadrantKind
    {
        Neither = 0,
        AOnly   = 1,
        BOnly   = 2,
        Both    = 4,
    }
}
