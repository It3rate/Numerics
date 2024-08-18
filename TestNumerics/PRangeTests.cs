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
public class PRangeTests
{
    private static double _delta = 0.0001;

    [TestInitialize]
    public void Init()
    {
    }
    [TestMethod]
    public void MultiplyFromZeroTests()
    {
        var left = new PRange(0, 5, Polarity.Aligned);
        var right = new PRange(0, 6, Polarity.Aligned);
        var result = left * right;
        Assert.AreEqual(0, result.Start, _delta);
        Assert.AreEqual(30, result.End, _delta);

        left = new PRange(0, -5, Polarity.Aligned);
        right = new PRange(0, 6, Polarity.Aligned);
        result = left * right;
        Assert.AreEqual(0, result.Start, _delta);
        Assert.AreEqual(-30, result.End, _delta);

        left = new PRange(0, -5, Polarity.Aligned); // (0i - 5) => ~(-0i + 5) 
        right = new PRange(0, 6, Polarity.Inverted); // ~(0i + 6)
        result = left * right;
        Assert.AreEqual(Polarity.Inverted, result.Polarity);
        Assert.AreEqual(0, result.Start, _delta);
        Assert.AreEqual(30, result.End, _delta);

        left = new PRange(0, -5, Polarity.Inverted);// ~(0i - 5)
        right = new PRange(0, 6, Polarity.Inverted); // ~(0i + 6)
        result = left * right;
        Assert.AreEqual(Polarity.Aligned, result.Polarity);
        Assert.AreEqual(0, result.Start, _delta);
        Assert.AreEqual(-30, result.End, _delta);

        left = new PRange(0, -5, Polarity.Inverted); // ~(0i - 5)
        right = new PRange(0, 6, Polarity.Aligned); // (0i + 6) => ~(-0i - 6) 
        result = left * right; // = ~(0i - 6)
        Assert.AreEqual(Polarity.Inverted, result.Polarity);
        Assert.AreEqual(0, result.Start, _delta);
        Assert.AreEqual(30, result.End, _delta);
    }
    [TestMethod]
    public void MultiplyTests()
    {
        var left = new PRange(2, 5, Polarity.Aligned);
        var right = new PRange(3, 6, Polarity.Aligned);
        var result = left * right; // (2i + 5) * (3i + 6) => (-3i - 36)
        Assert.AreEqual(27, result.Start, _delta);
        Assert.AreEqual(24, result.End, _delta);

        left = new PRange(2, -5, Polarity.Aligned);
        right = new PRange(3, 6, Polarity.Aligned);
        result = left * right; // (2i - 5) * (3i + 6) => (-3i - 36)
        Assert.AreEqual(-3, result.Start, _delta);
        Assert.AreEqual(-36, result.End, _delta);


        left = new PRange(2, -5, Polarity.Aligned); // (2i - 5) => ~(-2i + 5) // inverted
        right = new PRange(3, 6, Polarity.Inverted); // ~(3i + 6)
        result = left * right; // (-2i + 5) * (3i + 6) => ~(3i + 36)
        Assert.AreEqual(Polarity.Inverted, result.Polarity);
        Assert.AreEqual(3, result.Start, _delta);
        Assert.AreEqual(36, result.End, _delta);

        left = new PRange(2, -5, Polarity.Inverted);// ~(2i - 5) => (-2i + 5)
        right = new PRange(3, 6, Polarity.Inverted); // ~(3i + 6) => (-3i - 6)
        result = left * right; // (-2i + 5) * (-3i - 6) = ()
        Assert.AreEqual(Polarity.Aligned, result.Polarity);
        Assert.AreEqual(-3, result.Start, _delta);
        Assert.AreEqual(-36, result.End, _delta);

        left = new PRange(2, -5, Polarity.Inverted); // ~(2i - 5)
        right = new PRange(3, 6, Polarity.Aligned); // (3i + 6) => ~(-3i - 6) 
        result = left * right; // (2i - 5) * (-3i - 6) = ~(3i + 36)
        Assert.AreEqual(Polarity.Inverted, result.Polarity);
        Assert.AreEqual(3, result.Start, _delta);
        Assert.AreEqual(36, result.End, _delta);
    }
    [TestMethod]
    public void DivideTests()
    {
        var left = new PRange(60, 180, Polarity.Aligned);
        var right = new PRange(3, 6, Polarity.Aligned);
        var result = left / right; // (60i + 180) / (3i + 6) => (-4i + 28)
        Assert.AreEqual(-4, result.Start, _delta);
        Assert.AreEqual(28, result.End, _delta);

        left = new PRange(60, -180, Polarity.Aligned);
        right = new PRange(3, 6, Polarity.Aligned);
        result = left / right; // (60i - 180) / (3i + 6) => (20i - 20)
        Assert.AreEqual(20, result.Start, _delta);
        Assert.AreEqual(-20, result.End, _delta);


        left = new PRange(60, -180, Polarity.Aligned); // (60i - 180) => ~(-60i + 180) // inverted
        right = new PRange(3, 6, Polarity.Inverted); // ~(3i + 6)
        result = left / right; // (-60i + 180) / (3i + 6) => ~(-20i + 20)
        Assert.AreEqual(Polarity.Inverted, result.Polarity);
        Assert.AreEqual(-20, result.Start, _delta);
        Assert.AreEqual(20, result.End, _delta);

        left = new PRange(60, -180, Polarity.Inverted);// ~(60i - 180) => (-60i + 180)
        right = new PRange(3, 6, Polarity.Inverted); // ~(3i + 6) => (-3i - 6)
        result = left / right; // (-60i + 180) / (-3i - 6) = (20i - 20)
        Assert.AreEqual(Polarity.Aligned, result.Polarity);
        Assert.AreEqual(20, result.Start, _delta);
        Assert.AreEqual(-20, result.End, _delta);

        left = new PRange(60, -180, Polarity.Inverted); // ~(60i - 180)
        right = new PRange(3, 6, Polarity.Aligned); // (3i + 6) => ~(-3i - 6) 
        result = left / right; // (60i - 180) / (-3i - 6) = ~(3i + 36)
        Assert.AreEqual(Polarity.Inverted, result.Polarity);
        Assert.AreEqual(-20, result.Start, _delta);
        Assert.AreEqual(20, result.End, _delta);
    }

