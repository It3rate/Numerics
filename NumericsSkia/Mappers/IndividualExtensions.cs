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
			for (int i = 0; i < individual.TraitRanges.Count; i++)
			{
				var weight = individual.IndexWeights[i];
				var alpha = (byte)(weight * (255 / individual.MaxWeight));
				var num = individual.TraitRanges[i];
				var pen1 = CorePens.GetPen(new SKColor(100, 50, 50, alpha), 2f);

				renderer.DrawLine(
					new SKPoint((float)num.StartValue + x, y),
					new SKPoint((float)num.EndValue + x, y),
					pen1);
				//var sn = trait.SymmetricNumber;
				//var path = renderer.CreatePath(sn, x, y);
				//renderer.DrawPath(path, renderer.Pens.SegPen0);
				//var pen = num.IsPositiveDirection ? PosPen : NegPen;
				var sample = individual.TraitValueAt(i);
				var pen = CorePens.GetPen(new SKColor((byte)(128 - weight * 4), (byte)(50 + weight * 8), 100, alpha), 2f);

				renderer.DrawLine(
					new SKPoint((float)(sample - 2) + x, y),
					new SKPoint((float)(sample + 2) + x, y),
					pen);
			}
		}
		
	}
}
