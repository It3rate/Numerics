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

public class Slides : DemoBase
{
    private Random rnd = new Random();
    private SKPaint _oldTextPen;
    private SKPaint _newTextPen;

    private Trait _trait = null!;
    private Focal _basisFocal = null!;
    private Focal _limits = null!;
    private Domain _domain = null!;
    private Number _num2_8 = null!;
    private Number _num3_6 = null!;
    private Number _num1_m7 = null!;
    private Number _num_m3_m6 = null!;
    private Number[] _numList = null!;
    private static double _delta = 0.001;

    public void Init()
    {
        _trait = new Trait("Tests");
        _basisFocal = new Focal(0, 100);
        _limits = new Focal(-10000, 10000);
        _domain = new Domain(_trait, _basisFocal, _limits);
        _num2_8 = new Number(_domain.DefaultBasisNumber, new(-200, 800)); //   (2i + 8)
        _num3_6 = new Number(_domain.DefaultBasisNumber, new(-300, 600)); //   (3i + 6)
        _num1_m7 = new Number(_domain.DefaultBasisNumber, new(-100, -700)); // (i -7)
        _num_m3_m6 = new Number(_domain.DefaultBasisNumber, new(300, -600));// (-3i -6)
        _numList = new Number[] { _num2_8, _num3_6, _num1_m7, _num_m3_m6 };
    }

    public Slides()
    {
        _testIndex = 0;// 
        Pages.AddRange(new PageCreator[]
        {
            AnimationTest,
        });
    }
    private List<Expression> _xList;
    private List<Expression> _yList;
    SKPoint _origin;
    int _hCount = 25;
    int _space = 70;
    private SKMapper AnimationTest()
    {
        var wm = new OrnamentMapper(_currentMouseAgent, 100, 200, 1050, 0);
        _origin = wm.Guideline.StartPoint;

        Init();

        var offset = new Number(_domain.DefaultBasisNumber, new(0, 2000));
        var offset2x = new Number(_domain.DefaultBasisNumber, new(0, 4000));

        var path = new Number(_domain.DefaultBasisNumber, new(0, 3000));
        var lm1 = new Landmark(path, 0.2);
        var lm2 = new Landmark(path, 0.8);
        var lmn = new Number(_domain.DefaultBasisNumber, lm1, lm2);

        AtomicExpression expr0, expr1;
        var equation_xlong = new Expression([offset2x], 30, TileMode.Continue, true);
        expr0 = new AtomicExpression(0, new AddOperation(), 1);
        equation_xlong.AddAtomicExpression(expr0);//, step0_1);

        var equation_x0 = new Expression([offset], 30, TileMode.Bounce, true);
        expr0 = new AtomicExpression(0, new AddOperation(), 1);
        equation_x0.AddAtomicExpression(expr0);


        var equation_y0 = new Expression([offset], 30, TileMode.Bounce, true, 2);
        expr0 = new AtomicExpression(0, new AddOperation(), 1);
        equation_y0.AddAtomicExpression(expr0);

        var equation_y1 = new Expression([offset], 30, TileMode.Continue, true);
        expr0 = new AtomicExpression(0, new SubtractOperation(), 1);
        //expr1 = new AtomicExpression(new SwapOperation(), 1);
        equation_y1.AddAtomicExpression(expr0);

        _xList = [equation_x0, equation_xlong];
        _yList = [equation_y0, equation_y1];

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

        equation_x0.RepeatCount = 99;
        result = NextPath();
        for (int i = 0; i < _hCount/2; i++)
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

    private void Reset()
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

    private void AddLine(SKPath result)
    {
        var x = (float)(_xList.Select(x => x.CurrentResult.EndValue).Sum() + _origin.X);
        var y = (float)(_yList.Select(y => -y.CurrentResult.EndValue).Sum() + _origin.Y);
        result.LineTo(x, y);
    }


}
