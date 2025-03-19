using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Expressions;

namespace NumericsCore.Motions
{
    public interface IUnit // Trait
    {
        string Name { get; }
        int SubUnitCount { get; }
        IUnit[] SubUnits { get; } // X,Y,Z  R,G,B  etc These are unit 1, form the basis

		Focal Limits { get; }
	}
	public class DefaultUnit : IUnit
	{
		public string Name { get; }
		public int SubUnitCount => SubUnits.Length;
		public IUnit[] SubUnits { get; } = Array.Empty<IUnit>();
		public Focal Limits { get; }
        public DefaultUnit(string name, Focal limits)
		{
			Name = name;
			Limits = limits;
		}
	}
	public class Step
    {
        public List<SymmetricNumber> Numbers { get; } = new List<SymmetricNumber>();
    }
    public class UnitFocals
    {
        public List<Focal> Focals { get; } = new List<Focal>();
    }
    public interface IMeasure // single measure, no units, no basis. X or Y or Red etc
    {
        IUnit Unit { get; }
        Focal Focal { get; }
        int TickCount { get; }
        Focal GetSection(double startT, double endT);
    }
    public interface IPath : IMeasure // domain, single measure over potentially complex path
    {
        UnitFocals[] PathHistory { get; }
        // need to implement expressions here. They need sequential/parallel, abs/rel/target, tangent/linear, tiling, bool conditionals
        // will be multiple types here
        // Equation based
        // data based
        // compute based (branchable)
    }
    public interface INum // measure with a basis
    {
        IMeasure Numerator { get; }
        IMeasure Denominator { get; } // default to (-1, 1) ticks
    }
    public class Num(IMeasure numerator, IMeasure denominator) : INum
    {
        public IMeasure Numerator { get; } = numerator;
        public IMeasure Denominator { get; } = denominator;
    }
    public class NumRef : Num // not really needed, but for clarity this has a complex path for the denominator
    {
        public NumRef(IMeasure numerator, IPath denominator) : base(numerator, denominator)
        {
        }
    }
}
