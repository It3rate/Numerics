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

    private SKMapper AnimationTest()
    {
        var wm = new OrnamentMapper(_currentMouseAgent, 100, 500, 1050, 0);

        Init();
        var result = new SKPath();
        var sp = wm.Guideline.StartPoint;
        result.MoveTo(sp);

        var offset = new Number(_domain.DefaultBasisNumber, new(0, 0));
        var input = new Number(_domain.DefaultBasisNumber, new(0, 6000));
        var path = new Number(_domain.DefaultBasisNumber, new(0, 30000));
        var lm1 = new Landmark(path, 0.2);
        var lm2 = new Landmark(path, 0.8);
        var lmn = new Number(_domain.DefaultBasisNumber, lm1, lm2);
        var seedList = new List<Number> { offset };

        var equation = new Expression(seedList, 30, TileMode.Continue, true);
        var step1 = new AtomicExpression(0, new AddOperation(), 1);
        var step2 = new AtomicExpression(new InvertOperation(), 1);
        equation.AddAtomicExpression(step1, step2);
        equation.SetInput(input);

        for (int i = 0; i < 30; i++)
        {
            var output = equation.Next();
            result.LineTo(
                (float)(output.StartValue + sp.X + i * 35), 
                (float)(output.EndValue + sp.Y));
        }
        wm.Path = result;
        return wm;
    }


}
