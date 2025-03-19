namespace MathDemo;

using NumericsSkia.Agent;
using NumericsSkia.Drawing;
using NumericsSkia.Mappers;
using NumericsSkia.Renderer;
using NumericsCore.Primitives;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using NumericsSkia.Utils;
using Ornamental.Utils;
using Numerics.Primitives;
using NumericsCore.Expressions;
using NumericsCore.Interfaces;
using Ornamental.Commands;
using Numerics.CoreConcepts.Time;
using NumericsCore.Sequencer;
using Ornamental.Commands.Tasks;
using NumericsCore.Motions;
using NumericsCore.Motions.Units;
using System.Diagnostics;
using System.IO;
using NumericsAPI.GA;

public class SymSlides : DemoBase
{
    private Trait _trait = null!;
    private Focal _basisFocal = null!;
    private Focal _limits = null!;
    private Domain _domain = null!;
    private static double _delta = 0.001;
    private OrnamentMapper _curMapper => (OrnamentMapper)_currentAgent.Mapper;

	public void Init()
    {
        _trait = new Trait("Tests");
        _basisFocal = new Focal(0, 100);
        _limits = new Focal(-10000, 10000);
        _domain = new Domain(_trait, _basisFocal, _limits);
    }
    public SymSlides()
    {

		_testIndex = 0 ;// 
        Pages.AddRange(new PageCreator[]
		{
			GATest,
			SortTest,
			MotionTest,
			CommandTest,
            AnimationTest2,
            AnimationTest,
            OrnamentTest,
        });
    }
    private List<Expression> _xList;
    private List<Expression> _yList;
    SKPoint _origin;
    int _hCount = 25;
    int _space = 80;
	private List<Comparison<SymmetricNumber>> _sortFunctions;
	private List<string> _labels;
    private enum CancelKind { None, StartToZero, StartOnly, Midpoint, EndToZero, EndOnly, StartPlus, Fin}
    private CancelKind _cancelKind;
	private List<SymmetricNumber> _sortValues = new List<SymmetricNumber>();
	private int _sortIndex = 0;

    private SKMapper GATest()
	{
		var wm = new OrnamentMapper(_currentAgent, 100, 100, 1050, 0);
		var world = new GAWorld(8, 1000);
        return wm;
    }
	private SKMapper SortTest()
	{
        _space = 6;
		var wm = new OrnamentMapper(_currentAgent, 100, 100, 1050, 0);
		wm.Label = "Simulation";
		var domain = new Domain(Trait.ScalarTrait, Focal.One, Focal.MaxFocal);

		_sortFunctions = new List<Comparison<SymmetricNumber>>
		{
			(a, b) => 0,
			(a, b) => a.StartValue.CompareTo(b.StartValue),
			(a, b) => a.EndValue.CompareTo(b.EndValue),
		    (a, b) => a.Length.CompareTo(b.Length),
		    (a, b) => a.GetQuotientSum(BitMask.AD, BitMask.BC).CompareTo(b.GetQuotientSum(BitMask.AD, BitMask.BC)),
		    (a, b) => a.GetSum(BitMask.AB).CompareTo(b.GetSum(BitMask.AB)),
		    (a, b) => a.GetSumDifference(BitMask.AD, BitMask.BC).CompareTo(b.GetSumDifference(BitMask.AD, BitMask.BC)),
		    (a, b) => a.GetProductRatio(BitMask.AB, BitMask.CD).CompareTo(b.GetProductRatio(BitMask.AB, BitMask.CD)),
		};
        _labels = new List<string>
        {
            "None",
            "Start Value",
            "End Value",
            "Length",
            "Quotient Sum A/D + B/C",
			"Sum",
			"Sum Difference AD-BC",
			"Product Ratio AB/CD",
		};
        _currentAgent.Mapper = wm;
		CreateValues();
		ArrangeValues();

		return wm;
	}
    private void CreateValues()
    {
		_sortValues.Clear();
		var xyUnit = new XYUnit();
		var count = 130;
		var bottomFocal = new Focal(-100, 100);

		for (int i = 0; i < count; i++)
		{
			var topFocal = new Focal(RndLong(), RndLong());
			bottomFocal = new Focal(-RndShort(), RndShort());
			var nm = new SymmetricNumber(xyUnit.XUnit, topFocal, bottomFocal);
			_sortValues.Add(nm);
		}
        ArrangeValues();
	}
    private void ArrangeValues()
	{
		_sortValues.Sort(_sortFunctions[_sortIndex]);
        AdjustDisplay();
	}
    private void AdjustDisplay()
    {
        _curMapper.Label = "Sort by: " + _labels[_sortIndex] + " :: " + _cancelKind;
		_origin = _curMapper.Guideline.StartPoint + new SKPoint(500, 0);
		_currentAgent.Mapper.Paths.Clear();
		foreach (var nm in _sortValues)
		{
			var p = EmptyPath();
            var start = nm.StartValue;
            var end = nm.EndValue;
            switch (_cancelKind)
			{
				case CancelKind.None:
					break;
				case CancelKind.StartToZero:
					end = end - start;
					start = 0;
					break;
				case CancelKind.EndToZero:
					start = end - start;
					end = 0;
					break;
				case CancelKind.StartOnly:
					end = 0;
					break;
				case CancelKind.EndOnly:
					start = 0;
					break;
				case CancelKind.Midpoint:
                    var len = (end - start) / 2f;
					start = -len;
					end = len;
					break;
				case CancelKind.StartPlus:
					end = start + 10;
					break;
			}
			AddLine(p, start, end);
			_curMapper.Paths.Add(p);
		}
    }
	public override void Custom(int key)
	{
		base.Custom(key);
        if (key == 0)
        {
            _sortIndex = _sortIndex >= _sortFunctions.Count - 1 ? 0 : _sortIndex + 1;
            ArrangeValues();
        }
        else if (key == 1)
        {
            CreateValues();
		}
		else if (key == 2)
		{
            int next = (int)_cancelKind + 1;
            _cancelKind = (next >= (int)CancelKind.Fin) ? (CancelKind)0 : (CancelKind)next;
            AdjustDisplay();
		}
	}

