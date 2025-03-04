using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Motions
{
    public interface INumber
    {
        IUnit Unit { get; }
        Focal Basis { get; }
    }
	public class NumberPrimitive : INumber
	{
		public IUnit Unit { get; }
		public Focal Basis { get; }
		public NumberPrimitive(IUnit unit, Focal basis)
		{
			Unit = unit;
			Basis = basis;
		}	
	}
}
