using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsSkia.Agent;
using NumericsSkia.Drawing;
using NumericsSkia.Mappers;
using NumericsSkia.Renderer;
using SkiaSharp;

namespace Ornamental.Utils
{
    public class OrnamentMapper : SKMapper
    {
        public OrnamentMapper(MouseAgent agent, float left, float top, float width, float height) : base(agent)
        {
            Reset(new SKPoint(left, top), new SKPoint(left + width, top + height));
        }
        public Number TestNumber { get; set; }
        public override void Draw()
        {
            Renderer.DrawLine(Guideline, Pens.SegPen0);
        }
    }
}
