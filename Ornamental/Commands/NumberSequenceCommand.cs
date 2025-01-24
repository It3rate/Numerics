using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.CoreConcepts.Time;
using Numerics.Primitives;
using NumericsAPI.CommandEngine;
using NumericsCore.Primitives;
using NumericsSkia.Agent;
using NumericsSkia.Drawing;
using NumericsSkia.Mappers;
using Ornamental.Commands.Tasks;

namespace Ornamental.Commands
{
    public class NumberSequenceCommand : CommandBase
    {
        // selection
        // context
        // start task
        // updates
        // end task
        public Domain Domain => NumberSequenceTask.Domain;
        public SymmetricNumber Number => NumberSequenceTask.Number;
        public SymmetricNumber InProgressNumber => NumberSequenceTask.InProgressNumber ?? NumberSequenceTask.Number;
        public NumberSequenceTask NumberSequenceTask { get; set; }
        public NumberSequenceCommand(Domain domain, Focal topFocal, Focal bottomFocal, MillisecondNumber? duration = null) : base(duration)
            //Trait trait, long basisStart, long basisEnd, long minMaxStart, long minMaxEnd, SKSegment guideline, SKSegment unitSegment, string name) : base(guideline)
        {
            NumberSequenceTask = new NumberSequenceTask(domain, topFocal, bottomFocal, Duration);
        }

        public override void Execute()
        {
            if (NumberSequenceTask != null)
            {
                Tasks.Add(NumberSequenceTask);
            }
            base.Execute();

            //Mapper = MouseAgent.WorkspaceMapper.GetOrCreateDomainMapper(Domain, Guideline, UnitSegment);
            //if (Guideline.StartPoint.X > Guideline.EndPoint.X)
            //{
            //    DomainMapper.FlipRenderPerspective();
            //}
        }
        public override void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime)
        {
            base.Update(currentTime, deltaTime);
            NumberSequenceTask.Update(currentTime, deltaTime);
        }


        public override void Unexecute()
        {
            base.Unexecute();
           // MouseAgent.WorkspaceMapper.RemoveDomainMapper(DomainMapper);
        }

        public override void Completed()
        {
        }
    }
}
