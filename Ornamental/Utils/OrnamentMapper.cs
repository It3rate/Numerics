using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsAPI.GA;
using NumericsSkia.Agent;
using NumericsSkia.Drawing;
using NumericsSkia.Mappers;
using NumericsSkia.Renderer;
using SkiaSharp;

namespace Ornamental.Utils
{
    public class OrnamentMapper : SKMapper
    {
        public GAWorld? World { get; set; }
        //public List<SKPath> Paths { get; } = new List<SKPath>();
        public OrnamentMapper(RenderAgent agent, float left, float top, float width, float height) : base(agent)
        {
            Reset(new SKPoint(left, top), new SKPoint(left + width, top + height));
        }
        public Number TestNumber { get; set; }
        public override void Draw()
        {
            Renderer.DrawLine(Guideline, Pens.SegPen0);
            foreach (SKPath path in Paths)
            {
                var pen = path.Points[0].X <= path.Points[1].X ? Pens.SegPen1 : Pens.SegPen3;

				Renderer.DrawPolyline(pen, path.Points);
            }
            if(Label != "")
            {
                Renderer.DrawTextAt(new SKPoint(30, 50), Label, Pens.LabelBrush);
            }

            if(World != null)
			{
                int y = 100;
				foreach (var individual in World.Population)
				{
					individual.DrawAt(Agent, 650, y+=4);
				}
			}

		}
    }
}
