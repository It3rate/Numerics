using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumericsAPI.GA;
using NumericsSkia.Agent;
using NumericsSkia.Renderer;
using SkiaSharp;

namespace NumericsSkia.Mappers
{
	public static class IndividualExtensions
	{
		public static SKPaint PosPen = CorePens.GetPen(SKColor.Parse("#6030E0D0"), 2f);
		public static SKPaint NegPen = CorePens.GetPen(SKColor.Parse("#60D03090"), 2f);
		public static void DrawAt(this Individual individual, RenderAgent agent, int x, int y)
		{
			var renderer = agent.Renderer;
			foreach (var num in individual.TraitRanges)
			{
				//var sn = trait.SymmetricNumber;
				//var path = renderer.CreatePath(sn, x, y);
				//renderer.DrawPath(path, renderer.Pens.SegPen0);
				var pen = num.IsPositiveDirection ? PosPen : NegPen;

				renderer.DrawLine(
					new SKPoint((float)num.StartValue + x, y), 
					new SKPoint((float)num.EndValue + x, y), 
					pen);
			}
		}
	}
}