	Random _rnd = new Random();
	private long Rnd(int min, int max) => (long)_rnd.Next(max - min) + min;
	private long RndLong() => Rnd(-100000, 100000);
	private long RndShort() => Rnd(000, 300) + 150;

	private SKMapper MotionTest()
	{
		_space = 80;
		var xyUnit = new XYUnit();
		var xTrait = new Trait("X");
		var yTrait = new Trait("Y");
		var xDomain = new Domain(xTrait, _basisFocal, _limits);
		var yDomain = new Domain(yTrait, _basisFocal, _limits);
        
		var wm = new OrnamentMapper(_currentAgent, 100, 100, 1050, 0);
		_origin = wm.Guideline.StartPoint;

		var topFocal = new Focal(-1000, 105000);
		var bottomFocal = new Focal(-100, 100);
		var symNumX = new SymmetricNumber(xyUnit.XUnit, topFocal, bottomFocal);
		var symNumY = new SymmetricNumber(xyUnit.YUnit, topFocal, bottomFocal);
		var xFocal = new Focal(500, 900);
		var refx0 = new ReferenceByFixedPosition(symNumX, xFocal);
		var yFocal = new Focal(300, 1200);
		var refy0 = new ReferenceByFixedPosition(symNumY, yFocal);

		//StackOp[] ops = [
  //          new StackOp(BitMask.AC, Ops.Add),
		//	new StackOp(BitMask.BD, Ops.Add),
		//];
		var stack = new ExprStack(refx0, refy0, AddOps);
        var mot = new Motion(stack);
        var result0 = mot.Run();

		stack = new ExprStack(refx0, refy0, MultOps);
		mot = new Motion(stack);
		var result1 = mot.Run();

		return wm;
	}
	private StackOp[] AddOps => [
			new StackOp(BitMask.None, Ops.Subtract), // errer test
			new StackOp(BitMask.AC, Ops.Add), // r
			new StackOp(BitMask.BD, Ops.Add), // i
		];

	private StackOp[] MultOps => [
			new StackOp(BitMask.BD, Ops.Multiply),
			new StackOp(BitMask.AC, Ops.Multiply),
			new StackOp(BitMask.None, Ops.Subtract), // r

			new StackOp(BitMask.AD, Ops.Multiply),
			new StackOp(BitMask.BC, Ops.Multiply),
			new StackOp(BitMask.None, Ops.Add), // i
		];

	private SKMapper CommandTest()
	{
		_space = 80;
		var xTrait = new Trait("X");
        var yTrait = new Trait("Y");
        var xDomain = new Domain(xTrait, _basisFocal, _limits);
        var yDomain = new Domain(yTrait, _basisFocal, _limits);

        var wm = new OrnamentMapper(_currentAgent, 100, 100, 1050, 0);
        _origin = wm.Guideline.StartPoint;

        var delayDuration = MillisecondNumber.Create(0, 3000);
        var delayDuration2 = MillisecondNumber.Create(0, 500);

        var topFocal = new Focal(-1000, 105000);
        var topFocalY = new Focal(-1000, 12000);
        var bottomFocal = new Focal(-100, 100);

        var dcx = new NumberSequenceTask(xDomain, topFocal, bottomFocal, delayDuration);
        var dcy = new NumberSequenceTask(yDomain, topFocalY, bottomFocal, delayDuration2);
        var dcm = new NumberSequenceCommand(delayDuration, dcx, dcy);
        _currentAgent.Stack.Do(dcm);

        Init();


        return wm;
    }

