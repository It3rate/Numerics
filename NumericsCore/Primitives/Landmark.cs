using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerics.Primitives
{
    /// <summary>
    /// References a position on a Focal.
    /// </summary>
    public struct Landmark
    {
        public IMeasurable Reference { get; }
        public long Position { get; }
        public double T => (Position - Reference.StartTick) / (double)Reference.TickLength;
        public Landmark(IMeasurable reference, long position)
        {
            Reference = reference;
            Position = position;
        }
        public Landmark(in IMeasurable reference, double t)
        {
            Reference = reference;
            Position = PostionFromT(t);
        }
        private long PostionFromT(double t) => (long)(Reference.TickLength * t) + Reference.StartTick;
    }
}
