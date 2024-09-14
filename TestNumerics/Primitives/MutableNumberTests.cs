using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace TestNumerics.Primitives;

[TestClass]
public class MutableNumberTests
{
    private Trait _trait = null!;
    private Focal _basisFocal = null!;
    private Focal _limits = null!;
    private Domain _domain = null!;
    private static double _delta = 0.001;

    [TestInitialize]
    public void Init()
    {
        _trait = new Trait("mutableNumberTests");
        _basisFocal = new Focal(0, 10);
        _limits = new Focal(-10000, 10000);
        _domain = new Domain(_trait, _basisFocal, _limits);
    }
    [TestMethod]
    public void MutableOperations()
    {
        var result = new Number(_domain, new Focal(0, 20));
        var num3 = new Number(_domain, new Focal(0, 30));

        result.Add(num3);
        Assert.AreEqual(0, result.StartValue);
        Assert.AreEqual(5, result.EndValue);

        result.Subtract(num3);
        Assert.AreEqual(0, result.StartValue);
        Assert.AreEqual(2, result.EndValue);

        result.Multiply(num3);
        Assert.AreEqual(0, result.StartValue);
        Assert.AreEqual(6, result.EndValue);

        result.Divide(num3);
        Assert.AreEqual(0, result.StartValue);
        Assert.AreEqual(2, result.EndValue);

        result.Pow(num3);
        Assert.AreEqual(0, result.StartValue);
        Assert.AreEqual(8, result.EndValue);
    }
}