    [TestMethod]
    public void AddTests()
    {
        var left = new PRange(60, 180, Polarity.Aligned);
        var right = new PRange(3, 6, Polarity.Aligned);
        var result = left + right; // (60i + 180) + (3i + 6) => (63i + 186)
        Assert.AreEqual(63, result.Start, _delta);
        Assert.AreEqual(186, result.End, _delta);

        left = new PRange(60, -180, Polarity.Aligned);
        right = new PRange(3, 6, Polarity.Aligned);
        result = left + right; // (60i - 180) + (3i + 6) => (63i - 174)
        Assert.AreEqual(63, result.Start, _delta);
        Assert.AreEqual(-174, result.End, _delta);

        left = new PRange(60, -180, Polarity.Aligned); // (60i - 180) => ~(-60i + 180) // inverted
        right = new PRange(3, 6, Polarity.Inverted); // ~(3i + 6) => (-3i - 6)
        result = left + right; // (60i - 180) + (-3i - 6) => ~(57i - 186)
        Assert.AreEqual(Polarity.Aligned, result.Polarity);
        Assert.AreEqual(57, result.Start, _delta);
        Assert.AreEqual(-186, result.End, _delta);

        left = new PRange(60, -180, Polarity.Inverted);// ~(60i - 180)
        right = new PRange(3, 6, Polarity.Inverted); // ~(3i + 6)
        result = left + right; // (60i - 180) + (3i + 6) = (63i - 174)
        Assert.AreEqual(Polarity.Inverted, result.Polarity);
        Assert.AreEqual(63, result.Start, _delta);
        Assert.AreEqual(-174, result.End, _delta);

        left = new PRange(60, -180, Polarity.Inverted); // ~(60i - 180)
        right = new PRange(3, 6, Polarity.Aligned); // (3i + 6) => ~(-3i - 6) 
        result = left + right; // (60i - 180) + (-3i - 6) = ~(57i - 186)
        Assert.AreEqual(Polarity.Inverted, result.Polarity);
        Assert.AreEqual(57, result.Start, _delta);
        Assert.AreEqual(-186, result.End, _delta);
    }
    [TestMethod]
    public void SubtractTests()
    {
        var left = new PRange(60, 180, Polarity.Aligned);
        var right = new PRange(3, 6, Polarity.Aligned);
        var result = left - right; // (60i + 180) - (3i + 6) => (63i + 174)
        Assert.AreEqual(57, result.Start, _delta);
        Assert.AreEqual(174, result.End, _delta);

        left = new PRange(60, -180, Polarity.Aligned);
        right = new PRange(3, 6, Polarity.Aligned);
        result = left - right; // (60i - 180) - (3i + 6) => (57i - 186)
        Assert.AreEqual(57, result.Start, _delta);
        Assert.AreEqual(-186, result.End, _delta);

        left = new PRange(60, -180, Polarity.Aligned); // (60i - 180) => ~(-60i + 180) // inverted
        right = new PRange(3, 6, Polarity.Inverted); // ~(3i + 6) => (-3i - 6)
        result = left - right; // (60i - 180) - (-3i - 6) => ~(63i - 174)
        Assert.AreEqual(Polarity.Aligned, result.Polarity);
        Assert.AreEqual(63, result.Start, _delta);
        Assert.AreEqual(-174, result.End, _delta);

        left = new PRange(60, -180, Polarity.Inverted);// ~(60i - 180)
        right = new PRange(3, 6, Polarity.Inverted); // ~(3i + 6)
        result = left - right; // (60i - 180) - (3i + 6) = (57i - 186)
        Assert.AreEqual(Polarity.Inverted, result.Polarity);
        Assert.AreEqual(57, result.Start, _delta);
        Assert.AreEqual(-186, result.End, _delta);

        left = new PRange(60, -180, Polarity.Inverted); // ~(60i - 180)
        right = new PRange(3, 6, Polarity.Aligned); // (3i + 6) => ~(-3i - 6) 
        result = left - right; // (60i - 180) - (-3i - 6) = ~(63i - 174)
        Assert.AreEqual(Polarity.Inverted, result.Polarity);
        Assert.AreEqual(63, result.Start, _delta);
        Assert.AreEqual(-174, result.End, _delta);
    }

}