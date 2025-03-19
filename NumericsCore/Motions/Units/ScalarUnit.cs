using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Motions.Units
{
	public class ScalarUnit : IUnit
	{
		public static ScalarUnit Scalar1K = new ScalarUnit(new Focal(-1000, 1000));
		public static ScalarUnit Scalar10K = new ScalarUnit(new Focal(-10000, 10000));
		public static ScalarUnit Scalar100K = new ScalarUnit(new Focal(-100000, 100000));
		public string Name => "Scalar";
		public int SubUnitCount => 0;
		public IUnit[] SubUnits => Array.Empty<IUnit>();
		public Focal Limits { get; private set; }

		public ScalarUnit(Focal limits)
		{
			Limits = limits;
		}
	}
}
