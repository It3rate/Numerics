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
public class PolarityTests
{
    private Trait _trait = null!;
    private Focal _limits = null!;
    private Focal _zeroBasis = null!;
    private Domain _zeroDomain = null!;
    private Focal  _offsetBasis = null!;
    private Domain _offsetDomain = null!;
    private Focal  _invertedBasis = null!;
    private Domain _invertedDomain = null!;
    private static double _delta = 0.001;

    [TestInitialize]
    public void Init()
    {
        _trait = new Trait("polarityTests");
        _limits = new Focal(-10000, 10000);

        _zeroBasis = new Focal(0, 1000);
        _zeroDomain = new Domain(_trait, _zeroBasis, _limits);

        _offsetBasis = new Focal(200, 800);
        _offsetDomain = new Domain(_trait, _offsetBasis, _limits);

        _invertedBasis = new Focal(800, 200);
        _invertedDomain = new Domain(_trait, _invertedBasis, _limits);
    }
    [TestMethod]
    public void InvertTest()
    {
        var number = new Number(_zeroDomain, new(0, 200));
        Assert.AreEqual(Polarity.Aligned, number.Polarity);
        Assert.IsTrue(number.IsAligned);
        Assert.IsFalse(number.IsInverted);
        Assert.IsTrue(number.HasPolarity);

        number = number.InvertPolarity();
        Assert.AreEqual(Polarity.Inverted, number.Polarity);
        Assert.IsFalse(number.IsAligned);
        Assert.IsTrue(number.IsInverted);
        Assert.IsTrue(number.HasPolarity);

        number.Polarity = Polarity.Unknown;
        Assert.AreEqual(Polarity.Unknown, number.Polarity);
        number = number.InvertPolarity();
        Assert.AreEqual(Polarity.Unknown, number.Polarity);
        Assert.IsFalse(number.HasPolarity);

        number.Polarity = Polarity.None;
        Assert.AreEqual(Polarity.None, number.Polarity);
        number = number.InvertPolarity();
        Assert.AreEqual(Polarity.None, number.Polarity);
        Assert.IsFalse(number.HasPolarity);
    }
    [TestMethod]
    public void MultiplyTest()
    {
        var number200 = new Number(_zeroDomain, new(0, 200));
        var number300 = new Number(_zeroDomain, new(0, 300));

        var value = number200 * number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(60, value.Focal.LastTick);
        Assert.AreEqual(0.06, value.EndValue, _delta);

        number200.Polarity = Polarity.Inverted;
        value = number200 * number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(60, value.Focal.LastTick);
        Assert.AreEqual(-0.06, value.StartValue, _delta);
        Assert.AreEqual(0, value.EndValue, _delta);

        number300.Polarity = Polarity.Inverted;
        value = number200 * number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(60, value.Focal.LastTick);
        Assert.AreEqual(0.06, value.EndValue, _delta);

        number200.Polarity = Polarity.Aligned;
        value = number200 * number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(60, value.Focal.LastTick);
        Assert.AreEqual(-0.06, value.StartValue, _delta);
        Assert.AreEqual(0, value.EndValue, _delta);
    }
    [TestMethod]
    public void MultiplyInvertedTest()
    {
        var number200 = new Number(_invertedDomain, new(0, 200));
        var number300 = new Number(_invertedDomain, new(0, 300));
        _delta = 0.01;
        var value = number200 * number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(-2.4394, value.StartValue, _delta);
        Assert.AreEqual(-0.9469, value.EndValue, _delta);

        number200.Polarity = Polarity.Inverted;
        value = number200 * number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0.9469, value.StartValue, _delta);
        Assert.AreEqual(2.4394, value.EndValue, _delta);


        number300.Polarity = Polarity.Inverted;
        value = number200 * number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(-2.4394, value.StartValue, _delta);
        Assert.AreEqual(-0.9469, value.EndValue, _delta);

        number200.Polarity = Polarity.Aligned;
        value = number200 * number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0.9469, value.StartValue, _delta);
        Assert.AreEqual(2.4394, value.EndValue, _delta);
    }
    [TestMethod]
    public void DivideTest()
    {
        var number200 = new Number(_zeroDomain, new(0, 200));
        var number300 = new Number(_zeroDomain, new(0, 800));

        var value = number200 / number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(250, value.Focal.LastTick);
        Assert.AreEqual(0.25, value.EndValue, _delta);

        number200.Polarity = Polarity.Inverted;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(250, value.Focal.LastTick);

        number300.Polarity = Polarity.Inverted;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(250, value.Focal.LastTick);

        number200.Polarity = Polarity.Aligned;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(250, value.Focal.LastTick);
    }
    //[TestMethod]
    public void DivideInvertedTest()
    {
        var number200 = new Number(_invertedDomain, new(0, 200));
        var number300 = new Number(_invertedDomain, new(0, 300));
        _delta = 0.01;
        var value = number200 / number300;
        var pr1 = PRange.FromNumber(number200);
        var pr2 = PRange.FromNumber(number300);
        var prResult = pr1 / pr2;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(prResult.Start, value.StartValue, _delta);
        Assert.AreEqual(prResult.End, value.EndValue, _delta);

        number200.Polarity = Polarity.Inverted;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);

        pr1 = PRange.FromNumber(number200);
        pr2 = PRange.FromNumber(number300);
        prResult = pr1 / pr2;
        Assert.AreEqual(prResult.Start, value.StartValue, _delta);
        Assert.AreEqual(prResult.End, value.EndValue, _delta);

        number300.Polarity = Polarity.Inverted;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        pr1 = PRange.FromNumber(number200);
        pr2 = PRange.FromNumber(number300);
        prResult = pr1 / pr2;
        Assert.AreEqual(prResult.Start, value.StartValue, _delta);
        Assert.AreEqual(prResult.End, value.EndValue, _delta);

        number200.Polarity = Polarity.Aligned;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        pr1 = PRange.FromNumber(number200);
        pr2 = PRange.FromNumber(number300);
        prResult = pr1 / pr2;
        Assert.AreEqual(prResult.Start, value.StartValue, _delta);
        Assert.AreEqual(prResult.End, value.EndValue, _delta);
    }
    [TestMethod]
    public void AddTest()
    {
        var number200 = new Number(_zeroDomain, new(100, 200));
        var number300 = new Number(_zeroDomain, new(100, 300));

        number200.Polarity = Polarity.Inverted;
        var value = number200 + number300;
        Assert.AreEqual(-0.5, value.StartValue, _delta);
        Assert.AreEqual(0.2, value.EndValue, _delta);

        number200 = new Number(_zeroDomain, new(0, 200));
        number300 = new Number(_zeroDomain, new(0, 300));

        value = number200 + number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(500, value.Focal.LastTick);
        Assert.AreEqual(0, value.StartValue, _delta);
        Assert.AreEqual(0.5, value.EndValue, _delta);

        number200.Polarity = Polarity.Inverted;
        value = number200 + number300;

        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(500, value.Focal.LastTick);
        Assert.AreEqual(-0.5, value.StartValue, _delta);
        Assert.AreEqual(0, value.EndValue, _delta);

        number300.Polarity = Polarity.Inverted;
        value = number200 + number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(500, value.Focal.LastTick);
        Assert.AreEqual(-0.5, value.StartValue, _delta);
        Assert.AreEqual(0, value.EndValue, _delta);

        number200.Polarity = Polarity.Aligned;
        value = number200 + number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(500, value.Focal.LastTick);
        Assert.AreEqual(0, value.StartValue, _delta);
        Assert.AreEqual(0.5, value.EndValue, _delta);
    }
}