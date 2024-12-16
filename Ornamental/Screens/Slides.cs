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

public class Slides : DemoBase
{
    private Random rnd = new Random();
    private SKPaint _oldTextPen;
    private SKPaint _newTextPen;
    public Slides()
    {
        _testIndex = 24; //0;// 
        Pages.AddRange(new PageCreator[]
        {
            AnimationTest,
        });
    }

    private SKMapper AnimationTest()
    {
        var wm = new OrnamentMapper(_currentMouseAgent, 100, 300, 1050, 350);


        return wm;
    }


}