    private SKMapper AnimationTest2()
	{
		_space = 80;
		var wm = new OrnamentMapper(_currentAgent, 100, 100, 1050, 0);
        _origin = wm.Guideline.StartPoint;

        Init();

        var zero = new Number(_domain.DefaultBasisNumber, new(0, 0));
        var inc = new Number(_domain.DefaultBasisNumber, new(500, 2000));
        var offset = new Number(_domain.DefaultBasisNumber, new(4000, 3000));
        AtomicExpression expr0, expr1;

        var equation_x0 = new Expression([zero, inc, offset], 1, TileMode.Continue, false, 4);
        expr1 = new AtomicExpression(1, new AddOperation(), 1);
        expr0 = new AtomicExpression(expr1, new AddOperation(), 1);
        var expr3 = new AtomicExpression(2, new AddOperation(), 1);
        equation_x0.AddAtomicExpression(expr1, expr0, expr3);

        //var equation_x1 = new Expression([zero, inc], 1, TileMode.Continue, true, 1, false);
        //equation_x1.AddAtomicExpression(expr1);

        var equation_y0 = new Expression([offset], 1, TileMode.Bounce, false, 1);
        expr0 = new AtomicExpression(0, new AddOperation(), 1);
        equation_y0.AddAtomicExpression(expr0);

        _xList = [equation_x0];
        _yList = [equation_y0];

        var result = NextPath();
        for (int i = 0; i < _hCount * 3; i++)
        {
            equation_x0.Next();
            equation_y0.Next(); 
            AddLine(result);
            //offset.Add(inc);
        }
        wm.Paths.Add(result);

        return wm;
    }


    private SKMapper AnimationTest()
	{
		_space = 80;
		var wm = new OrnamentMapper(_currentAgent, 100, 100, 1050, 0);
        _origin = wm.Guideline.StartPoint;

        Init();

        var inc = new Number(_domain.DefaultBasisNumber, new(0, 100));
        var offset = new Number(_domain.DefaultBasisNumber, new(2000, 2000));
        var offset2x = new Number(_domain.DefaultBasisNumber, new(-4000, 4000));

        var path = new Number(_domain.DefaultBasisNumber, new(0, 3000));
        var lm1 = new Landmark(path, 0.2);
        var lm2 = new Landmark(path, 0.8);
        var lmn = new Number(_domain.DefaultBasisNumber, lm1, lm2);

        AtomicExpression expr0, expr1;
        Expression equation_x0, equation_y0;

        equation_x0 = new Expression([offset, offset], 30, TileMode.Bounce, false, 1);
        expr0 = new AtomicExpression(0, new AddOperation(), 1);
        equation_x0.AddAtomicExpression(expr0);

        equation_y0 = new Expression([offset, offset], 30, TileMode.Bounce, false, 2);
        expr0 = new AtomicExpression(0, new AddOperation(), 1);
        equation_y0.AddAtomicExpression(expr0);

        var equation_xlong = new Expression([offset2x], 30, TileMode.Continue, false, 1);
        expr0 = new AtomicExpression(0, new AddOperation(), 1);
        equation_xlong.AddAtomicExpression(expr0);//, step0_1);

        var equation_ylong = new Expression([offset2x], 30, TileMode.Continue, false, 1);
        expr0 = new AtomicExpression(0, new AddOperation(), 1);
        equation_ylong.AddAtomicExpression(expr0);//, step0_1);


        _xList = [equation_x0, equation_xlong];
        _yList = [equation_y0, equation_ylong];

        var result = NextPath();
        for (int i = 0; i < _hCount; i++)
        {
            equation_y0.Next();
            equation_y0.Next();
            AddLine(result);

            equation_xlong.Next();
            AddLine(result);
        }
        wm.Paths.Add(result);

        result = NextPath();
        for (int i = 0; i < _hCount; i++)
        {
            equation_y0.Next();
            equation_y0.Next();
            equation_xlong.Next();
            AddLine(result);
        }
        wm.Paths.Add(result);

        result = NextPath();
        for (int i = 0; i < _hCount; i++)
        {
            equation_x0.Next();
            AddLine(result);
            equation_y0.Next();
            AddLine(result);

            equation_x0.Next();
            AddLine(result);
            equation_y0.Next();
            AddLine(result);

            equation_xlong.Next();
            AddLine(result);
        }
        wm.Paths.Add(result);

        result = NextPath();
        for (int i = 0; i < _hCount; i++)
        {
            equation_x0.Next();
            AddLine(result);
            equation_y0.Next();

            equation_x0.Next();
            equation_y0.Next();
            AddLine(result);

            equation_xlong.Next();
            AddLine(result);
        }

        wm.Paths.Add(result);
        result = NextPath();
        for (int i = 0; i < _hCount; i++)
        {
            equation_x0.Next();
            equation_y0.Next();
            AddLine(result);

            equation_x0.Next();
            equation_y0.Next();
            AddLine(result);

            equation_xlong.Next();
            AddLine(result);
        }
        wm.Paths.Add(result);
            
        result = NextPath();
        for (int i = 0; i < _hCount; i++)
        {
            equation_x0.Next();
            AddLine(result);
            equation_y0.Next();
            AddLine(result);

            equation_x0.Next();
            AddLine(result);
            equation_y0.Next();
            AddLine(result);

            equation_x0.Next();
            AddLine(result);
            equation_y0.Next();
            AddLine(result);

            equation_xlong.Next();
            AddLine(result);
        }
        wm.Paths.Add(result);

        result = NextPath();
        for (int i = 0; i < _hCount; i++)
        {
            equation_y0.Next();
            AddLine(result);
            equation_xlong.Next();
            AddLine(result);
        }
        wm.Paths.Add(result);


        result = NextPath();
        equation_x0.Next();
        equation_x0.TileMode = TileMode.Continue;
        for (int i = 0; i < _hCount * .63; i++)
        {
            equation_xlong.Next();
            AddLine(result);
            equation_y0.Next();
            equation_y0.Next();
            AddLine(result);
            equation_xlong.Next();
            AddLine(result);

            equation_y0.Next();
            AddLine(result);
            equation_x0.Next();
            AddLine(result);
            equation_y0.Next();
            AddLine(result);
        }
        wm.Paths.Add(result);


        result = NextPath();
        for (int i = 0; i < _hCount / 2; i++)
        {
            equation_x0.Next();
            AddLine(result);
            equation_y0.Next();
            AddLine(result);

            equation_x0.Next();
            AddLine(result);
            equation_y0.Next();
            AddLine(result);

            equation_xlong.Next();
            AddLine(result);
        }
        wm.Paths.Add(result);

        result = NextPath();

        return wm;
    }


