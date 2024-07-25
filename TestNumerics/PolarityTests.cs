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
        _limits = new Focal(-1000000, 1000000);

        _zeroBasis = new Focal(0, 1000);
        _zeroDomain = new Domain(_trait, _zeroBasis, _limits);

        _offsetBasis = new Focal(200, 1200);
        _offsetDomain = new Domain(_trait, _offsetBasis, _limits);

        _invertedBasis = new Focal(1200, 200);
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
    public void MultiplyInverted2Test()
    {
        _delta = 0.01;
        _invertedBasis = new Focal(8000, 4000);
        _invertedDomain = new Domain(_trait, _invertedBasis, _limits);
        var number200 = new Number(_invertedDomain, new(8000, 0)); // 2
        var number300 = new Number(_invertedDomain, new(8000, -4000)); // 3

        var value = number200 * number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        //Assert.AreEqual(0, value.Focal.FirstTick);
        //Assert.AreEqual(6000, value.Focal.LastTick);
        Assert.AreEqual(0, value.StartValue, _delta);
        Assert.AreEqual(6, value.EndValue, _delta);

        number200.Polarity = Polarity.Inverted; // -2i
        value = number200 * number300; // (-2i * 3) = -6i
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        //Assert.AreEqual(0, value.Focal.FirstTick);
        //Assert.AreEqual(6000, value.Focal.LastTick);
        Assert.AreEqual(-6, value.StartValue, _delta);
        Assert.AreEqual(0, value.EndValue, _delta);

        number300.Polarity = Polarity.Inverted; // -3i
        value = number200 * number300; // (-2i * -3i) = -6 [8000:32000]
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        //Assert.AreEqual(0, value.Focal.FirstTick);
        //Assert.AreEqual(-6000, value.Focal.LastTick);
        Assert.AreEqual(0, value.StartValue, _delta);
        Assert.AreEqual(-6, value.EndValue, _delta);

        number200.Polarity = Polarity.Aligned; // 2
        value = number200 * number300; // (2 * -3i) = -6i
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        //Assert.AreEqual(0, value.Focal.FirstTick);
        //Assert.AreEqual(6000, value.Focal.LastTick);
        Assert.AreEqual(-6, value.StartValue, _delta);
        Assert.AreEqual(0, value.EndValue, _delta);
    }
    [TestMethod]
    public void MultiplyInverted3Test()
    {
        _invertedBasis = new Focal(8000, 4000);
        _invertedDomain = new Domain(_trait, _invertedBasis, _limits);
        var number200 = new Number(_invertedDomain, new(0, 2000)); // -2i + 1.5 
        var number300 = new Number(_invertedDomain, new(0, -4000)); // -2i + 3

        var value = number200 * number300; // (-2i + 1.5) * (-2i + 3)
        Assert.AreEqual(Polarity.Aligned, value.Polarity); // (-9i + 0.5)
        Assert.AreEqual(-9, value.StartValue, _delta);
        Assert.AreEqual(0.5, value.EndValue, _delta);

        var n = new Number(_invertedDomain, new(-26000, -4000));
        var db = new Focal(0, -4000);
        var id = new Domain(_trait, db, _limits);
        var n2 = new Number(id, new(-36000, -2000));//(-9i+0.5)
        var n3 = new Number(id, new(-34000, -12000));//(-8.5i + 3)
        number200.Polarity = Polarity.Inverted;
        value = number200 * number300; // (2 + -1.5i) * (-2i + 3) = (-8.5i + 3) [-26000, -4000] 
        Assert.AreEqual(Polarity.Inverted, value.Polarity); //
        Assert.AreEqual(3, value.StartValue, _delta);
        Assert.AreEqual(-8.5, value.EndValue, _delta);


        number300.Polarity = Polarity.Inverted;
        value = number200 * number300; //  (2 + -1.5i) * (2 + -3i) = (-9i - 5)
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(-9, value.StartValue, _delta);
        Assert.AreEqual(-5, value.EndValue, _delta);

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
        Assert.AreEqual(0, value.StartValue, _delta);
        Assert.AreEqual(0.25, value.EndValue, _delta);

        number200.Polarity = Polarity.Inverted;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(250, value.Focal.LastTick);
        Assert.AreEqual(-0.25, value.StartValue, _delta);
        Assert.AreEqual(0, value.EndValue, _delta);

        number300.Polarity = Polarity.Inverted;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(-250, value.Focal.LastTick);
        Assert.AreEqual(0, value.StartValue, _delta);
        Assert.AreEqual(-0.25, value.EndValue, _delta);

        number200.Polarity = Polarity.Aligned;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(0, value.Focal.FirstTick);
        Assert.AreEqual(250, value.Focal.LastTick);
        Assert.AreEqual(-0.25, value.StartValue, _delta);
        Assert.AreEqual(0, value.EndValue, _delta);
    }
    [TestMethod]
    public void DivideOffsetTest()
    {
        var number200 = new Number(_offsetDomain, new(200, 2200));
        var number300 = new Number(_offsetDomain, new(200, 8200));

        var value = number200 / number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(200, value.Focal.FirstTick);
        Assert.AreEqual(450, value.Focal.LastTick);
        Assert.AreEqual(0, value.StartValue, _delta);
        Assert.AreEqual(0.25, value.EndValue, _delta);

        number200.Polarity = Polarity.Inverted;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(200, value.Focal.FirstTick);
        Assert.AreEqual(450, value.Focal.LastTick);
        Assert.AreEqual(-0.25, value.StartValue, _delta);
        Assert.AreEqual(0, value.EndValue, _delta);

        number300.Polarity = Polarity.Inverted;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(200, value.Focal.FirstTick);
        Assert.AreEqual(-50, value.Focal.LastTick);
        Assert.AreEqual(0, value.StartValue, _delta);
        Assert.AreEqual(-0.25, value.EndValue, _delta);

        number200.Polarity = Polarity.Aligned;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(200, value.Focal.FirstTick);
        Assert.AreEqual(450, value.Focal.LastTick);
        Assert.AreEqual(-0.25, value.StartValue, _delta);
        Assert.AreEqual(0, value.EndValue, _delta);
    }
    [TestMethod]
    public void DivideInvertedTest()
    {
        var number200 = new Number(_invertedDomain, new(1200, -800));
        var number300 = new Number(_invertedDomain, new(1200, -6800));

        var value = number200 / number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(1200, value.Focal.FirstTick);
        Assert.AreEqual(950, value.Focal.LastTick);
        Assert.AreEqual(0, value.StartValue, _delta);
        Assert.AreEqual(0.25, value.EndValue, _delta);

        number200.Polarity = Polarity.Inverted;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(1200, value.Focal.FirstTick);
        Assert.AreEqual(950, value.Focal.LastTick);
        Assert.AreEqual(-0.25, value.StartValue, _delta);
        Assert.AreEqual(0, value.EndValue, _delta);

        number300.Polarity = Polarity.Inverted;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Aligned, value.Polarity);
        Assert.AreEqual(1200, value.Focal.FirstTick);
        Assert.AreEqual(1450, value.Focal.LastTick);
        Assert.AreEqual(0, value.StartValue, _delta);
        Assert.AreEqual(-0.25, value.EndValue, _delta);

        number200.Polarity = Polarity.Aligned;
        value = number200 / number300;
        Assert.AreEqual(Polarity.Inverted, value.Polarity);
        Assert.AreEqual(1200, value.Focal.FirstTick);
        Assert.AreEqual(950, value.Focal.LastTick);
        Assert.AreEqual(-0.25, value.StartValue, _delta);
        Assert.AreEqual(0, value.EndValue, _delta);
    }
    [TestMethod]
    public void DivideInverted2Test()
    {
        _invertedBasis = new Focal(800, 200);
        _invertedDomain = new Domain(_trait, _invertedBasis, _limits);
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