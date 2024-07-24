using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Primitives;

namespace TestNumerics;

[TestClass]
public class PolarityTests
{
    private Trait _trait = null!;
    private Focal _limits = null!;
    private Focal _zeroBasis = null!;
    private Domain _zeroDomain = null!;
    private Focal _offsetBasis = null!;
    private Domain _offsetDomain = null!;
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

        var product = number200 * number300;
        Assert.AreEqual(Polarity.Aligned, product.Polarity);
        Assert.AreEqual(0, product.Focal.FirstTick);
        Assert.AreEqual(60, product.Focal.LastTick);
        Assert.AreEqual(0.06, product.EndValue, _delta);

        number200.Polarity = Polarity.Inverted;
        product = number200 * number300;
        Assert.AreEqual(Polarity.Inverted, product.Polarity);
        Assert.AreEqual(0, product.Focal.FirstTick);
        Assert.AreEqual(60, product.Focal.LastTick);
        Assert.AreEqual(-0.06, product.StartValue, _delta);
        Assert.AreEqual(0, product.EndValue, _delta);

        number300.Polarity = Polarity.Inverted;
        product = number200 * number300;
        Assert.AreEqual(Polarity.Aligned, product.Polarity);
        Assert.AreEqual(0, product.Focal.FirstTick);
        Assert.AreEqual(-60, product.Focal.LastTick);
        Assert.AreEqual(-0.06, product.EndValue, _delta);

        number200.Polarity = Polarity.Aligned;
        product = number200 * number300;
        Assert.AreEqual(Polarity.Inverted, product.Polarity);
        Assert.AreEqual(0, product.Focal.FirstTick);
        Assert.AreEqual(60, product.Focal.LastTick);
        Assert.AreEqual(-0.06, product.StartValue, _delta);
        Assert.AreEqual(0, product.EndValue, _delta);
    }
    [TestMethod]
    public void DivideTest()
    {
        var number200 = new Number(_zeroDomain, new(0, 200));
        var number300 = new Number(_zeroDomain, new(0, 800));

        var product = number200 / number300;
        Assert.AreEqual(Polarity.Aligned, product.Polarity);
        Assert.AreEqual(0, product.Focal.FirstTick);
        Assert.AreEqual(250, product.Focal.LastTick);
        Assert.AreEqual(0.25, product.EndValue, _delta);

        number200.Polarity = Polarity.Inverted;
        product = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, product.Polarity);
        Assert.AreEqual(0, product.Focal.FirstTick);
        Assert.AreEqual(250, product.Focal.LastTick);

        number300.Polarity = Polarity.Inverted;
        product = number200 / number300;
        Assert.AreEqual(Polarity.Aligned, product.Polarity);
        Assert.AreEqual(0, product.Focal.FirstTick);
        Assert.AreEqual(-250, product.Focal.LastTick);

        number200.Polarity = Polarity.Aligned;
        product = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, product.Polarity);
        Assert.AreEqual(0, product.Focal.FirstTick);
        Assert.AreEqual(250, product.Focal.LastTick);
    }
    [TestMethod]
    public void AddTest()
    {
        var number200 = new Number(_zeroDomain, new(0, 200));
        var number300 = new Number(_zeroDomain, new(0, 300));

        var product = number200 + number300;
        Assert.AreEqual(Polarity.Aligned, product.Polarity);
        Assert.AreEqual(0, product.Focal.FirstTick);
        Assert.AreEqual(500, product.Focal.LastTick);
        Assert.AreEqual(0.5, product.EndValue, _delta);

        number200.Polarity = Polarity.Inverted;
        product = number200 + number300;
        Assert.AreEqual(Polarity.Inverted, product.Polarity);
        Assert.AreEqual(300, product.Focal.FirstTick);
        Assert.AreEqual(200, product.Focal.LastTick);
        Assert.AreEqual(-0.2, product.StartValue, _delta);
        Assert.AreEqual(0.3, product.EndValue, _delta);

        number300.Polarity = Polarity.Inverted;
        product = number200 + number300;
        Assert.AreEqual(Polarity.Inverted, product.Polarity);
        Assert.AreEqual(0, product.Focal.FirstTick);
        Assert.AreEqual(500, product.Focal.LastTick);
        Assert.AreEqual(-0.5, product.StartValue, _delta);
        Assert.AreEqual(0, product.EndValue, _delta);

        number200.Polarity = Polarity.Aligned;
        product = number200 + number300;
        Assert.AreEqual(Polarity.Aligned, product.Polarity);
        Assert.AreEqual(300, product.Focal.FirstTick);
        Assert.AreEqual(200, product.Focal.LastTick);
        Assert.AreEqual(-0.3, product.StartValue, _delta);
        Assert.AreEqual(0.2, product.EndValue, _delta);
    }
}