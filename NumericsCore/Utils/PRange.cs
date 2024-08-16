﻿
using System.Numerics;
using Numerics.Primitives;
using NumericsCore.Primitives;

namespace NumericsCore.Utils;

/// <summary>
/// A segment of two values, with the non aligned one using the inverted polarity.
/// </summary>
public struct PRange
{
    public static readonly PRange Empty = new PRange(0.0, 1.0);
    public static readonly PRange Zero = new PRange(0.0, 0.0);
    public static readonly PRange Unit = new PRange(0.0, 1.0);
    public static readonly PRange Unot = new PRange(1.0, 0.0);
    public static readonly PRange Umid = new PRange(-0.5, 0.5);
    public static readonly PRange MaxRange = new PRange(double.MaxValue, double.MaxValue);
    public static readonly PRange MinRange = new PRange(double.MinValue, double.MinValue);
    public static readonly double Tolerance = 0.000000001;

    public Polarity Polarity { get; private set; }
    public int PolarityDirection => Polarity.Direction();
    public bool IsAligned => Polarity == Polarity.Aligned;
    public bool IsInverted => Polarity == Polarity.Inverted;

    private readonly bool _hasValue;
    private double _startValue;
    private double _endValue;
    public double Start
    {
        get => _startValue;
        set => _startValue = value;
        //get => IsAligned ? _startValue : _endValue;
        //set { if (IsAligned) { _startValue = value; } else { _endValue = value; } }
    }
    public double End
    {
        get => _endValue;
        set => _endValue = value;
        //get => IsAligned ? _endValue : _startValue;
        //private set { if (IsAligned) { _endValue = value; } else { _startValue = value; } }
    }

    public float StartF => (float)Start;
    public float EndF => (float)End;
    public float RenderStart => (float)(IsAligned ? _startValue : -_startValue);
    public float RenderEnd => (float)(IsAligned ? _endValue : -_endValue);

    public PRange(PRange value, Polarity polarity = Polarity.Aligned) : this(value.Start, value.End, polarity) { }

    public PRange(double start, double end, Polarity polarity = Polarity.Aligned)
    {
        _hasValue = true;
        Polarity = polarity;
        Start = start;
        End = end;
    }
    public PRange(Number num) : this(num.StartValue, num.EndValue, num.Polarity) { }
    public static PRange FromNumber(Number num) => new(num);

    private PRange(bool isEmpty) // empty ctor
    {
        _hasValue = false;
        Polarity = Polarity.None;
    }


    public bool IsEmpty => _hasValue;
    public double Min => Start >= End ? End : Start;
    public double Max => End >= Start ? End : Start;
    public float MinF => (float)Min;
    public float MaxF => (float)Max;
    public double Length => PRange.AbsLength(this);
    public double DirectedLength() => PRange.DirectedLength(this);
    public double AbsLength() => PRange.AbsLength(this);
    public double TAtValue(double value)
    {
        var dist = value - (-Start);
        return dist / DirectedLength();
    }
    public PRange InvertStart() => new(-Start, End, Polarity);
    public PRange InvertEnd() => new(Start, -End, Polarity);
    public PRange InvertPolarity() => new(Start, End, Polarity.Invert());
    public PRange InvertRange() => new(-Start, -End, Polarity);
    public PRange InvertPolarityAndRange() => new(-Start, -End, Polarity.Invert());

    public PRange Negation() => PRange.Negation(this);
    public PRange Conjugate() => PRange.Conjugate(this);
    public PRange Reciprocal() => PRange.Reciprocal(this);
    public PRange Square() => PRange.Square(this);
    public PRange Normalize() => PRange.Normalize(this);
    public PRange NormalizeTo(PRange value) => PRange.NormalizeTo(this, value);
    public PRange ClampInner() => PRange.ClampInner(this);
    public PRange ClampOuter() => PRange.ClampOuter(this);
    public PRange Round() => PRange.Round(this);
    public PRange PositiveDirection() => PositiveDirection(this);
    public PRange NegativeDirection() => NegativeDirection(this);
    public bool IsZero => End == 0 && Start == 0;
    public bool IsZeroLength => (End == Start);
    public bool IsForward => End - Start > Tolerance;
    public bool IsBackward => Start - End > Tolerance;
    public bool IsPoint => Length < Tolerance;
    public double Direction => End >= Start ? 1.0 : -1.0;

    // because everything is segments, can add 'prepositions' (before, after, between, entering, leaving, near etc)
    public bool IsWithin(PRange value) => Start >= value.Start && End <= value.End;
    public bool IsBetween(PRange value) => Start > value.Start && End < value.End;
    public bool IsBefore(PRange value) => Start < value.Start && End < value.Start;
    public bool IsAfter(PRange value) => Start > value.End && End > value.End;
    public bool IsBeginning(PRange value) => Start <= value.Start && End > value.Start;
    public bool IsEnding(PRange value) => Start >= value.End && End > value.End;
    public bool IsTouching(PRange value) => (Start >= value.Start && Start <= value.End) || (End >= value.Start && End <= value.End);
    public bool IsNotTouching(PRange value) => !IsTouching(value);

