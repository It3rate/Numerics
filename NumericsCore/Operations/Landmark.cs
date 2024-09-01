using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Primitives;

namespace NumericsCore.Structures
{
    // t is a multiply op (number as basis), fixed offset is addition (number as basis), indexed is power (repetition)
    public interface ILandmark
    {
        Number Reference { get; }
        double T { get; }
        double Value { get; }
        Polarity Polarity { get; }
        Number AsNumber();
    }
    public abstract class LandmarkBase : ILandmark
    {
        // having a basis offset and scalar would allow a domain's valance and weighting to adjust without changing the measured value.
        public abstract Number Reference { get; }
        public double T { get; protected set; }
        public double Value => Reference.ValueAtT(T);// check for 0 and 1 as these are going to be common, and could cause polyline .
        public Polarity Polarity => Reference.Polarity;

        public Number AsNumber() => new Number(Reference.Domain, Reference.Domain.FocalFromDecimalRaw(0, Value));

    }
    /// <summary>
    /// References a position on a Number segment. T causes the segment to be treated as the basis.
    /// </summary>
    public class Landmark : LandmarkBase
    {
        // maybe 'points' ref specific index, and segments with length allow ops on multiple values (average, max etc)
        public override Number Reference { get; }
        public Landmark(Number reference, double t)
        {
            Reference = reference;
            T = t;
        }
    }
    /// <summary>
    /// References a position on an indexed Number segment. Negative numbers reference from the end of the store.  T causes the segment to be treated as the basis.
    /// </summary>
    public class LandmarkByIndex : LandmarkBase
    { 
        public override Number Reference => Index >= 0 ? Store[Index] : Store[^Index]; // todo: account for missing index
        public List<Number> Store { get; } // store is passed allowing local library reusable shapes
        public int Index { get; }
        public LandmarkByIndex(List<Number> store, int index, double t)
        {
            Store = store;
            Index = index;
            T = t;
        }
    }
}
