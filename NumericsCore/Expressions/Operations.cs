using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;

namespace NumericsCore.Expressions
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
        public override Number Calculate(Number input) => input.Zero;
    }

    public class InvertOperation : OperationBase
    {
        public InvertOperation() { }
        public override Number Calculate(Number input) => input.Invert();
    }
    public class InvertNegateOperation : OperationBase
    {
        public InvertNegateOperation() { }
        public override Number Calculate(Number input) => input.InvertNegate();
    }
    public class LengthOperation : OperationBase
    {
        public LengthOperation() { }
        public override Number Calculate(Number input) => input.Length;
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
        protected Number? _rightSide;
        public BinaryOperationsBase()
        {
        }
        public BinaryOperationsBase(Number rightSide)
        {
            _rightSide = rightSide;
        }
        public void SetRightSide(Number rightSide)
        {
            _rightSide = rightSide;
        }

        public override Number Calculate(Number input) => _rightSide == null ? input : Calculate(input, _rightSide);
        protected abstract Number Calculate(Number input, Number rightSide);
    }

    public class SetOperation : BinaryOperationsBase
    {
        public SetOperation() { }
        public SetOperation(Number rightSide) : base(rightSide) { }
        protected override Number Calculate(Number input, Number rightSide) => rightSide; // set fixed value regardless of input
    }
    public class AddOperation : BinaryOperationsBase
    {
        public AddOperation() { }
        public AddOperation(Number rightSide) : base(rightSide) { }
        protected override Number Calculate(Number input, Number rightSide) => input.Add(rightSide);
    }

    public class SubtractOperation : BinaryOperationsBase
    {
        public SubtractOperation() { }
        public SubtractOperation(Number rightSide) : base(rightSide) { }
        protected override Number Calculate(Number input, Number rightSide) => input.Subtract(rightSide);
    }

    public class MultiplyOperation : BinaryOperationsBase
    {
        public MultiplyOperation() { }
        public MultiplyOperation(Number rightSide) : base(rightSide) { }
        protected override Number Calculate(Number input, Number rightSide) => input.Multiply(rightSide);
    }

    public class DivideOperation : BinaryOperationsBase
    {
        public DivideOperation() { }
        public DivideOperation(Number rightSide) : base(rightSide) { }
        protected override Number Calculate(Number input, Number rightSide) => input.Divide(rightSide);
    }

    public class PowOperation : BinaryOperationsBase
    {
        public PowOperation() { }
        public PowOperation(Number rightSide) : base(rightSide) { }
        protected override Number Calculate(Number input, Number rightSide) => input.Pow(rightSide);
    }
    public class CompareOperation : BinaryOperationsBase
    {
        public CompareOperation() { }
        public CompareOperation(Number rightSide) : base(rightSide) { }
        protected override Number Calculate(Number input, Number rightSide) => input;//todo: comparisons
    }
    #endregion

    #region Ternary

    public class TernaryOperation : OperationBase
    {
        private Number TrueSide { get; }
        private Number FalseSide { get; }
        public TernaryOperation(OperationBase operation) { }
        public TernaryOperation(OperationBase operation, Number trueSide, Number falseSide)
        { 
            TrueSide = trueSide;
            FalseSide = falseSide;
        }

        public override Number Calculate(Number input)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
