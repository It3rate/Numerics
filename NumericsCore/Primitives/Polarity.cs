﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumericsCore.Utils;

namespace NumericsCore.Primitives;

public enum Polarity { None, Aligned, Inverted };

public static class PolarityExtension
{
    public static bool HasPolarity(this Polarity polarity) => polarity != Polarity.None;
    public static bool IsTrue(this Polarity polarity) => polarity == Polarity.Aligned;
    public static bool IsFalse(this Polarity polarity) => polarity == Polarity.Inverted;
    public static int Direction(this Polarity polarity) => polarity == Polarity.Aligned ? 1 : polarity == Polarity.Inverted ? -1 : 0;
    public static int ForceValue(this Polarity polarity) => polarity == Polarity.Inverted ? -1 : 1;

    public static Polarity Invert(this Polarity polarity)
    {
        return polarity switch
        {
            Polarity.Aligned => Polarity.Inverted,
            Polarity.Inverted => Polarity.Aligned,
            _ => polarity
        };
    }
}

public interface IPolarityOperators<TSelf, TOther, TResult>
    where TSelf : IPolarityOperators<TSelf, TOther, TResult>?
{
    static abstract TResult operator ~(TSelf value);
    TResult Invert();
    TResult InvertNegate();
}

//public interface IBitwiseOperators<TSelf, TOther, TResult> where TSelf : IBitwiseOperators<TSelf, TOther, TResult>?
//{
//    static abstract TResult operator ~(TSelf value);
//    static abstract TResult operator &(TSelf left, TOther right);
//    static abstract TResult operator |(TSelf left, TOther right);
//    static abstract TResult operator ^(TSelf left, TOther right);
//}