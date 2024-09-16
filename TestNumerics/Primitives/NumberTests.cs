using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Primitives;
using NumericsCore.Utils;

namespace TestNumerics.Primitives;

[TestClass]
public class NumberTests
{
    private Trait _trait = null!;
    private Focal _basisFocal = null!;
    private Focal _limits = null!;
    private Domain _domain = null!;
    private static double _delta = 0.001;

    [TestInitialize]
    public void Init()
    {
        _trait = new Trait("numberTests");
        _basisFocal = new Focal(2, 8);
        _limits = new Focal(-1000, 1010);
        _domain = new Domain(_trait, _basisFocal, _limits);
    }
    [TestMethod]
    public void Constructor_ValidValues_SetsPropertiesCorrectly()
    {
        var focal = new Focal(2, 8);
        var number = new Number(_domain.DefaultBasisNumber, focal);
        var invDomain = _domain.Inverse;

        Assert.AreEqual(_domain, number.Domain);
        Assert.AreEqual(_basisFocal, number.Focal);
        Assert.AreEqual(Polarity.Aligned, number.Polarity);

        focal = new Focal(2, 8);
        number = new Number(invDomain.DefaultBasisNumber, focal);

        Assert.AreEqual(invDomain, number.Domain);
        Assert.AreEqual(_basisFocal, number.Focal);
        Assert.AreEqual(Polarity.Inverted, number.Polarity);
    }
    [TestMethod]
    public void TickLength_ValidValues_CalculatesCorrectly()
    {
        var focal = new Focal(2, 8);
        var number = new Number(_domain.DefaultBasisNumber, focal);
        Assert.AreEqual(6, number.TickLength);
        focal = new Focal(8, 2);
        number = new Number(_domain.DefaultBasisNumber, focal);
        Assert.AreEqual(-6, number.TickLength);
    }
    [TestMethod]
    public void AbsTickLength_ValidValues_CalculatesCorrectly()
    {
        var focal = new Focal(8, 2);
        var number = new Number(_domain.DefaultBasisNumber, focal);
        var absTickLength = number.AbsTickLength;
        Assert.AreEqual(6, absTickLength);
        focal = new Focal(8, 2);
        number = new Number(_domain.DefaultBasisNumber, focal);
        Assert.AreEqual(6, number.AbsTickLength);
    }
    [TestMethod]
    public void Add_ValidValues_CalculatesCorrectly()
    {
        var focal1 = new Focal(2, 6);
        var focal2 = new Focal(3, 7);
        var number1 = new Number(_domain.DefaultBasisNumber, focal1);
        var number2 = new Number(_domain.DefaultBasisNumber, focal2);
        var result = number1 + number2;

        Assert.AreEqual(3, result.StartTick);
        Assert.AreEqual(11, result.EndTick);
        Assert.AreEqual(-0.1666, result.StartValue, _delta);
        Assert.AreEqual(1.5000, result.EndValue, _delta);

        var prResult = PRange.FromNumber(number1) + PRange.FromNumber(number2);
        Assert.AreEqual(prResult.Start, result.StartValue, _delta);
        Assert.AreEqual(prResult.End, result.EndValue, _delta);
    }
    [TestMethod]
    public void Subtract_ValidValues_CalculatesCorrectly()
    {
        _basisFocal = new Focal(200, 800);
        _domain = new Domain(_trait, _basisFocal, _limits);

        var focal1 = new Focal(200, 600);
        var focal2 = new Focal(300, 700);
        var number1 = new Number(_domain.DefaultBasisNumber, focal1);
        var number2 = new Number(_domain.DefaultBasisNumber, focal2);
        var result = number1 - number2;
        Assert.AreEqual(100, result.StartTick);
        Assert.AreEqual(100, result.EndTick);
        Assert.AreEqual(0.1666, result.StartValue, _delta);
        Assert.AreEqual(-0.1666, result.EndValue, _delta);

        var prResult = PRange.FromNumber(number1) - PRange.FromNumber(number2); // {-.17,-.17}
        Assert.AreEqual(prResult.Start, result.StartValue, _delta);
        Assert.AreEqual(prResult.End, result.EndValue, _delta);
    }
    [TestMethod]
    public void Multiply_ValidValues_CalculatesCorrectly()
    {
        _basisFocal = new Focal(0, 1000);
        _limits = new Focal(-100000, 100000);
        _domain = new Domain(_trait, _basisFocal, _limits);

        var focal1 = new Focal(0, 4000);
        var focal2 = new Focal(0, 2000);
        var number1 = new Number(_domain.DefaultBasisNumber, focal1);
        var number2 = new Number(_domain.DefaultBasisNumber, focal2);
        var result = number1 * number2;
        Assert.AreEqual(0.00, result.StartValue, _delta);
        Assert.AreEqual(8.00, result.EndValue, _delta);

        focal1 = new Focal(2000, 4000);
        focal2 = new Focal(0, 2000);
        number1 = new Number(_domain.DefaultBasisNumber, focal1);
        number2 = new Number(_domain.DefaultBasisNumber, focal2);
        result = number1 * number2;
        Assert.AreEqual(-4.00, result.StartValue, _delta);
        Assert.AreEqual(8.00, result.EndValue, _delta);

        var prResult = PRange.FromNumber(number1) * PRange.FromNumber(number2);
        Assert.AreEqual(prResult.Start, result.StartValue, _delta);
        Assert.AreEqual(prResult.End, result.EndValue, _delta);



        _basisFocal = new Focal(200, 800);
        _domain = new Domain(_trait, _basisFocal, _limits);

        focal1 = new Focal(200, 600);
        focal2 = new Focal(300, 700);
        number1 = new Number(_domain.DefaultBasisNumber, focal1);
        number2 = new Number(_domain.DefaultBasisNumber, focal2);
        result = number1 * number2;
        Assert.AreEqual(266, result.StartTick);
        Assert.AreEqual(533, result.EndTick);
        Assert.AreEqual(-0.11, result.StartValue, _delta);
        Assert.AreEqual(0.555, result.EndValue, _delta);

        prResult = PRange.FromNumber(number1) * PRange.FromNumber(number2);// {.11, .56}
        Assert.AreEqual(prResult.Start, result.StartValue, .01);
        Assert.AreEqual(prResult.End, result.EndValue, .01);
    }
    [TestMethod]
    public void Divide_ValidValues_CalculatesCorrectly()
    {
        _basisFocal = new Focal(0, 1000);
        _limits = new Focal(-100000, 100000);
        _domain = new Domain(_trait, _basisFocal, _limits);

        var focal1 = new Focal(0, 4000);
        var focal2 = new Focal(0, 400);
        var number1 = new Number(_domain.DefaultBasisNumber, focal1);
        var number2 = new Number(_domain.DefaultBasisNumber, focal2);
        var result = number1 / number2; // 4 / 0.4 = 10
        Assert.AreEqual(0.00, result.StartValue, _delta);
        Assert.AreEqual(10.00, result.EndValue, _delta);

        focal1 = new Focal(2000, 4000);
        focal2 = new Focal(0, 400);
        number1 = new Number(_domain.DefaultBasisNumber, focal1);
        number2 = new Number(_domain.DefaultBasisNumber, focal2);
        result = number1 / number2; // (-2i + 4) / (0.4) = (-5i + 10)
        Assert.AreEqual(-5.00, result.StartValue, _delta);
        Assert.AreEqual(10.00, result.EndValue, _delta);

        _basisFocal = new Focal(1000, 2000);
        _domain = new Domain(_trait, _basisFocal, _limits);
        focal1 = new Focal(3000, 5000);
        focal2 = new Focal(1000, 1400);
        number1 = new Number(_domain.DefaultBasisNumber, focal1);
        number2 = new Number(_domain.DefaultBasisNumber, focal2);
        result = number1 / number2;
        Assert.AreEqual(-5.00, result.StartValue, _delta);
        Assert.AreEqual(10.00, result.EndValue, _delta);

        _basisFocal = new Focal(200, 800);
        _domain = new Domain(_trait, _basisFocal, _limits);

        focal1 = new Focal(600, 4200);
        focal2 = new Focal(300, 700);
        number1 = new Number(_domain.DefaultBasisNumber, focal1);
        number2 = new Number(_domain.DefaultBasisNumber, focal2);
        result = number1 / number2; // (-0.67i + 6.67) / (-0.17i + 0.83) = (0.8050i + 7.8713) // [-283, 4923] 

        var pr1 = PRange.FromNumber(number1);
        var pr2 = PRange.FromNumber(number2);
        var prResult = pr1 / pr2;

        Assert.AreEqual(0.7683, result.StartValue, _delta);
        Assert.AreEqual(7.845, result.EndValue, _delta);
        Assert.AreEqual(prResult.Start, result.StartValue, .01);
        Assert.AreEqual(prResult.End, result.EndValue, .01);

        //Assert.AreEqual(16, result.StartTick);
        //Assert.AreEqual(9, result.EndTick);
        //Assert.AreEqual(2.3333, result.StartValue, _delta);
        //Assert.AreEqual(1.1666, result.EndValue, _delta);

        //Assert.AreEqual(prResult.Start, result.StartValue, _delta);
        //Assert.AreEqual(prResult.End, result.EndValue, _delta);
    }
    [TestMethod]
    public void Divide_OffsetBasis_CalculatesCorrectly()
    {
        _delta = 0.01;
        _basisFocal = new Focal(10000, 6000);
        _limits = new Focal(-100000, 100000);
        _domain = new Domain(_trait, _basisFocal, _limits);

        var focal1 = new Focal(6000, 42000);
        var focal2 = new Focal(3000, 7000);
        var number1 = new Number(_domain.DefaultBasisNumber, focal1);
        var number2 = new Number(_domain.DefaultBasisNumber, focal2);
        var result = number1 / number2; // (-1i - 8) / (-1.75i + 0.75) = (-4.0690i - 1.1724)

        var pr1 = PRange.FromNumber(number1);
        var pr2 = PRange.FromNumber(number2);
        var prResult = pr1 / pr2;

        Assert.AreEqual(-4.0675, result.StartValue, _delta);
        Assert.AreEqual(-1.17, result.EndValue, _delta);
        Assert.AreEqual(prResult.Start, result.StartValue, .01);
        Assert.AreEqual(prResult.End, result.EndValue, .01);
    }
    [TestMethod]
    public void DecimalValue_ValidTick_CalculatesCorrectly()
    {
        var focal = new Focal(2, 8);
        var number = new Number(_domain.DefaultBasisNumber, focal);
        Assert.AreEqual(0, number.StartValue, _delta);
        Assert.AreEqual(1, number.EndValue, _delta);
    }
    [TestMethod]
    public void TickValue_ValidDecimalValue_CalculatesCorrectly()
    {
        _basisFocal = new Focal(10000, 20000);
        _limits = new Focal(-100000, 100000);
        _domain = new Domain(_trait, _basisFocal, _limits);
        double decimalValueStart = 0.5;
        double decimalValueEnd = 0.8;
        var tick = _domain.FocalFromDecimalSigned(decimalValueStart, decimalValueEnd);
        Assert.AreEqual(5000, tick.StartTick);
        Assert.AreEqual(18000, tick.EndTick);
    }
    [TestMethod]
    public void Clone_ValidNumber_CreatesNewInstance()
    {
        var focal = new Focal(2, 8);
        var number = new Number(_domain.DefaultBasisNumber, focal);
        var clone = number.Clone();
        Assert.AreNotSame(number, clone);
        Assert.AreEqual(number.Domain, clone.Domain);
        Assert.AreEqual(number.Focal, clone.Focal);
        Assert.AreEqual(number.Polarity, clone.Polarity);
    }
}
