using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.CoreConcepts.Time;
using NumericsSkia.Agent;
using NumericsSkia.Renderer;
using NumericsSkia.Utils;
using Ornamental.Commands;
using SkiaSharp;

namespace Ornamental.Agent
{
    public class OrnamentAgent : RenderAgent
    {
        OrnamentalForm Form;
        public OrnamentAgent(OrnamentalForm form, CoreRenderer renderer, IDemos demos) : base(renderer, demos)
        {
            Form = form;
        }

        public override void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime)
        {
            base.Update(currentTime, deltaTime);
            foreach (var cmd in Stack.CurrentCommands)
            {
                cmd.Update(currentTime, deltaTime);
            }
            Form.NeedsUpdate();
        }
        public override void Draw()
        {
            base.Draw();

            var cmds = Stack.CurrentCommands;
            foreach (var cmd in cmds) 
            {
                if (cmd is NumberSequenceCommand cnc)
                {
                    DrawNumberSequence(cnc);
                }
            }
        }

        private void DrawNumberSequence(NumberSequenceCommand cnc)
        {
            var guide = Mapper.Guideline;
            var interpNum = cnc.InProgressNumber;
            var p0 = new SKPoint((float)interpNum.StartValue, 0) + guide.StartPoint;
            var p1 = new SKPoint((float)(interpNum.EndValue), 0) + guide.StartPoint;
            Renderer.DrawLine(p0, p1, Renderer.Pens.SegPenHighlight);
        }
    }
}
