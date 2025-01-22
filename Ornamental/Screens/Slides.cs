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
    private Trait _trait = null!;
    private Focal _basisFocal = null!;
    private Focal _limits = null!;
    private Domain _domain = null!;
    private static double _delta = 0.001;

    public void Init()
    {
        _trait = new Trait("Tests");
        _basisFocal = new Focal(0, 100);
        _limits = new Focal(-10000, 10000);
        _domain = new Domain(_trait, _basisFocal, _limits);
    }

    public Slides()
    {
        _testIndex = 1;// 
        Pages.AddRange(new PageCreator[]
        {
            OrnamentTest
        });
    }
    private List<Expression> _xList;
    private List<Expression> _yList;
    SKPoint _origin;
    int _hCount = 25;
    int _space = 80;


    private SKMapper OrnamentTest()
    {
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
        var y = (float)(_yList.Select(y => -y.CurrentResult.StartValue).Sum() + _origin.Y);
        result.LineTo(x, y);
    }


}