    public PRange Invert() => new PRange(-_startValue, -_endValue, Polarity.Invert());
    public PRange Aligned => IsAligned ? this : Invert();
    public PRange Inverted => IsInverted ? this : Invert();
    public PRange ConvertToPolarity(Polarity target) => target == Polarity ? this : Invert();
    public PRange Reverse() => new PRange(_endValue, _startValue, Polarity);
    public PRange InvertReverse() => new PRange(-_endValue, -_startValue, Polarity.Invert());
    public PRange AlignedConjugate() => new PRange(_startValue, -_endValue, Polarity);
    public PRange InvertedConjugate() => new PRange(-_startValue, _endValue, Polarity);

    public static PRange operator +(PRange a, double value) => a + new PRange(0, value);
    public static PRange operator -(PRange a, double value) => a - new PRange(0, value);
    public static PRange operator *(PRange a, double value) => a * new PRange(0, value);
    public static PRange operator /(PRange a, double value) => a / new PRange(0, value);

    public static PRange PositiveDirection(PRange value) => new PRange(value.Min, value.Max);
    public static PRange NegativeDirection(PRange value) => new PRange(value.Max, value.Min);
    public static PRange Negate(PRange value) => -value;
    public static PRange Add(PRange left, PRange right) => left + right;
    public static PRange Subtract(PRange left, PRange right) => left - right;
    public static PRange Multiply(PRange left, PRange right) => left * right;
    public static PRange Divide(PRange dividend, PRange divisor) => dividend / divisor;

    public static PRange operator -(PRange value) => new PRange(-value.Start, -value.End);
    public static PRange operator +(PRange left, PRange right)
    {
        var result = left.Clone();
        result.Start += right.Start;
        result.End += right.End;
        return result;
    }
    public static PRange operator -(PRange left, PRange right)
    {
        var result = left.Clone();
        result.Start -= right.Start;
        result.End -= right.End;
        return result;
    }
    public static PRange operator *(PRange leftIn, PRange rightIn)
    {
        var polarity = (rightIn.Polarity == Polarity.Inverted) ? leftIn.Polarity.Invert() : leftIn.Polarity;
        var left = leftIn.ConvertToPolarity(polarity);
        var right = rightIn.ConvertToPolarity(polarity);
        var iVal = left._startValue * right._endValue + left._endValue * right._startValue;
        var rVal = left._endValue * right._endValue - left._startValue * right._startValue;
        //var result = polarity.IsTrue() ?
        //    new PRange(iVal, rVal, polarity) : new PRange(rVal, iVal, polarity);
        return new PRange(iVal, rVal, polarity);
    }
    public static PRange operator /(PRange leftIn, PRange rightIn)
    {
        var polarity = (rightIn.Polarity == Polarity.Inverted) ? leftIn.Polarity.Invert() : leftIn.Polarity;
        var left = leftIn.ConvertToPolarity(polarity);
        var right = rightIn.ConvertToPolarity(polarity);

        double real1 = left.End;
        double imaginary1 = left.Start;
        double real2 = right.End;
        double imaginary2 = right.Start;
        double iVal;
        double rVal;
        //var polarity = (right.Polarity == Polarity.Inverted) ? left.Polarity.Invert() : left.Polarity;
        if (Math.Abs(imaginary2) < Math.Abs(real2))
        {
            double num = imaginary2 / real2;
            iVal = (imaginary1 - real1 * num) / (real2 + imaginary2 * num);
            rVal = (real1 + imaginary1 * num) / (real2 + imaginary2 * num);
        }
        else
        {
            double num1 = real2 / imaginary2;
            iVal = (-real1 + imaginary1 * num1) / (imaginary2 + real2 * num1);
            rVal = (imaginary1 + real1 * num1) / (imaginary2 + real2 * num1);
        }
        //var result = polarity.IsTrue() ?
        //    new PRange(iVal, rVal, polarity) : new PRange(rVal, iVal, polarity);
        return new PRange(iVal, rVal, polarity);
    }
    public PRange SolvePolarityWith(Polarity right)
    {
        var result = this;
        if (Polarity == Polarity.Inverted && right == Polarity.Inverted)
        {
            result = InvertPolarityAndRange();
        }
        else if (Polarity == Polarity.Aligned && right == Polarity.Inverted)
        {
            result = InvertPolarity();
        }
        return result;
    }
    public static double DirectedLength(PRange a) => a.End + a.Start;
    public static double AbsLength(PRange a) => Math.Abs(a.End + a.Start);

    public static PRange Negation(PRange a) => new PRange(-a.Start, -a.End);
    public static PRange Conjugate(PRange a) => new PRange(a.Start, -a.End);
    public static PRange Reciprocal(PRange a) => a.End == 0.0 && a.Start == 0.0 ? PRange.Zero : PRange.Unit / a;
    public static PRange Square(PRange a) => a * a;
    public static PRange Normalize(PRange a) => a.IsZeroLength ? new PRange(0.5, 0.5) : a / a;
    public static PRange NormalizeTo(PRange from, PRange to) => from.Normalize() * to;

