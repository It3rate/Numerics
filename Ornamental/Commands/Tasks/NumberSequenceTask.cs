using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.CoreConcepts.Time;
using Numerics.Primitives;
using NumericsAPI.CommandEngine;
using NumericsCore.Primitives;
using NumericsCore.Utils;
using NumericsSkia.Mappers;
using SkiaSharp;

namespace Ornamental.Commands.Tasks
{
    public class NumberSequenceTask : TaskBase
    {
        public SymmetricNumber Number { get; private set; }
        public SymmetricNumber? InProgressNumber { get; private set; }
        public Domain Domain { get; private set; }
        public Focal TopFocal { get; private set; }
        public Focal BottomFocal { get; private set; }
        public override bool IsValid => true;
        public NumberSequenceTask(Domain domain, Focal topFocal, Focal bottomFocal, MillisecondNumber duration) : base(duration)
        {
            Domain = domain;
            TopFocal = topFocal;
            BottomFocal = bottomFocal;
            Number = new SymmetricNumber(domain, topFocal, bottomFocal);
        }
        public override void RunTask()
        {
            if (Number == null)
            {
                Number = new SymmetricNumber(Domain, TopFocal, BottomFocal);
            }
            //Domain.AddNumber(Number);
        }

        public override void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime)
        {
            base.Update(currentTime, deltaTime);
            InProgressNumber = Number.Interpolate(0, InterpolationT);

            //var interp = currentTime.EndTick / (float)Timer.DurationValue;
            //var interpNum = cnc.Number.Interpolate(0, interp, true);
            //var p0 = new SKPoint((float)interpNum.StartValue, 0) + guide.StartPoint;
            //var p1 = new SKPoint((float)(interpNum.EndValue), 0) + guide.StartPoint;
            //Renderer.DrawLine(p0, p1, Renderer.Pens.SegPenHighlight);
        }

        public override void UnRunTask()
        {
            //Domain.RemoveNumber(Number);
        }
    }
}
