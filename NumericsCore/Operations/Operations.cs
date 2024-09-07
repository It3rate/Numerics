using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Structures
{
    #region Unary
    public abstract class OperationBase
    {
        public OperationBase()
        {
        }
        public abstract Number Calculate(Number input);
    }

    public class ResetOperation : OperationBase
    {
        public ResetOperation() { }
        public override Number Calculate(Number input) => input.Domain.Zero;
    }

    public class FlipPolarityOperation : OperationBase
    {
        public FlipPolarityOperation() { }
        public override Number Calculate(Number input) => input.InvertPolarity();
    }
    public class IncrementOperation : OperationBase
    {
        public IncrementOperation() { }
        public override Number Calculate(Number input) => ++input; // input.Increment();
    }
    public class DecrementOperation : OperationBase
    {
        public DecrementOperation() { }
        public override Number Calculate(Number input) => --input; //input.Decrement();
    }
    public class IncrementEndTickOperation : OperationBase
    {
        public IncrementEndTickOperation() { }
        public override Number Calculate(Number input) => input.IncrementEndTick();
    }
    public class DecrementEndTickOperation : OperationBase
    {
        public DecrementEndTickOperation() { }
        public override Number Calculate(Number input) => input.DecrementEndTick();
    }
    #endregion

    #region Binary
    public abstract class BinaryOperationsBase : OperationBase
    {
        public Number? RightSide { get; }
        public BinaryOperationsBase()
        {
        }
        public BinaryOperationsBase(Number rightSide)
        {
            RightSide = rightSide;
        }

        public override Number Calculate(Number input) => RightSide == null ? input : Calculate(input, RightSide);
        public abstract Number Calculate(Number input, Number rightSide);
    }

    public class SetOperation : BinaryOperationsBase
    {
        public SetOperation() { }
        public SetOperation(Number rightSide) : base(rightSide) { }
        public override Number Calculate(Number input, Number rightSide) => rightSide; // set fixed value regardless of input
    }
    public class AddOperation : BinaryOperationsBase
    {
        public AddOperation() { }
        public AddOperation(Number rightSide) : base(rightSide) { }
        public override Number Calculate(Number input, Number rightSide) => input + rightSide;
    }

    public class SubtractOperation : BinaryOperationsBase
    {
        public SubtractOperation() { }
        public SubtractOperation(Number rightSide) : base(rightSide) { }
        public override Number Calculate(Number input, Number rightSide) => input - rightSide;
    }

    public class MultiplyOperation : BinaryOperationsBase
    {
        public MultiplyOperation() { }
        public MultiplyOperation(Number rightSide) : base(rightSide) { }
        public override Number Calculate(Number input, Number rightSide) => input * rightSide;
    }

    public class DivideOperation : BinaryOperationsBase
    {
        public DivideOperation() { }
        public DivideOperation(Number rightSide) : base(rightSide) { }
        public override Number Calculate(Number input, Number rightSide) => input / rightSide;
    }

    public class PowOperation : BinaryOperationsBase
    {
        public PowOperation() { }
        public PowOperation(Number rightSide) : base(rightSide) { }
        public override Number Calculate(Number input, Number rightSide) => input.Pow(rightSide);
    }
    #endregion
}
