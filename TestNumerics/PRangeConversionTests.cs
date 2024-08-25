using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Primitives;
using NumericsCore.Utils;

namespace TestNumerics;

[TestClass]
public class PRangeConversionTests
{
    private Trait _trait = null!;
    private Focal _basisFocal = null!;
    private Focal _limits = null!;
    private Domain _domain = null!;
    private Domain _invDomain = null!;
    private static double _delta = 0.001;

    [TestInitialize]
    public void Init()
    {
        _trait = new Trait("numberTests");
        _basisFocal = new Focal(0, 1000);
        _limits = new Focal(-10000, 10000);
        _domain = new Domain(_trait, _basisFocal, _limits);
        _invDomain = _domain.InvertedDomain();
    }
    [TestMethod]
    public void MultiplyAlignedTests()
    {
        var n1 = new Number(_domain, new(2000, 3000)); // (-2i+3)
        var n2 = new Number(_domain, new(0, -1000)); // (-1)
        var pr1 = PRange.FromNumber(n1);
        var pr2 = PRange.FromNumber(n2);
        var result = n1 * n2;
        var prResult = pr1 * pr2;

        Assert.AreEqual(prResult.Start, result.StartValue, .01);
        Assert.AreEqual(prResult.End, result.EndValue, .01);
    }
    [TestMethod]
    public void MultiplyAligned_iTests()
    {
        var n1 = new Number(_domain, new(2000, 3000)); // (-2i+3)
        var n2 = new Number(_domain, new(-1000, 0)); // (i)
        var pr1 = PRange.FromNumber(n1);
        var pr2 = PRange.FromNumber(n2);
        var result = n1 * n2;
        var prResult = pr1 * pr2;

        Assert.AreEqual(prResult.Start, result.StartValue, .01);
        Assert.AreEqual(prResult.End, result.EndValue, .01);
    }
    [TestMethod]
    public void MultiplyAligned_negiTests()
    {
        var n1 = new Number(_domain, new(2000, 3000)); // (-2i+3)
        var n2 = new Number(_domain, new(1000, 0)); // (-i)
        var pr1 = PRange.FromNumber(n1);
        var pr2 = PRange.FromNumber(n2);
        var result = n1 * n2;
        var prResult = pr1 * pr2;

        Assert.AreEqual(prResult.Start, result.StartValue, .01);
        Assert.AreEqual(prResult.End, result.EndValue, .01);
    }
    [TestMethod]
    public void MultiplyTests()
    {
        var n1 = new Number(_domain, new(2000, 3000)); // (-2i+3)
        var n2 = new Number(_invDomain, new(0, -1000)); // ~(1)
        var pr1 = PRange.FromNumber(n1);
        var pr2 = PRange.FromNumber(n2);
        var result = n1 * n2;
        var prResult = pr1 * pr2; // (-2i+3) * ~(1) = ~(2i-3)

        Assert.AreEqual(prResult.Start, result.StartValue, .01);
        Assert.AreEqual(prResult.End, result.EndValue, .01);
    }
    [TestMethod]
    public void UnitByAlignedTests()
    {
        _basisFocal = new Focal(0, 100);
        _limits = new Focal(-10000, 10000);
        _domain = new Domain(_trait, _basisFocal, _limits);
        _invDomain = _domain.InvertedDomain();
        var n = new Number(_domain, new(200, 300)); // (-2i+3)
        var prn = PRange.FromNumber(n);
        // preserve length: * 1, * -1, * ~(1), * ~(-1), 
        // anything where the 'real' part is one and imaginary part is zero preserves length, independent of polarity.
        var a_pos1 = new Number(_domain, new(0, 100)); // ~seg from 0i->1
        var rap1 = n * a_pos1; // (1)
        var pResult = prn * PRange.FromNumber(a_pos1);
        Assert.IsTrue(pResult.Polarity == rap1.Polarity);
        Assert.AreEqual(pResult.Start, rap1.StartValue); // i
        Assert.AreEqual(pResult.End, rap1.EndValue);

        var a_neg1 = new Number(_domain, new(0, -100)); // ~seg from 0i->-1
        var ran1 = n * a_neg1; // (-1)
        pResult = prn * PRange.FromNumber(a_neg1);
        Assert.IsTrue(pResult.Polarity == ran1.Polarity);
        Assert.AreEqual(pResult.Start, ran1.StartValue); // i
        Assert.AreEqual(pResult.End, ran1.EndValue);

        var i_neg1 = new Number(_invDomain, new(-100, 0)); // ~seg from 1->0i inverts segments in place
        var r0 = n * i_neg1; // ~(-1)
        pResult = prn * PRange.FromNumber(i_neg1);
        Assert.IsTrue(pResult.Polarity == r0.Polarity);
        Assert.AreEqual(pResult.Start, r0.StartValue);
        Assert.AreEqual(pResult.End, r0.EndValue); // i

        var i_pos1 = new Number(_invDomain, new(100, 0)); // ~seg from 1->0i inverts segments in place
        var r1 = n * i_pos1; // ~(1)
        pResult = prn * PRange.FromNumber(i_pos1);
        Assert.IsTrue(pResult.Polarity == r1.Polarity);
        Assert.AreEqual(pResult.Start, r1.StartValue);
        Assert.AreEqual(pResult.End, r1.EndValue); // i



        // change length: * (i), * ~(-i), * ~(i), * (-i)
        var a_posi = new Number(_domain, new(-100, 0));// seg from i to 0
        var r5 = n * a_posi; // +(i)
        pResult = prn * PRange.FromNumber(a_posi);
        Assert.IsTrue(pResult.Polarity == r5.Polarity);
        Assert.AreEqual(pResult.Start, r5.StartValue); // i
        Assert.AreEqual(pResult.End, r5.EndValue);

        var a_negi = new Number(_domain, new(100, 0));// seg from -i to 0
        var r2 = n * a_negi; // +(-i)
        pResult = prn * PRange.FromNumber(a_negi);
        Assert.IsTrue(pResult.Polarity == r2.Polarity);
        Assert.AreEqual(pResult.Start, r2.StartValue); // i
        Assert.AreEqual(pResult.End, r2.EndValue);

        var i_posi = new Number(_invDomain, new(0, -100)); // seg i
        var r4 = n * i_posi; // ~(i)
        pResult = prn * PRange.FromNumber(i_posi);
        Assert.IsTrue(pResult.Polarity == r4.Polarity);
        Assert.AreEqual(pResult.Start, r4.StartValue);
        Assert.AreEqual(pResult.End, r4.EndValue); // i

        var i_negi = new Number(_invDomain, new(0, 100)); // seg -i
        var r3 = n * i_negi; // ~(-i)
        pResult = prn * PRange.FromNumber(i_negi);
        Assert.IsTrue(pResult.Polarity == r3.Polarity);
        Assert.AreEqual(pResult.Start, r3.StartValue);
        Assert.AreEqual(pResult.End, r3.EndValue); // i
    }
    [TestMethod]
    public void UnitByInvertedTests()
    {
        _basisFocal = new Focal(0, 100);
        _limits = new Focal(-10000, 10000);
        _domain = new Domain(_trait, _basisFocal, _limits);
        _invDomain = _domain.InvertedDomain();
        var n = new Number(_invDomain, new(200, 300)); // ~(2-3i)
        var prn = PRange.FromNumber(n);

        Assert.IsTrue(prn.IsInverted);
        Assert.AreEqual(2, prn.Start);
        Assert.AreEqual(-3, prn.End); // i

        // preserve length: * 1, * -1, * ~(1), * ~(-1), 
        // anything where the 'real' part is one and imaginary part is zero preserves length, independent of polarity.
        var a_pos1 = new Number(_domain, new(0, 100)); // ~seg from 0i->1
        var rap1 = n * a_pos1; // (1)
        var pResult = prn * PRange.FromNumber(a_pos1);
        Assert.IsTrue(pResult.Polarity == rap1.Polarity);
        Assert.AreEqual(pResult.Start, rap1.StartValue); // i
        Assert.AreEqual(pResult.End, rap1.EndValue);

        var a_neg1 = new Number(_domain, new(0, -100)); // ~seg from 0i->-1
        var ran1 = n * a_neg1; // (-1)
        pResult = prn * PRange.FromNumber(a_neg1);
        Assert.IsTrue(pResult.Polarity == rap1.Polarity);
        Assert.AreEqual(pResult.Start, ran1.StartValue); // i
        Assert.AreEqual(pResult.End, ran1.EndValue);

        var i_neg1 = new Number(_invDomain, new(-100, 0)); // ~seg from 1->0i inverts segments in place
        var r0 = n * i_neg1; // ~(-1)
        pResult = prn * PRange.FromNumber(i_neg1);
        Assert.IsTrue(pResult.Polarity == r0.Polarity);
        Assert.AreEqual(pResult.Start, r0.StartValue);
        Assert.AreEqual(pResult.End, r0.EndValue); // i

        var i_pos1 = new Number(_invDomain, new(100, 0)); // ~seg from 1->0i inverts segments in place
        var r1 = n * i_pos1; // ~(1)
        pResult = prn * PRange.FromNumber(i_pos1);
        Assert.IsTrue(pResult.Polarity == r1.Polarity);
        Assert.AreEqual(pResult.Start, r1.StartValue);
        Assert.AreEqual(pResult.End, r1.EndValue); // i



        // change length: * (i), * ~(-i), * ~(i), * (-i)
        var a_posi = new Number(_domain, new(-100, 0));// seg from i to 0
        var r5 = n * a_posi; // +(i)
        pResult = prn * PRange.FromNumber(a_posi);
        Assert.IsTrue(pResult.Polarity == r5.Polarity);
        Assert.AreEqual(pResult.Start, r5.StartValue);
        Assert.AreEqual(pResult.End, r5.EndValue); // i

        var a_negi = new Number(_domain, new(100, 0));// seg from -i to 0
        var r2 = n * a_negi; // +(-i)
        pResult = prn * PRange.FromNumber(a_negi);
        Assert.IsTrue(pResult.Polarity == r2.Polarity);
        Assert.AreEqual(pResult.Start, r2.StartValue);
        Assert.AreEqual(pResult.End, r2.EndValue); // i

        var i_posi = new Number(_invDomain, new(0, -100)); // seg i
        var r4 = n * i_posi; // ~(i)
        pResult = prn * PRange.FromNumber(i_posi);
        Assert.IsTrue(pResult.Polarity == r4.Polarity);
        Assert.AreEqual(pResult.Start, r4.StartValue); // i
        Assert.AreEqual(pResult.End, r4.EndValue);

        var i_negi = new Number(_invDomain, new(0, 100)); // seg -i
        var r3 = n * i_negi; // ~(-i)
        pResult = prn * PRange.FromNumber(i_negi);
        Assert.IsTrue(pResult.Polarity == r3.Polarity);
        Assert.AreEqual(pResult.Start, r3.StartValue); // i
        Assert.AreEqual(pResult.End, r3.EndValue);
    }
    [TestMethod]
    public void DivisionTests()
    {
        _basisFocal = new Focal(0, 1000);
        _limits = new Focal(-10000, 10000);
        _domain = new Domain(_trait, _basisFocal, _limits);
        var n = new Number(_domain, new(4000, 5000)); // (-4i+5)
        var prn = PRange.FromNumber(n);

        var denom = new Number(_domain, new(-2000, 3000)); // (2i+3)
        var r4 = n / denom;
        var pResult = prn / PRange.FromNumber(denom);
        Assert.IsTrue(pResult.Polarity == r4.Polarity);
        Assert.AreEqual(pResult.Start, r4.StartValue, _delta); // i
        Assert.AreEqual(pResult.End, r4.EndValue, _delta);
    }
    [TestMethod]
    public void Division_invTests()
    {
        _basisFocal = new Focal(0, 10000);
        _limits = new Focal(-100000, 100000);
        _domain = new Domain(_trait, _basisFocal, _limits);
        var n = new Number(_domain, new(40000, 50000)); // (-4i+5) => ~(4-5i)
        var prn = PRange.FromNumber(n);

        var denom = new Number(_invDomain, new(-20000, 30000)); // ~(-2-3i) => (2i+3)
        // calc in Aligned : (-4i+5) /  (2i+3)  =  (-1.69i)
        // calc in Inverted: ~(4-5i) / ~(-2-3i) = ~(0.53 + 1.69i) // X
        // calc in Inverted: ~(4i-5) / ~(-2i-3) = ~(-1.69i + 0.53) // i always represents 'other' perspective
        // what happens on one side of zero maps to the mirror version. ~ is akin to this mathematically
        var r4 = n / denom;
        var pResult = prn / PRange.FromNumber(denom);
        Assert.IsTrue(pResult.Polarity == r4.Polarity);
        //Assert.AreEqual(pResult.Start, r4.StartValue, _delta); // i
        //Assert.AreEqual(pResult.End, r4.EndValue, _delta);
    }
}