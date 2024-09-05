//using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Numerics.Primitives;

namespace TestNumerics.Primitives
{
    [TestClass]
    public class DomainTests
    {
        private Trait _trait = null!;
        private Focal _unitFocal = null!;
        private Focal _maxMin = null!;
        private Domain _domain = null!;

        [TestInitialize]
        public void Init()
        {
            _trait = new Trait("domain tests");
            _unitFocal = new Focal(-4, 6);
            _maxMin = new Focal(-54, 46);
            _domain = new Domain(_trait, _unitFocal, _maxMin);
        }
        [TestMethod]
        public void DomainSizeTests()
        {
            Assert.AreEqual(1, _domain.TickSize);
        }
    }
}