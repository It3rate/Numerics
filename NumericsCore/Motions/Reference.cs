using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Motions
{
	public abstract class Reference
	{
		public SymmetricNumber Source { get; private set; } // should be a numberline, maybe same thing? Yes.
		public virtual double StartValue { get; }
		public virtual double EndValue { get; }
		public Reference(SymmetricNumber source)
		{
			Source = source;
		}
	}

	public class ReferenceByRatio : Reference
	{
		public double StartT { get; private set; }
		public double EndT { get; private set; }
		public override double StartValue => StartT;
		public override double EndValue => EndT;
		public ReferenceByRatio(SymmetricNumber source, double startT, double endT) : base(source)
		{
			StartT = startT;
			EndT = endT;
		}
	}

	public class ReferenceByOffset : Reference
	{
		public Focal Offset { get; private set; }
		public ReferenceByOffset(SymmetricNumber source, Focal offset) : base(source)
		{
			Offset = offset;
		}
	}

	public class ReferenceByEvent : Reference
	{
		public string EventName { get; private set; }
		public ReferenceByEvent(SymmetricNumber source, string eventName) : base(source)
		{
			EventName = eventName;
		}
	}

	public class ReferenceByFixedPosition : Reference
	{
		public string Name { get; private set; }
		public Focal Position { get; private set; }
		public override double StartValue => Source.StartValueAtPosition(Position.StartTick);
		public override double EndValue => Source.EndValueAtPosition(Position.EndTick);
		public ReferenceByFixedPosition(SymmetricNumber source, Focal position, string name = "") : base(source)
		{
			Position = position;
			Name = name;
		}
	}
	public class ReferenceByTimestamp : Reference
	{
		public long TimestampMS { get; private set; }
		public ReferenceByTimestamp(SymmetricNumber source, long timestampMS) : base(source)
		{
			TimestampMS = timestampMS;
		}
	}
}
