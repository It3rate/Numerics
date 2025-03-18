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

        float _x;
        float _y;
        float _prevX;
        float _prevY;
        List<SKPoint> _points = new List<SKPoint>();
		private void DrawNumberSequence(NumberSequenceCommand cnc)
        {
            var guide = Mapper.Guideline;
            _x = guide.StartPoint.X;
            _y = guide.StartPoint.Y;

            foreach (var val in cnc.Values)
            {
                if (val.Key == "X")
                {
                    _x += (float)val.Value.EndValue;
                }
                else if (val.Key == "Y")
                {
                    _y += (float)val.Value.EndValue;
                }

            }
            if(_x != _prevX || _y != _prevY)
            {
                _points.Add(new SKPoint(_x, _y));
            }

            Renderer.DrawPolyline(Renderer.Pens.SegPen1, _points.ToArray());
        }
    }
}
