using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.CoreConcepts.Time;
using Numerics.Primitives;
using NumericsAPI.CommandEngine;
using NumericsCore.Motions;
using NumericsCore.Utils;
using NumericsSkia.Mappers;
using SkiaSharp;

namespace Ornamental.Commands.Tasks
{
    public class NumberSequenceTask : TaskBase
    {
        public string TraitName => Domain.Trait != null ? Domain.Trait.Name : "";
        public SymmetricNumber Number { get; private set; }
        public SymmetricNumber InProgressNumber { get; private set; }
        public Domain Domain { get; private set; }
        public Focal TopFocal { get; private set; }
        public Focal BottomFocal { get; private set; }
        public override bool IsValid => true;
        public NumberSequenceTask(Domain domain, Focal topFocal, Focal bottomFocal, MillisecondNumber duration) : base(duration)
        {
            Domain = domain;
            TopFocal = topFocal;
            BottomFocal = bottomFocal;
        }
        public override void RunTask()
        {
            if (Number == null)
            {
                Number = new SymmetricNumber(Domain, TopFocal, BottomFocal);
                InProgressNumber = Number.Interpolate(0, 0); ;
            }
            //Domain.AddNumber(Number);
        }

        public override void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime)
        {
            base.Update(currentTime, deltaTime);
            if(InterpolationT >= 1f)
            {
                InProgressNumber = Number.Interpolate(0, 1f);
                Number.TopFocal.Swap();
                Timer.Restart();
            }
            else
            {
                InProgressNumber = Number.Interpolate(0, InterpolationT);
            }
        }

        public override void UnRunTask()
        {
            //Domain.RemoveNumber(Number);
        }
    }
}
