using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Motions
{
	public class Ref
	{
		public Focal Source { get; private set; }
	}
	public class RefByRatio : Ref
	{
		public double StartT { get; private set; }
		public double EndT { get; private set; }
	}
	public class RefByOffset : Ref
	{
		public Focal Offset { get; private set; }
	}
	public class RefByEvent : Ref { }
	public class RefByFixedPosition : Ref
	{
		public Focal Position { get; private set; }
	}
}
