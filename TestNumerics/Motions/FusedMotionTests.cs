using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace TestNumerics.Motions;

[TestClass]
public class FusedMotionTests
{
    private Trait _trait = null!;
    private Focal _basisFocal = null!;
    private Focal _limits = null!;
    private Domain _domain = null!;
    private static double _delta = 0.001;

    [TestInitialize]
    public void Init()
    {
        _trait = new Trait("Tests");
        _basisFocal = new Focal(0, 100);
        _limits = new Focal(-10000, 10000);
        _domain = new Domain(_trait, _basisFocal, _limits);
    }

    [TestMethod]
    public void BasicTests()
    {
    }
}