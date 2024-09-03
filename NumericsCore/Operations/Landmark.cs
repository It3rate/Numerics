using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Primitives;
using NumericsCore.Utils;

namespace NumericsCore.Structures
{

    public interface IValueRef
    {
        double Value { get; }
        bool NeedsUpdate { get; set; }
    }
    public struct ValueRef : IValueRef
    {
        public double Value { get; }
        public bool NeedsUpdate { get; set; } = false;
        public ValueRef(double value) {  Value = value; }
    }
    public class FocalVal : IValueRef
    {
        public Focal Basis { get; }
        public Focal Focal { get; }
        public long Tick { get; }
        public bool IsStart { get; } // start must be negative, end positive, so polarity value?
        public double Value => 0;//calc
        public bool NeedsUpdate { get; set; } = true;
    }

    // t is a multiply op (number as basis), fixed offset is addition (number as basis), indexed is power (repetition)
    public interface ILandmark : IValueRef
    {
        // maybe a landmark always has to be a segment? solves the inverted start issue, and it gets its own focal
        Number Reference { get; }
        double T { get; }
        Number PartialNumber();
    }
    public abstract class LandmarkBase : ILandmark
    {
        // the main thing is a landmark is relative to a number or focal's value - the start of the number is 0 and the end is 1 (it is the basis). Otherwise just use a number.
        // using a focal doesn't allow limits, using a number allows limits, clamping, resolution independence. So we use a number.
        public abstract Number Reference { get; }
        public double T { get; protected set; }
        // these Values are always aligned to the basis
        public double Value => Reference.AsBasisTValue(T);// check for 0 and 1 as these are going to be common, and could cause polyline calculation cascade.
        public bool NeedsUpdate { get; set; } = true;// timestamp changes to Number (reference), and check this timestamp. Allows holding old values. Have option of locking value and not updating.

        public Number PartialNumber() => new Number(Reference.Domain, Reference.Domain.FocalFromDecimalRaw(0, Value));

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
