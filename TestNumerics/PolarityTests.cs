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
    public void Rotate_i_Test()
    {
        var number1 = new Number(_zeroDomain, new(0, 1000));
        var number_i = new Number(_zeroDomain, new(0, -1000), Polarity.Inverted);
        var value1 = number1 * number_i;
        Assert.AreEqual(Polarity.Inverted, value1.Polarity);
        Assert.AreEqual(0, value1.Focal.FirstTick);
        Assert.AreEqual(-1000, value1.Focal.LastTick);

        var value2 = value1 * number_i;
        Assert.AreEqual(Polarity.Aligned, value2.Polarity);
        Assert.AreEqual(0, value2.Focal.FirstTick);
        Assert.AreEqual(-1000, value2.Focal.LastTick);

        var value3 = value2 * number_i;
        Assert.AreEqual(Polarity.Inverted, value3.Polarity);
        Assert.AreEqual(0, value3.Focal.FirstTick);
        Assert.AreEqual(1000, value3.Focal.LastTick);

        var value4 = value3 * number_i;
        Assert.AreEqual(Polarity.Aligned, value4.Polarity);
        Assert.AreEqual(0, value4.Focal.FirstTick);
        Assert.AreEqual(1000, value4.Focal.LastTick);
    }

    [TestMethod]
    public void MultiplyTest()
    {
        var number200 = new Number(_zeroDomain, new(0, 2000));
        var number300 = new Number(_zeroDomain, new(0, 3000));

        var value = number200 * number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(6000, value.Focal.LastTick);
        Assert.AreEqual(6, value.EndValue, _delta);

        number200.Polarity = Polarity.Inverted;
        value = number200 * number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(6000, value.Focal.LastTick);
        Assert.AreEqual(-6, value.StartValue, _delta);
        Assert.AreEqual(0, value.EndValue, _delta);

        number300.Polarity = Polarity.Inverted;
        value = number200 * number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(-6000, value.Focal.LastTick);
        Assert.AreEqual(-6, value.EndValue, _delta);

        number200.Polarity = Polarity.Aligned;
        value = number200 * number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(6000, value.Focal.LastTick);
        Assert.AreEqual(-6, value.StartValue, _delta);
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
        //Assert.AreEqual(-0.9469, value.EndValue, _delta);

        number200.Polarity = Polarity.Aligned;
        value = number200 * number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0.9469, value.StartValue, _delta);
        Assert.AreEqual(2.4394, value.EndValue, _delta);
    }
    [TestMethod]
    public void DivideTest()
    {
        var number200 = new Number(_zeroDomain, new(0, 2000));
        var number300 = new Number(_zeroDomain, new(0, 8000));

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
        Assert.AreEqual(-250, value.Focal.LastTick);

        number200.Polarity = Polarity.Aligned;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(250, value.Focal.LastTick);
        Assert.AreEqual( -0.25, value.StartValue);
    }
    [TestMethod]
    public void DivideInvertedTest()
    {
        var number200 = new Number(_invertedDomain, new(0, 200));
        var number300 = new Number(_invertedDomain, new(0, 300));
        _delta = 0.01;
        var value = number200 / number300;

        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(0.0883, value.StartValue, _delta);
        Assert.AreEqual(1.055, value.EndValue, _delta);

        number200.Polarity = Polarity.Inverted;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(-1.055, value.StartValue, _delta);
        Assert.AreEqual(-0.0883, value.EndValue, _delta);

        number300.Polarity = Polarity.Inverted;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(0.0883, value.StartValue, _delta);
        //Assert.AreEqual(1.0572, value.EndValue, _delta);

        number200.Polarity = Polarity.Aligned;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(-1.055, value.StartValue, _delta);
        Assert.AreEqual(-0.0883, value.EndValue, _delta);
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