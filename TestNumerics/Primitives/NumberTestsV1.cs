using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Primitives;

namespace TestNumerics.Primitives;

[TestClass]
public class NumbersTestsV1
{
    private Trait _trait;
    private Focal _unitFocal;
    private Focal _maxMin;
    private Domain _domain;
    private Domain _invDomain;
    private static double _delta = 0.001;

    [TestInitialize]
    public void Init()
    {
        _trait = new Trait("number tests v1");
        _unitFocal = new Focal(0, 10);
        _maxMin = new Focal(-1000, 1010);
        _domain = new Domain(_trait, _unitFocal, _maxMin);
        _invDomain = _domain.Inverse;
    }
    [TestMethod]
    public void UnitChangePositionTests()
    {
        var n0 = new Number(_domain.DefaultBasisNumber, new(0, 20));
        var n1 = new Number(_domain.DefaultBasisNumber, new(20, 0));
        var n2 = new Number(_domain.DefaultBasisNumber, new(-30, 20));
        var n3 = new Number(_domain.DefaultBasisNumber, new(-20, -30));
        Assert.AreEqual(0, n0.StartValue);
        Assert.AreEqual(2, n0.EndValue);
        Assert.AreEqual(-2, n1.StartValue);
        Assert.AreEqual(0, n1.EndValue);
        Assert.AreEqual(3, n2.StartValue);
        Assert.AreEqual(2, n2.EndValue);
        Assert.AreEqual(2, n3.StartValue);
        Assert.AreEqual(-3, n3.EndValue);
        _unitFocal.ModifyEnd(20);
        Assert.AreEqual(0, n0.StartValue);
        Assert.AreEqual(1, n0.EndValue);
        Assert.AreEqual(-1, n1.StartValue);
        Assert.AreEqual(0, n1.EndValue);
        Assert.AreEqual(1.5, n2.StartValue);
        Assert.AreEqual(1, n2.EndValue);
        Assert.AreEqual(1, n3.StartValue);
        Assert.AreEqual(-1.5, n3.EndValue);
        _unitFocal.ModifyEnd(-20);
        Assert.AreEqual(0, n0.StartValue);
        Assert.AreEqual(-1, n0.EndValue);
        Assert.AreEqual(1, n1.StartValue);
        Assert.AreEqual(0, n1.EndValue);
        Assert.AreEqual(-1.5, n2.StartValue);
        Assert.AreEqual(-1, n2.EndValue);
        Assert.AreEqual(-1, n3.StartValue);
        Assert.AreEqual(1.5, n3.EndValue);
        _unitFocal.ModifyStart(-10);
        Assert.AreEqual(1, n0.StartValue);
        Assert.AreEqual(-3, n0.EndValue);
        Assert.AreEqual(3, n1.StartValue);
        Assert.AreEqual(-1, n1.EndValue);
        Assert.AreEqual(-2, n2.StartValue);
        Assert.AreEqual(-3, n2.EndValue);
        Assert.AreEqual(-1, n3.StartValue);
        Assert.AreEqual(2, n3.EndValue);
        _unitFocal.ModifyStart(2000);
        _unitFocal.ModifyEnd(-2000);
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
        var n0 = new Number(_domain.DefaultBasisNumber, _domain.FocalFromDecimalSigned(0, 20));
        var n1 = new Number(_domain.DefaultBasisNumber, _domain.FocalFromDecimalSigned(20, 0));
        var n2 = new Number(_domain.DefaultBasisNumber, _domain.FocalFromDecimalSigned(-30, 20));
        var n3 = new Number(_domain.DefaultBasisNumber, _domain.FocalFromDecimalSigned(-20, -30));
        Assert.AreEqual(0, n0.StartValue);
        Assert.AreEqual(20, n0.EndValue);
        Assert.AreEqual(20, n1.StartValue);
        Assert.AreEqual(0, n1.EndValue);
        Assert.AreEqual(-30, n2.StartValue);
        Assert.AreEqual(20, n2.EndValue);
        Assert.AreEqual(-20, n3.StartValue);
        Assert.AreEqual(-30, n3.EndValue);
        _unitFocal.ModifyStart(100);
        _unitFocal.ModifyEnd(0);
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
    public void CoreNumberTests()
    {
        var f0 = new Focal(0, 20);
        var n0 = new Number(_domain.DefaultBasisNumber, f0);
        var f1 = new Focal(0, 30);
        var n1 = new Number(_domain.DefaultBasisNumber, f1);
        var f2 = new Focal(-32, 0);
        var n2 = new Number(_domain.DefaultBasisNumber, f2);
        var f3 = new Focal(-50, 45);
        var n3 = new Number(_domain.DefaultBasisNumber, f3);
        var f4 = new Focal(50, -45);
        var n4 = new Number(_domain.DefaultBasisNumber, f4);
        var f5 = new Focal(53, 69);
        var n5 = new Number(_domain.DefaultBasisNumber, f5);

        var n0b = n0.Clone();
        Assert.AreEqual(n0b, n0);
        n0b = n0b + n3;// n0b.Add(n3);
        Assert.AreNotEqual(n0, n0b);

        Assert.AreEqual(_unitFocal.AbsLength, n0.Domain.AbsBasisLength);
        Assert.AreEqual(1, n0.BasisDirection);
        Assert.AreEqual(1, n4.BasisDirection);
        Assert.AreEqual(n1.Domain, n0.Domain);
        Assert.AreEqual(n0.StartValue, n1.StartValue, _delta);
        Assert.AreEqual(n1.EndValue, n1.Focal.Length / (double)n1.Domain.BasisLength);
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
        Assert.IsTrue(_domain.DefaultBasisNumber.IsAligned);
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
    [TestMethod]
    public void UnitByAlignedTests()
    {
        _unitFocal = new Focal(0, 100);
        _maxMin = new Focal(-10000, 10000);
        _domain = new Domain(_trait, _unitFocal, _maxMin);
        _invDomain = _domain.Inverse;
        var n = new Number(_domain.DefaultBasisNumber, new(200, 300)); // (-2i+3)

        // preserve length: * 1, * -1, * ~(1), * ~(-1), 
        // anything where the 'real' part is one and imaginary part is zero preserves length, independent of polarity.
        var a_pos1 = new Number(_domain.DefaultBasisNumber, new(0, 100)); // ~seg from 0i->1
        var rap1 = n * a_pos1; // (1)
        Assert.IsTrue(rap1.IsAligned);
        Assert.AreEqual(-2, rap1.StartValue); // i
        Assert.AreEqual(3, rap1.EndValue);

        var a_neg1 = new Number(_domain.DefaultBasisNumber, new(0, -100)); // ~seg from 0i->-1
        var ran1 = n * a_neg1; // (-1)
        Assert.IsTrue(ran1.IsAligned);
        Assert.AreEqual(2, ran1.StartValue); // i
        Assert.AreEqual(-3, ran1.EndValue);

        var i_neg1 = new Number(_invDomain.DefaultBasisNumber, new(-100, 0)); // ~(-i)
        var r0 = n * i_neg1; // (-2i + 3) * (i) = (3i + 2)
        Assert.IsTrue(r0.IsAligned);
        Assert.AreEqual(3, r0.StartValue);
        Assert.AreEqual(2, r0.EndValue); // i

        var i_pos1 = new Number(_invDomain.DefaultBasisNumber, new(100, 0)); // ~(i)
        var r1 = n * i_pos1; // converts to => (-2i+3)* (-i) = ~(-2i-3)
        Assert.IsTrue(r1.IsAligned);
        Assert.AreEqual(-3, r1.StartValue);
        Assert.AreEqual(-2, r1.EndValue); // i



        // change length: * (i), * ~(-i), * ~(i), * (-i)
        var a_posi = new Number(_domain.DefaultBasisNumber, new(-100, 0));// seg from i to 0
        var r5 = n * a_posi; // +(i)
        Assert.IsTrue(r5.IsAligned);
        Assert.AreEqual(3, r5.StartValue); // i
        Assert.AreEqual(2, r5.EndValue);

        var a_negi = new Number(_domain.DefaultBasisNumber, new(100, 0));// seg from -i to 0
        var r2 = n * a_negi; // +(-i)
        Assert.IsTrue(r2.IsAligned);
        Assert.AreEqual(-3, r2.StartValue); // i
        Assert.AreEqual(-2, r2.EndValue);

        var i_posi = new Number(_invDomain.DefaultBasisNumber, new(0, -100)); // seg i
        var r4 = n * i_posi; // (-2i + 3) * (0i + -1) = (2i - 3)
        Assert.IsTrue(r4.IsAligned);
        Assert.AreEqual(2, r4.StartValue);
        Assert.AreEqual(-3, r4.EndValue); // i

        var i_negi = new Number(_invDomain.DefaultBasisNumber, new(0, 100)); // seg -i
        var r3 = n * i_negi; // ~(-i)
        Assert.IsTrue(r3.IsAligned);
        Assert.AreEqual(-2, r3.StartValue);
        Assert.AreEqual(3, r3.EndValue); // i
    }
    [TestMethod]
    public void UnitByInvertedTests()
    {
        _unitFocal = new Focal(0, 100);
        _maxMin = new Focal(-10000, 10000);
        _domain = new Domain(_trait, _unitFocal, _maxMin);
        _invDomain = _domain.Inverse;
        var n = new Number(_invDomain.DefaultBasisNumber, new(200, 300)); // ~(2-3i)
        Assert.IsTrue(n.IsInverted);
        Assert.AreEqual(2, n.StartValue);
        Assert.AreEqual(-3, n.EndValue); // i

        // preserve length: * 1, * -1, * ~(1), * ~(-1), 
        var a_pos1 = new Number(_domain.DefaultBasisNumber, new(0, 100)); // ~seg from 0i->1
        var rap1 = n * a_pos1; // (2i - 3) * (0i - 1) = (-2i + 3)
        Assert.IsTrue(rap1.IsInverted);
        Assert.AreEqual(-2, rap1.StartValue); // i
        Assert.AreEqual(3, rap1.EndValue);

        var a_neg1 = new Number(_domain.DefaultBasisNumber, new(0, -100)); // ~seg from 0i->-1
        var ran1 = n * a_neg1; // (2i - 3) * (1) = (2i - 3)
        Assert.IsTrue(ran1.IsInverted);
        Assert.AreEqual(2, ran1.StartValue); // i
        Assert.AreEqual(-3, ran1.EndValue);

        var i_neg1 = new Number(_invDomain.DefaultBasisNumber, new(100, 0));
        var xx = i_neg1.ToString();
        var r0 = n * i_neg1; // (2i - 3) * (i) = (-3i - 2)
        Assert.IsTrue(r0.IsInverted);
        Assert.AreEqual(-3, r0.StartValue);
        Assert.AreEqual(-2, r0.EndValue); // i

        var i_pos1 = new Number(_invDomain.DefaultBasisNumber, new(-100, 0));
        var r1 = n * i_pos1; // (2i - 3) * (-i)
        Assert.IsTrue(r1.IsInverted);
        Assert.AreEqual(3, r1.StartValue);
        Assert.AreEqual(2, r1.EndValue); // i



        // change length: * (i), * ~(-i), * ~(i), * (-i)
        var a_posi = new Number(_domain.DefaultBasisNumber, new(-100, 0));// seg from i to 0
        var r5 = n * a_posi; // (2i - 3) * (-i)
        Assert.IsTrue(r5.IsInverted);
        Assert.AreEqual(3, r5.StartValue);
        Assert.AreEqual(2, r5.EndValue); // i

        var a_negi = new Number(_domain.DefaultBasisNumber, new(100, 0));// seg from -i to 0
        var r2 = n * a_negi; // (2i - 3) * (i)
        Assert.IsTrue(r2.IsInverted);
        Assert.AreEqual(-3, r2.StartValue);
        Assert.AreEqual(-2, r2.EndValue); // i

        var i_posi = new Number(_invDomain.DefaultBasisNumber, new(0, 100)); // seg i
        var r4 = n * i_posi; // (2i - 3) * (-1)
        Assert.IsTrue(r4.IsInverted);
        Assert.AreEqual(-2, r4.StartValue); // i
        Assert.AreEqual(3, r4.EndValue);

        var i_negi = new Number(_invDomain.DefaultBasisNumber, new(0, -100)); // seg -i
        var r3 = n * i_negi; // (2i - 3) * (1)
        Assert.IsTrue(r3.IsInverted);
        Assert.AreEqual(2, r3.StartValue); // i
        Assert.AreEqual(-3, r3.EndValue);
    }
}