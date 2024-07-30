﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace TestNumerics;

[TestClass]
public class NumbersTestsV1
{
    private Trait _trait;
    private Focal _unitFocal;
    private Focal _maxMin;
    private Domain _domain;
    private static double _delta = 0.001;

    [TestInitialize]
    public void Init()
    {
        _trait = new Trait("number tests v1");
        _unitFocal = new Focal(0, 10);
        _maxMin = new Focal(-1000, 1010);
        _domain = new Domain(_trait, _unitFocal, _maxMin);
    }
    [TestMethod]
    public void UnitChangePositionTests()
    {
        var n0 = new Number(_domain, new (0, 20));
        var n1 = new Number(_domain, new (20, 0));
        var n2 = new Number(_domain, new (-30, 20));
        var n3 = new Number(_domain, new (-20, -30));
        Assert.AreEqual(0, n0.StartValue);
        Assert.AreEqual(2, n0.EndValue);
        Assert.AreEqual(-2, n1.StartValue);
        Assert.AreEqual(0, n1.EndValue);
        Assert.AreEqual(3, n2.StartValue);
        Assert.AreEqual(2, n2.EndValue);
        Assert.AreEqual(2, n3.StartValue);
        Assert.AreEqual(-3, n3.EndValue);
        _unitFocal.LastTick = 20;
        Assert.AreEqual(0, n0.StartValue);
        Assert.AreEqual(1, n0.EndValue);
        Assert.AreEqual(-1, n1.StartValue);
        Assert.AreEqual(0, n1.EndValue);
        Assert.AreEqual(1.5, n2.StartValue);
        Assert.AreEqual(1, n2.EndValue);
        Assert.AreEqual(1, n3.StartValue);
        Assert.AreEqual(-1.5, n3.EndValue);
        _unitFocal.LastTick = -20;
        Assert.AreEqual(0, n0.StartValue);
        Assert.AreEqual(-1, n0.EndValue);
        Assert.AreEqual(1, n1.StartValue);
        Assert.AreEqual(0, n1.EndValue);
        Assert.AreEqual(-1.5, n2.StartValue);
        Assert.AreEqual(-1, n2.EndValue);
        Assert.AreEqual(-1, n3.StartValue);
        Assert.AreEqual(1.5, n3.EndValue);
        _unitFocal.FirstTick = -10; // unot perspective
        Assert.AreEqual(1, n0.StartValue);
        Assert.AreEqual(-3, n0.EndValue);
        Assert.AreEqual(3, n1.StartValue);
        Assert.AreEqual(-1, n1.EndValue);
        Assert.AreEqual(-2, n2.StartValue);
        Assert.AreEqual(-3, n2.EndValue);
        Assert.AreEqual(-1, n3.StartValue);
        Assert.AreEqual(2, n3.EndValue);
        _unitFocal.FirstTick = 2000; // unot perspective
        _unitFocal.LastTick = -2000; // forces things to about the middle
        Assert.AreEqual(-0.5, n0.StartValue);
        Assert.AreEqual(0.495, n0.EndValue);
        Assert.AreEqual(-0.495, n1.StartValue);
        Assert.AreEqual(0.5, n1.EndValue);
        Assert.AreEqual(-0.5075, n2.StartValue);
        Assert.AreEqual(0.495, n2.EndValue);
        Assert.AreEqual(-0.505, n3.StartValue);
        Assert.AreEqual(0.5075, n3.EndValue);
    }
    [TestMethod]
    public void UnitChangeValueTests()
    {
        var n0 = new Number(_domain, _domain.FocalFromDecimal(0, 20));
        var n1 = new Number(_domain, _domain.FocalFromDecimal(20, 0));
        var n2 = new Number(_domain, _domain.FocalFromDecimal(-30, 20));
        var n3 = new Number(_domain, _domain.FocalFromDecimal(-20, -30));
        Assert.AreEqual(0, n0.StartValue);
        Assert.AreEqual(20, n0.EndValue);
        Assert.AreEqual(20, n1.StartValue);
        Assert.AreEqual(0, n1.EndValue);
        Assert.AreEqual(-30, n2.StartValue);
        Assert.AreEqual(20, n2.EndValue);
        Assert.AreEqual(-20, n3.StartValue);
        Assert.AreEqual(-30, n3.EndValue);
        _unitFocal.FirstTick = 100; // unot perspective
        _unitFocal.LastTick = 0;
        Assert.AreEqual(-1, n0.StartValue);
        Assert.AreEqual(-1, n0.EndValue);
        Assert.AreEqual(-3, n1.StartValue);
        Assert.AreEqual(1, n1.EndValue);
        Assert.AreEqual(2, n2.StartValue);
        Assert.AreEqual(-1, n2.EndValue);
        Assert.AreEqual(1, n3.StartValue);
        Assert.AreEqual(4, n3.EndValue);
    }
    [TestMethod]
    public void PowerTests()
    {
        var a = new System.Numerics.Complex(0, 1);
        var b = Complex.Pow(a, a);
        Console.WriteLine( b);
    }
    [TestMethod]
    public void CoreNumberTests()
    {
        var f0 = new Focal(0, 20);
        var n0 = new Number(_domain, f0);
        var f1 = new Focal(0, 30);
        var n1 = new Number(_domain, f1);
        var f2 = new Focal(-32, 0);
        var n2 = new Number(_domain, f2);
        var f3 = new Focal(-50, 45);
        var n3 = new Number(_domain, f3);
        var f4 = new Focal(50, -45);
        var n4 = new Number(_domain, f4);
        var f5 = new Focal(53, 69);
        var n5 = new Number(_domain, f5);

        var n0b = n0.Clone();
        Assert.AreEqual(n0b, n0);
        n0b = n0b + n3;// n0b.Add(n3);
        Assert.AreNotEqual(n0, n0b);

        Assert.AreEqual(_unitFocal.AbsTickLength, n0.Domain.BasisFocal.AbsTickLength);
        Assert.AreEqual(1, n0.PositiveTickDirection);
        Assert.AreEqual(1, n4.PositiveTickDirection);
        Assert.AreEqual(n1.Domain, n0.Domain);
        Assert.AreEqual(n0.StartValue, n1.StartValue, _delta);
        Assert.AreEqual(n1.EndValue, n1.Focal.TickLength / (double)n1.Domain.BasisFocal.TickLength);
        //Assert.AreEqual(0, n3.RemainderStartValue, Utils.Tolerance);
        //Assert.AreEqual(5, n3.RemainderEndValue, Utils.Tolerance);
        //Assert.AreEqual(2, n2.RemainderStartValue, Utils.Tolerance);
        //Assert.AreEqual(0, n2.RemainderEndValue, Utils.Tolerance);

        //Assert.AreEqual(3, n2.CeilingRange.Start, Utils.Tolerance);
        //Assert.AreEqual(0, n2.CeilingRange.End, Utils.Tolerance);
        //Assert.AreEqual(5, n3.FloorRange.Start, Utils.Tolerance);
        //Assert.AreEqual(4, n3.FloorRange.End, Utils.Tolerance);
        //Assert.AreEqual(-5, n5.RoundedRange.Start, Utils.Tolerance);
        //Assert.AreEqual(7, n5.RoundedRange.End, Utils.Tolerance);
        //Assert.AreEqual(-0.3, n5.RemainderRange.Start, Utils.Tolerance);
        //Assert.AreEqual(0.9, n5.RemainderRange.End, Utils.Tolerance);

        Assert.IsTrue(n3.IsAligned);
        Assert.IsTrue(_domain.BasisNumber.IsAligned);
        Assert.IsTrue(n4.IsAligned);
        Assert.IsFalse(n4.IsInverted);
        //Assert.AreEqual(0.472636, n3.RangeInMinMax.Start, Utils.Tolerance);
        //Assert.AreEqual(0.519900, n3.RangeInMinMax.End, Utils.Tolerance);

        //n1.Subtract(n2);
        //Assert.AreEqual(new PRange(-3.2, 3), n1.Value);
        //n2.Add(n3);
        //Assert.AreEqual(new PRange(5, 4.5), n3.Value);
        //n3.Multiply(n4);
        //Assert.AreEqual(new PRange(-45, 4.8), n3.Value);
        //n4.Divide(n5);
        //Assert.AreEqual(new PRange(-0.8, -0.1), n4.Value);
    }
}
