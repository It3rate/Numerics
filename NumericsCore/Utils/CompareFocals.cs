using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Utils;

public static class CompareFocals
{
    public static Focal? Matches(Focal a, Focal b) // B matches A by segment comparison (ignore direction)
    {
        Focal? result = null;
        if (a.Min == b.Min && a.Max == b.Max)
        {
            result = new(a.Min, a.Max);
        }
        return result;
    }
    public static Focal? Contains(Focal a, Focal b)  // B fits inside A
    {
        Focal? result = null;
        if (b.Min >= a.Min && b.Max <= a.Max)
        {
            result = new(b.Min, b.Max);
        }
        else if ((b.Min < a.Min && b.Max > a.Min) || (b.Max > a.Max && b.Min < a.Max))
        {
            result = new(Math.Max(b.Min, a.Min), Math.Min(b.Max, a.Max));
        }
        return result;
    }
    public static Focal? ContainedBy(Focal a, Focal b) // A fits inside B
    {
        Focal? result = null;
        if (a.Min >= b.Min && a.Max <= b.Max)
        {
            result = new(a.Min, a.Max);
        }
        else if ((a.Min < b.Min && a.Max > b.Min) || (a.Max > b.Max && a.Min < b.Max))
        {
            result = new(Math.Max(a.Min, b.Min), Math.Min(a.Max, b.Max));
        }
        return result;
    }
    public static Focal? GreaterThan(Focal a, Focal b) // all A values to right of B
    {
        Focal? result = null;
        if (a.Min > b.Max)
        {
            result = new(a.Min, a.Max);
        }
        else if (a.Max > b.Max)
        {
            result = new(Math.Max(a.Min, b.Max), a.Max);
        }
        return result;
    }
    public static Focal? GreaterThanOrEqual(Focal a, Focal b) // no part of A to left of B
    {
        Focal? result = null;
        if (a.Min >= b.Max)
        {
            result = new(a.Min, a.Max);
        }
        else if (a.Max > b.Max)
        {
            result = new(Math.Min(a.Min, b.Max), a.Max);
        }
        return result;
    }
    public static Focal? GreaterThanAndEqual(Focal a, Focal b) // no part of A to left of B, and part to the right of B (A overlap BMax)
    {
        Focal? result = null;
        if (a.Min < b.Max && a.Min > b.Min && a.Max > b.Max)
        {
            result = new(a.Min, a.Max);
        }
        else if (a.Min <= b.Min && a.Max > b.Max)
        {
            result = new(Math.Max(a.Min, b.Min), a.Max);
        }
        return result;
    }
    public static Focal? LessThan(Focal a, Focal b) // A all to left of B
    {
        Focal? result = null;
        if (a.Max < b.Min)
        {
            result = new(a.Min, a.Max);
        }
        else if (a.Min < b.Min)
        {
            result = new(a.Min, Math.Min(a.Max, b.Min));
        }
        return result;
    }
    public static Focal? LessThanAndEqual(Focal a, Focal b) // no part of A to right of B, and part to the left of B  (overlap left)
    {
        Focal? result = null;
        if (a.Min < b.Min && a.Max > b.Min && a.Max < b.Max)
        {
            result = new(a.Min, a.Max);
        }
        else if (a.Min <= b.Min && a.Max > b.Min)
        {
            result = new(a.Min, Math.Min(a.Max, b.Max));
        }
        return result;
    }
    public static Focal? LessThanOrEqual(Focal a, Focal b) // no part of A to right of B
    {
        Focal? result = null;
        if (a.Max <= b.Max)
        {
            result = new(a.Min, a.Max);
        }
        else if (a.Min <= b.Max)
        {
            result = new(a.Min, Math.Min(a.Max, b.Max));
        }
        else if (a.Min <= b.Min && a.Max > b.Min)
        {
            result = new(a.Min, Math.Min(a.Max, b.Max));
        }
        return result;
    }
}
