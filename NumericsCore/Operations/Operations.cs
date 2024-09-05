using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Structures
{
    public abstract class OperationsBase
    {
        public Number? RightSide { get; }
        public OperationsBase(Number rightSide)
        {
            RightSide = rightSide;
        }
        public OperationsBase()
        {
        }

        public virtual Number Calculate(Number input) => RightSide == null ? input : Calculate(input, RightSide);
        public abstract Number Calculate(Number input, Number rightSide);
    }

    public class SetOperation : OperationsBase
    {
        public SetOperation() { }
        public SetOperation(Number rightSide) :base(rightSide) { }
        public override Number Calculate(Number input, Number rightSide) => rightSide; // set fixed value regardless of input
    }
    public class AddOperation : OperationsBase
    {
        public AddOperation() { }
        public AddOperation(Number rightSide) : base(rightSide) { }
        public override Number Calculate(Number input, Number rightSide) => input + rightSide;
    }

    public class SubtractOperation : OperationsBase
    {
        public SubtractOperation() { }
        public SubtractOperation(Number rightSide) : base(rightSide) { }
        public override Number Calculate(Number input, Number rightSide) => input - RightSide;
    }

    public class MultiplyOperation : OperationsBase
    {
        public MultiplyOperation() { }
        public MultiplyOperation(Number rightSide) : base(rightSide) { }
        public override Number Calculate(Number input, Number rightSide) => input * RightSide;
    }

    public class DivideOperation : OperationsBase
    {
        public DivideOperation() { }
        public DivideOperation(Number rightSide) : base(rightSide) { }
        public override Number Calculate(Number input, Number rightSide) => input / RightSide;
    }

    public class PowOperation : OperationsBase
    {
        public PowOperation() { }
        public PowOperation(Number rightSide) : base(rightSide) { }
        public override Number Calculate(Number input, Number rightSide) => input.Pow(RightSide);
    }
}
