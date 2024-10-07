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
		public abstract Number CalculateInverse(Number input);
        public Number CalculateInPlace(Number input) { input.SetWith(Calculate(input)); return input; }
		public virtual Number CalculateInverseInPlace(Number input) { input.SetWith(CalculateInverse(input)); return input; }
	}

    public class ResetOperation : OperationBase
    {
        public ResetOperation() { }
		public override Number Calculate(Number input) => input.Zero;
		public override Number CalculateInverse(Number input) => input;
	}

    public class InvertOperation : OperationBase
    {
        public InvertOperation() { }
        public override Number Calculate(Number input) => input.Invert();
		public override Number CalculateInverse(Number input) => input;
	}
    public class InvertNegateOperation : OperationBase
    {
        public InvertNegateOperation() { }
        public override Number Calculate(Number input) => input.InvertNegate();
		public override Number CalculateInverse(Number input) => input;
	}
    public class LengthOperation : OperationBase
    {
        public LengthOperation() { }
        public override Number Calculate(Number input) => input.Length;
		public override Number CalculateInverse(Number input) => input.Length.Inverse; // need to work out these all
	}
    public class IncrementOperation : OperationBase
    {
        public IncrementOperation() { }
        public override Number Calculate(Number input) => ++input; // input.Increment();
        public override Number CalculateInverse(Number input) => --input;
	}
    public class DecrementOperation : OperationBase
    {
        public DecrementOperation() { }
        public override Number Calculate(Number input) => --input; //input.Decrement();
		public override Number CalculateInverse(Number input) => ++input;
	}
    public class IncrementEndTickOperation : OperationBase
    {
        public IncrementEndTickOperation() { }
        public override Number Calculate(Number input) => input.IncrementEndTick();
		public override Number CalculateInverse(Number input) => input.DecrementEndTick();
	}
    public class DecrementEndTickOperation : OperationBase
    {
        public DecrementEndTickOperation() { }
        public override Number Calculate(Number input) => input.DecrementEndTick();
		public override Number CalculateInverse(Number input) => input.IncrementEndTick();
	}
    #endregion

    #region Binary
    public abstract class BinaryOperationsBase : OperationBase
    {
        protected Number _rightSide;
        public BinaryOperationsBase()
        {
            _rightSide = Number.SCALAR_ZERO;
        }
        public BinaryOperationsBase(Number rightSide)
        {
            _rightSide = rightSide;
        }
        public void SetRightSide(Number rightSide)
        {
            _rightSide = rightSide;
        }
	}

    public class SetOperation : BinaryOperationsBase
    {
        public SetOperation() { }
        public SetOperation(Number rightSide) : base(rightSide) { }
		public override Number Calculate(Number input) => _rightSide.Clone(); // set fixed value regardless of input
		public override Number CalculateInverse(Number input) => input;
	}
    public class AddOperation : BinaryOperationsBase
    {
        public AddOperation() { }
        public AddOperation(Number rightSide) : base(rightSide) { }
		public override Number Calculate(Number input) => Number.Add(input, _rightSide);
		public override Number CalculateInverse(Number input) => Number.Add(_rightSide, input);
	}

    public class SubtractOperation : BinaryOperationsBase
    {
        public SubtractOperation() { }
        public SubtractOperation(Number rightSide) : base(rightSide) { }
		public override Number Calculate(Number input) => Number.Subtract(input, _rightSide);
		public override Number CalculateInverse(Number input) => Number.Subtract(_rightSide, input);
	}

    public class MultiplyOperation : BinaryOperationsBase
    {
        public MultiplyOperation() { }
        public MultiplyOperation(Number rightSide) : base(rightSide) { }
		public override Number Calculate(Number input) => Number.Multiply(input, _rightSide);
		public override Number CalculateInverse(Number input) => Number.Multiply(_rightSide, input);
	}

    public class DivideOperation : BinaryOperationsBase
    {
        public DivideOperation() { }
        public DivideOperation(Number rightSide) : base(rightSide) { }
		public override Number Calculate(Number input) => Number.Divide(input, _rightSide);
		public override Number CalculateInverse(Number input) => Number.Divide(_rightSide, input);
	}

	public class SquareOperation : BinaryOperationsBase
	{
		public SquareOperation() { }
		public SquareOperation(Number rightSide) : base(rightSide) { }
		public override Number Calculate(Number input) =>
            (_rightSide.Half * _rightSide) * (input.Squared());
		public override Number CalculateInverse(Number input) =>
			((_rightSide.Two * input) / _rightSide).Sqrt();
	}
	public class PowOperation : BinaryOperationsBase
	{
		public PowOperation() { }
		public PowOperation(Number rightSide) : base(rightSide) { }
		public override Number Calculate(Number input) => input.Pow(_rightSide);
		public override Number CalculateInverse(Number input) => throw new NotImplementedException();
	}
	public class CompareOperation : BinaryOperationsBase
    {
        public CompareOperation() { }
        public CompareOperation(Number rightSide) : base(rightSide) { }
		public override Number Calculate(Number input) => input;//todo: comparisons
		public override Number CalculateInverse(Number input) => throw new NotImplementedException();
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
		public override Number CalculateInverse(Number input) => throw new NotImplementedException();
	}
    #endregion
}