    public static PRange ClampInner(PRange a) => new PRange(Math.Floor(Math.Abs(a.Start)) * Math.Sign(a.Start), Math.Floor(Math.Abs(a.End)) * Math.Sign(a.End));
    public static PRange ClampOuter(PRange a) => new PRange(Math.Ceiling(Math.Abs(a.Start)) * Math.Sign(a.Start), Math.Ceiling(Math.Abs(a.End)) * Math.Sign(a.End));
    public static PRange Round(PRange a) => new PRange(Math.Round(a.Start), Math.Round(a.End));

    public static double Abs(PRange a)
    {
        if (double.IsInfinity(a.End) || double.IsInfinity(a.Start))
            return double.PositiveInfinity;
        double num1 = Math.Abs(a.End);
        double num2 = Math.Abs(a.Start);
        if (num1 > num2)
        {
            double num3 = num2 / num1;
            return num1 * Math.Sqrt(1.0 + num3 * num3);
        }
        if (num2 == 0.0)
            return num1;
        double num4 = num1 / num2;
        return num2 * Math.Sqrt(1.0 + num4 * num4);
    }

    public static PRange Pow(PRange value, PRange power)
    {
        if (power == PRange.Zero)
            return PRange.Unit;
        if (value == PRange.Zero)
            return PRange.Zero;
        double real1 = value.End;
        double imaginary1 = value.Start;
        double real2 = power.End;
        double imaginary2 = power.Start;
        double num1 = PRange.Abs(value);
        double num2 = Math.Atan2(imaginary1, real1);
        double num3 = real2 * num2 + imaginary2 * Math.Log(num1);
        double num4 = Math.Pow(num1, real2) * Math.Pow(Math.E, -imaginary2 * num2);
        return new PRange(num4 * Math.Sin(num3), num4 * Math.Cos(num3)); // todo: need to handle polarity
    }
    public static PRange Pow(PRange value, double power) => PRange.Pow(value, new PRange(0.0, power));
    private static PRange Scale(PRange value, double factor) => new PRange(factor * value.Start, factor * value.End);

    public bool IsSameDirection(PRange range) => Math.Abs(range.Direction + Direction) > Tolerance;
    public bool FullyContains(PRange toTest, bool includeEndpoints = true)
    {
        bool result = false;
        if (IsSameDirection(toTest))
        {
            var pd = PositiveDirection();
            var pdTest = toTest.PositiveDirection();
            result = includeEndpoints ? pdTest.IsWithin(pd) : pdTest.IsBetween(pd);
        }
        return result;
    }

    public float Midpoint() => (EndF - StartF) / 2f + StartF;
    public float SampleRandom(Random rnd) => (EndF - StartF) * (float)rnd.NextDouble() + StartF;
    public float SampleStdDev() => (EndF - StartF) / 2f + StartF; // todo: add standard deviation sampler

    #region Conversions
    public Focal ToFocal() => new((long)Start, (long)End);
    public static PRange FromFocal(Focal value) => new(value.FirstTick, value.LastTick);
    public Number ToNumber(Domain domain) => new(
        domain, 
        domain.FocalFromValues(Start, End), 
        Polarity);
    #endregion

    #region Equality
    public PRange Clone() => new PRange(Start, End, Polarity);

    public static bool operator ==(PRange a, PRange b) // value type, so no nulls
    {
        return a.Equals(b);
    }
    public static bool operator !=(PRange a, PRange b) => !(a == b);
    public override bool Equals(object obj)
    {
        return obj is PRange other && Equals(other);
    }
    public bool Equals(PRange value)
    {
        return Start.Equals(value.Start) && End.Equals(value.End) && Polarity.Equals(value.Polarity);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Start.GetHashCode();
            hashCode = (hashCode * 397) ^ End.GetHashCode();
            hashCode = (hashCode * 17) ^ Polarity.GetHashCode();
            return hashCode;
        }
    }
    #endregion

    public override string ToString()
    {
        //var prefix = Polarity == Polarity.Inverted ? "~" : "";
        //var midSign = End > 0 ? " + " : " ";
        //return $"{prefix}[{Start:0.00}{midSign}{End:0.00}]";

        string result;
        if (Polarity == Polarity.None)
        {
            result = $"x({Start:0.##}_{End:0.##})"; // no polarity, so just list values
        }
        else
        {
            //var midSign = End > 0 ? " + " : " ";
            //var invert = IsAligned ? "+" : "~";
            //result = $"{invert}({Start:0.##}s{midSign}{End:0.##}e)";

            var midSign = End > 0 ? " + " : " ";
            var pol = Polarity == Polarity.Inverted ? "~" : "";
            var start = Start == 0 ? "0" : $"{Start:0.##}";
            var end = End == 0 ? "0" : $"{End:0.##}";
            result = $"{pol}({start}s{midSign}{end}e)";
        }
        return result;
    }

}
