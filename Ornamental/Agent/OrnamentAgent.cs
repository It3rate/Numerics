using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumericsSkia.Agent;
using NumericsSkia.Renderer;
using NumericsSkia.Utils;
using Ornamental.Commands;
using SkiaSharp;

namespace Ornamental.Agent
{
    public class OrnamentAgent : RenderAgent
    {
        public OrnamentAgent(CoreRenderer renderer, IDemos demos) : base(renderer, demos)
        {
        }

        public override void Draw()
        {
            base.Draw();

            var cmds = Stack.CurrentCommands;
            foreach (var cmd in cmds) 
            {
                if (cmd is CreateNumberCommand cnc)
                {
                    var nm = cnc.Number;
                    var p0 = new SKPoint((float)nm.StartValue, 200);
                    var p1 = new SKPoint((float)nm.EndValue, 200);
                    Renderer.DrawLine(p0, p1, Renderer.Pens.SegPenHighlight);
                }
            }
        }
    }
}