    private SKMapper OrnamentTest()
	{
		_space = 80;
		var wm = new OrnamentMapper(_currentAgent, 100, 100, 1050, 0);
        _origin = wm.Guideline.StartPoint;
        Init();

        var zero = new Number(_domain.DefaultBasisNumber, new(0, 0));
        var inc = new Number(_domain.DefaultBasisNumber, new(500, 2000));
        var offset = new Number(_domain.DefaultBasisNumber, new(4000, 3000));
        AtomicExpression expr0, expr1;

        var equation_x0 = new Expression([zero, inc, offset], 1, TileMode.Continue, false, 4);
        expr1 = new AtomicExpression(1, new AddOperation(), 1);
        expr0 = new AtomicExpression(expr1, new AddOperation(), 1);
        var expr3 = new AtomicExpression(2, new AddOperation(), 1);
        equation_x0.AddAtomicExpression(expr1, expr0, expr3);

        //var equation_x1 = new Expression([zero, inc], 1, TileMode.Continue, true, 1, false);
        //equation_x1.AddAtomicExpression(expr1);

        var equation_y0 = new Expression([offset], 1, TileMode.Bounce, false, 1);
        expr0 = new AtomicExpression(0, new AddOperation(), 1);
        equation_y0.AddAtomicExpression(expr0);

        _xList = [equation_x0];
        _yList = [equation_y0];

        var result = NextPath();
        for (int i = 0; i < _hCount * 3; i++)
        {
            equation_x0.Next();
            equation_y0.Next();
            AddLine(result);
            //offset.Add(inc);
        }
        wm.Paths.Add(result);

        return wm;
    }


	private SKPath NextPath()
	{
		Reset();
		_origin.Y += _space;
		var result = new SKPath();
		result.MoveTo(_origin);
		return result;
	}
	private SKPath EmptyPath()
	{
		_origin.Y += _space;
		var result = new SKPath();
		return result;
	}

	private void Reset()
	{
		if (_xList != null)
        {
            foreach (var expr in _xList)
            {
                expr.Reset();
            }
            foreach (var expr in _yList)
            {
                expr.Reset();
            }
        }
        
    }

	private void AddLine(SKPath result)
	{
		var x = (float)(_xList.Select(x => x.CurrentResult.EndValue).Sum() + _origin.X);
		var y = (float)(_yList.Select(y => -y.CurrentResult.StartValue).Sum() + _origin.Y);
		result.LineTo(x, y);
	}
	private void AddLine(SKPath result, double start, double end)
	{
		result.MoveTo((float)start + _origin.X, _origin.Y);
		result.LineTo((float)end + _origin.X, _origin.Y);
	}


}
