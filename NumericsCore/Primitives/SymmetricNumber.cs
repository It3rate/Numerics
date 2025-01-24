using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Primitives
{
    public class SymmetricNumber
    {
        public Domain Domain;

        public Focal TopFocal{ get; }
        public Focal BottomFocal { get; }
        public long ZeroOffset { get; } = 0; // running average, or unobstructed average

        public Tuple<long, long, long, long> TickSet => new Tuple<long, long, long, long>(EndValueTicks, StartValueTicks, EndUnitTicks, StartUnitTicks);
        private long EndValueTicks => TopFocal.EndTick - ZeroOffset;
        private long StartValueTicks => -(TopFocal.StartTick - ZeroOffset); // positive is always in the direction of the unit
        private long EndUnitTicks => BottomFocal.EndTick - ZeroOffset;
        private long StartUnitTicks => -(BottomFocal.StartTick - ZeroOffset);

        // landmark is two focals and a double?

        public Number Segment => throw new NotImplementedException();

        public SymmetricNumber(Domain domain, Focal topFocal, Focal bottomFocal, long zeroOffset = 0)
        {
            Domain = domain;
            TopFocal = topFocal;
            BottomFocal = bottomFocal;
            ZeroOffset = zeroOffset;
        }
        public SymmetricNumber(Domain domain, long start, long end, long unitLength)
        {
            Domain = domain;
            TopFocal = new Focal(-unitLength, end);
            BottomFocal = new Focal(start, unitLength);
            ZeroOffset = 0;
        }
        public SymmetricNumber(Domain domain, long start, long endUnit, long startUnit, long end, long zeroOffset = 0)
        {
            Domain = domain;
            TopFocal = new Focal(endUnit + zeroOffset, end + zeroOffset);
            BottomFocal = new Focal(start + zeroOffset, startUnit + zeroOffset);
            ZeroOffset = zeroOffset;
        }

        public double StartValue => StartValueTicks / (double)StartUnitTicks;
        public double EndValue => EndValueTicks / (double)EndUnitTicks;


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
            var result = new SymmetricNumber(Domain, new Focal((long)startTick, (long)endTick), BottomFocal);
            return result;
        }
    }

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
}
