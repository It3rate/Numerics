using Numerics.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericsCore.Expressions
{
	public class BinaryExpression : Expression
	{
		public Number Left => Results[0];
		public Number Right => Results[1];
		public BinaryExpression(Number left, Number right) : base(left, right)
		{

		}
	}
}
