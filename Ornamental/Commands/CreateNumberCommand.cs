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
    public class CreateNumberCommand : CommandBase
    {
        // selection
        // context
        // start task
        // updates
        // end task
        public Domain Domain => CreateNumberTask.Domain;
        public SymmetricNumber Number => CreateNumberTask.Number;//{ get; private set; }
        public CreateNumberTask CreateNumberTask { get; set; }
        public CreateNumberCommand(Domain domain, Focal topFocal, Focal bottomFocal)
            //Trait trait, long basisStart, long basisEnd, long minMaxStart, long minMaxEnd, SKSegment guideline, SKSegment unitSegment, string name) : base(guideline)
        {
            CreateNumberTask = new CreateNumberTask(domain, topFocal, bottomFocal);
        }

        public override void Execute()
        {
            if (CreateNumberTask != null)
            {
                Tasks.Add(CreateNumberTask);
            }
            base.Execute();

            //Mapper = MouseAgent.WorkspaceMapper.GetOrCreateDomainMapper(Domain, Guideline, UnitSegment);
            //if (Guideline.StartPoint.X > Guideline.EndPoint.X)
            //{
            //    DomainMapper.FlipRenderPerspective();
            //}
        }

        public override void Unexecute()
        {
            base.Unexecute();
           // MouseAgent.WorkspaceMapper.RemoveDomainMapper(DomainMapper);
        }

        public override void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime)
        {
            base.Update(currentTime, deltaTime);
        }

        public override void Completed()
        {
        }
    }
}
