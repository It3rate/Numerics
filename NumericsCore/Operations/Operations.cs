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
        public Number RightSide { get; }
        public OperationsBase(Number rightSide)
        {
            RightSide = rightSide;
        }

        public abstract Number Calculate(Number input);
    }

    public class AddOperation : OperationsBase
    {
        public AddOperation(Number rightSide) : base(rightSide)
        {
        }
        public override Number Calculate(Number input) => input + RightSide;
    }

    public class SubtractOperation : OperationsBase
    {
        public SubtractOperation(Number rightSide) : base(rightSide)
        {
        }
        public override Number Calculate(Number input) => input - RightSide;
    }

    public class MultiplyOperation : OperationsBase
    {
        public MultiplyOperation(Number rightSide) : base(rightSide)
        {
        }
        public override Number Calculate(Number input) => input * RightSide;
    }

    public class DivideOperation : OperationsBase
    {
        public DivideOperation(Number rightSide) : base(rightSide)
        {
        }
        public override Number Calculate(Number input) => input / RightSide;
    }
}
