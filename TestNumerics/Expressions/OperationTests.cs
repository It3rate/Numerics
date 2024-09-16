using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Expressions;

namespace TestNumerics.Operations;

[TestClass]
public class OperationTests
{
    private Trait _trait = null!;
    private Focal _basisFocal = null!;
    private Focal _limits = null!;
    private Domain _domain = null!;
    private Number _basisNum = null!;
    private Number _num2_8 = null!;
    private Number _num3_6 = null!;
    private static double _delta = 0.001;

    [TestInitialize]
    public void Init()
    {
        _trait = new Trait("Tests");
        _basisFocal = new Focal(0, 100);
        _limits = new Focal(-10000, 10000);
        _domain = new Domain(_trait, _basisFocal, _limits);
        _basisNum = _domain.DefaultBasisNumber;
        _num2_8 = new Number(_basisNum, new(-200, 800));
        _num3_6 = new Number(_basisNum, new(-300, 600));
    }

    [TestMethod]
    public void SetTests()
    {
        var setOperation = new SetOperation(_num2_8);
        var result = setOperation.Calculate(_num3_6);
        Assert.AreEqual(2, result.StartValue);
        Assert.AreEqual(8, result.EndValue);
        result = setOperation.Calculate(result);
        Assert.AreEqual(2, result.StartValue);
        Assert.AreEqual(8, result.EndValue);
    }

    [TestMethod]
    public void AddTests()
    {
        var setOperation = new AddOperation(_num2_8);
        var result = setOperation.Calculate(_num3_6);
        Assert.AreEqual(5, result.StartValue);
        Assert.AreEqual(14, result.EndValue);
        result = setOperation.Calculate(result);
        Assert.AreEqual(7, result.StartValue);
        Assert.AreEqual(22, result.EndValue);
    }

    [TestMethod]
    public void SubtractTests()
    {
        var setOperation = new SubtractOperation(_num2_8);
        var result = setOperation.Calculate(_num3_6);
        Assert.AreEqual(1, result.StartValue);
        Assert.AreEqual(-2, result.EndValue);
        result = setOperation.Calculate(result);
        Assert.AreEqual(-1, result.StartValue);
        Assert.AreEqual(-10, result.EndValue);
    }

    [TestMethod]
    public void MultiplyTests()
    {
        var setOperation = new MultiplyOperation(_num2_8);
        var result = setOperation.Calculate(_num3_6);
        Assert.AreEqual(36, result.StartValue);
        Assert.AreEqual(42, result.EndValue);
        result = setOperation.Calculate(result);
        Assert.AreEqual(372, result.StartValue);
        Assert.AreEqual(264, result.EndValue);
    }
    [TestMethod]
    public void DivideTests()
    {
        var setOperation = new DivideOperation(_num2_8);
        var result = setOperation.Calculate(_num3_6);
        Assert.AreEqual(0.17, result.StartValue, _delta);
        Assert.AreEqual(0.79, result.EndValue, _delta);
        result = setOperation.Calculate(result);
        Assert.AreEqual(0, result.StartValue, _delta);
        Assert.AreEqual(0.09, result.EndValue, _delta);
    }
    [TestMethod]
    public void PowTests()
    {
        var setOperation = new PowOperation(_num2_8);
        var result = setOperation.Calculate(_num3_6);
        Assert.AreEqual(1530434.37, result.StartValue, _delta);
        Assert.AreEqual(538167.4, result.EndValue, _delta);
    }
}
