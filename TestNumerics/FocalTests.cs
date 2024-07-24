using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Utils;

namespace TestNumerics;
[TestClass]
public class FocalTests
{
    [TestMethod]
    public void Constructor_ValidValues_SetsPositionsCorrectly()
    {
        long start = 10;
        long end = 20;
        var focal = new Focal(start, end);
        Assert.AreEqual(start, focal.FirstTick);
        Assert.AreEqual(end, focal.LastTick);
    }

    [TestMethod]
    public void TickLength_ValidValues_CalculatesCorrectly()
    {
        var focal1 = new Focal(10, 20);
        var focal2 = new Focal(20, 10);
        long tickLength1 = focal1.TickLength;
        long tickLength2 = focal2.TickLength;
        Assert.AreEqual(10, tickLength1);
        Assert.AreEqual(-10, tickLength2);
    }

    [TestMethod]
    public void NonZeroTickLength_ZeroTickLength_ReturnsOne()
    {
        var focal = new Focal(10, 10);
        long nonZeroTickLength = focal.NonZeroTickLength;
        Assert.AreEqual(1, nonZeroTickLength);
    }

    [TestMethod]
    public void Direction_ValidValues_CalculatesCorrectly()
    {
        var focal1 = new Focal(10, 20);
        var focal2 = new Focal(20, 10);
        var focal3 = new Focal(10, 10);
        int direction1 = focal1.Direction;
        int direction2 = focal2.Direction;
        int direction3 = focal3.Direction;
        Assert.AreEqual(1, direction1);
        Assert.AreEqual(-1, direction2);
        Assert.AreEqual(0, direction3);
    }

    [TestMethod]
    public void AbsDirection_ValidValues_CalculatesCorrectly()
    {
        var focal1 = new Focal(10, 20);
        var focal2 = new Focal(20, 10);
        var focal3 = new Focal(10, 10);
        int absDirection1 = focal1.NonZeroDirection;
        int absDirection2 = focal2.NonZeroDirection;
        int absDirection3 = focal3.NonZeroDirection;
        Assert.AreEqual(1, absDirection1);
        Assert.AreEqual(-1, absDirection2);
        Assert.AreEqual(1, absDirection3); // Default to positive direction when unknown
    }

    [TestMethod]
    public void IsPositiveDirection_ValidValues_CalculatesCorrectly()
    {
        var focal1 = new Focal(10, 20);
        var focal2 = new Focal(20, 10);
        bool isPositiveDirection1 = focal1.IsPositiveDirection;
        bool isPositiveDirection2 = focal2.IsPositiveDirection;
        Assert.IsTrue(isPositiveDirection1);
        Assert.IsFalse(isPositiveDirection2);
    }

    [TestMethod]
    public void InvertedEndPosition_ValidValues_CalculatesCorrectly()
    {
        var focal = new Focal(11, 21);
        long invertedEndPosition = focal.InvertedEndPosition;
        Assert.AreEqual(1, invertedEndPosition);
    }

    [TestMethod]
    public void Add_ValidValues_CalculatesCorrectly()
    {
        var focal1 = new Focal(10, 20);
        var focal2 = new Focal(5, 15);
        var result = focal1 + focal2;
        Assert.AreEqual(15, result.FirstTick);
        Assert.AreEqual(35, result.LastTick);

        var prResult = PRange.FromFocal(focal1) + PRange.FromFocal(focal2);
        Assert.AreEqual(prResult.Start, result.FirstTick);
        Assert.AreEqual(prResult.End, result.LastTick);
    }

    [TestMethod]
    public void Subtract_ValidValues_CalculatesCorrectly()
    {
        var focal1 = new Focal(10, 20);
        var focal2 = new Focal(5, 15);
        var result = focal1 - focal2;
        Assert.AreEqual(5, result.FirstTick);
        Assert.AreEqual(5, result.LastTick);

        var prResult = PRange.FromFocal(focal1) - PRange.FromFocal(focal2);
        Assert.AreEqual(prResult.Start, result.FirstTick);
        Assert.AreEqual(prResult.End, result.LastTick);
    }

    [TestMethod]
    public void Multiply_ValidValues_CalculatesCorrectly()
    {
        var focal1 = new Focal(2, 4);
        var focal2 = new Focal(3, 6);
        var result = focal1 * focal2;
        Assert.AreEqual(24, result.FirstTick);
        Assert.AreEqual(18, result.LastTick);

        var prResult = PRange.FromFocal(focal1) * PRange.FromFocal(focal2);
        Assert.AreEqual(prResult.Start, result.FirstTick);
        Assert.AreEqual(prResult.End, result.LastTick);
    }

    [TestMethod]
    public void Divide_ValidValues_CalculatesCorrectly()
    {
        var focal1 = new Focal(10, 20);
        var focal2 = new Focal(2, 4);
        var result = focal1 / focal2;
        Assert.AreEqual(0, result.FirstTick);
        Assert.AreEqual(5, result.LastTick);

        var prResult = PRange.FromFocal(focal1) / PRange.FromFocal(focal2);
        Assert.AreEqual(prResult.Start, result.FirstTick);
        Assert.AreEqual(prResult.End, result.LastTick);
    }

    [TestMethod]
    public void Expand_ValidValues_CalculatesCorrectly()
    {
        var focal = new Focal(10, 20);
        long multiple = 3;
        var result = focal.Expand(multiple);
        Assert.AreEqual(30, result.FirstTick);
        Assert.AreEqual(60, result.LastTick);

        var prResult = PRange.FromFocal(focal) * multiple;
        Assert.AreEqual(prResult.Start, result.FirstTick);
        Assert.AreEqual(prResult.End, result.LastTick);
    }

    [TestMethod]
    public void Contract_ValidValues_CalculatesCorrectly()
    {
        var focal = new Focal(30, 60);
        long divisor = 3;
        var result = focal.Contract(divisor);
        Assert.AreEqual(10, result.FirstTick);
        Assert.AreEqual(20, result.LastTick);

        var prResult = PRange.FromFocal(focal) / divisor;
        Assert.AreEqual(prResult.Start, result.FirstTick);
        Assert.AreEqual(prResult.End, result.LastTick);
    }

    [TestMethod]
    public void GetOffset_ValidValues_CalculatesCorrectly()
    {
        var focal = new Focal(10, 20);
        long offset = 5;
        var result = focal.GetOffset(offset);
        Assert.AreEqual(15, result.FirstTick);
        Assert.AreEqual(25, result.LastTick);
    }

    [TestMethod]
    public void Clone_ValidValues_CreatesNewInstance()
    {
        var focal = new Focal(10, 20);
        var clone = focal.Clone();
        Assert.AreNotSame(focal, clone);
        Assert.AreEqual(focal.FirstTick, clone.FirstTick);
        Assert.AreEqual(focal.LastTick, clone.LastTick);
    }

    [TestMethod]
    public void Zero_ReturnsCorrectInstance()
    {
        var zero = Focal.Zero;
        Assert.AreEqual(0, zero.FirstTick);
        Assert.AreEqual(0, zero.LastTick);
    }

    [TestMethod]
    public void One_ReturnsCorrectInstance()
    {
        var one = Focal.One;
        Assert.AreEqual(0, one.FirstTick);
        Assert.AreEqual(1, one.LastTick);
    }
}