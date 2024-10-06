
using System;
using NumericsCore.Primitives;
using NumericsCore.Utils;

namespace Numerics.Primitives;

// this constrains the significant figures of a measurement, unit/not is the minimum tick size, and range is the max possible.
// Feels like this should just be the unit chosen - that works from min tick size, but how to specify a max value? On the trait I guess?
// the trait then needs to 'know' how measurable it is, and the units are calibrated to that, which seems overspecified
// (eg 'length' knows a nanometer is min and a light year max, cms and inches calibrate to this. Hmm, no).
// So each 'situation' has sig-fig/precision metadata. Working in metal units vs working in wood units. A metal length trait and a wood length trait, convertible.
// This is what domains are. BasisFocal size(s), min tick size, max ranges, confidence/probability data at limits of measure (gaussian precision etc).
// E.g. changing the domain 'tolerance' could change neat writing into messy.

// Min size is tick size. BasisFocal is start/end point (only one focal allowed for a unit). MinMaxFocal is bounds in ticks. todo: add conversion methods etc.
public class Domain : IEquatable<Domain>
{
    public Trait? Trait { get; } // todo: Probably merge domain and trait?
    public Number DefaultBasisNumber { get; }
    public Focal DefaultBasisFocal => DefaultBasisNumber.Focal;
    private Focal DefaultLimitsFocal { get; }
    private Domain(Focal basisFocal, Focal limitsFocal) : this(Trait.WorkingTrait, basisFocal, limitsFocal) { }
    public Domain(Trait? trait, Focal basisFocal, Focal limitsFocal)
    {
        Trait = trait;
        DefaultLimitsFocal = limitsFocal;
        DefaultBasisNumber = Number.CreateDomainNumber(this, basisFocal);
    }

    #region Properties
    //public Polarity Polarity => 
    //    DefaultBasisFocal.Direction > 0 ? Polarity.Aligned :
    //    DefaultBasisFocal.Direction < 0 ? Polarity.Inverted : Polarity.None;

    // IsTickLessThanBasis, IsBasisInMinmax, IsTiling, IsClamping, IsInvertable, IsNegateable, IsPoly, HasTrait
    #endregion
    #region Transformations
    private Domain? _inverse;
    public Domain Inverse
    {
        get
        {
            if ((_inverse == null))
            {
                _inverse = new Domain(Trait, DefaultBasisFocal.BasisInverse, DefaultLimitsFocal.InvertClone());
                _inverse._inverse = this;
            }
            return _inverse;        
        }
    }
    #endregion

    public static readonly Domain SCALAR_DOMAIN = new Domain(Trait.ScalarTrait, Focal.One, Focal.MaxFocal);

    #region Equality
    public Domain Clone() => new Domain(Trait, DefaultBasisFocal.Clone(), DefaultLimitsFocal.Clone());
    public override bool Equals(object? obj)
    {
        return Equals(obj as Domain);
    }

    public bool Equals(Domain? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Trait == other.Trait &&
               EqualityComparer<Focal>.Default.Equals(DefaultBasisFocal, other.DefaultBasisFocal) &&
               EqualityComparer<Focal>.Default.Equals(DefaultLimitsFocal, other.DefaultLimitsFocal);
    }
    public static bool operator ==(Domain? left, Domain? right)
    {
        return EqualityComparer<Domain>.Default.Equals(left, right);
    }
    public static bool operator !=(Domain? left, Domain? right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (Trait?.GetHashCode() ?? 0);
            hash = hash * 23 + DefaultBasisFocal.GetHashCode();
            hash = hash * 23 + DefaultLimitsFocal.GetHashCode();
            return hash;
        }
    }
    #endregion
}
