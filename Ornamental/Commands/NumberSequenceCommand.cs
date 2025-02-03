using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.CoreConcepts.Time;
using Numerics.Primitives;
using NumericsAPI.CommandEngine;
using NumericsCore.Motions;
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
        public List<SymmetricNumber> Numbers { get; } = new List<SymmetricNumber>();
        public Dictionary<string, SymmetricNumber> Values { get; } = new Dictionary<string, SymmetricNumber>();
        public NumberSequenceCommand(MillisecondNumber duration, params NumberSequenceTask[] tasks) : base(duration)
        {
            InputTasks.AddRange(tasks);
        }
        //public NumberSequenceCommand(Domain domain, Focal topFocal, Focal bottomFocal, MillisecondNumber? duration = null) : base(duration)
        //    //Trait trait, long basisStart, long basisEnd, long minMaxStart, long minMaxEnd, SKSegment guideline, SKSegment unitSegment, string name) : base(guideline)
        //{
        //    NumberSequenceTasks = new NumberSequenceTask(domain, topFocal, bottomFocal, Duration);
        //}

        public override void Execute()
        {
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
            Values.Clear();
            foreach (var task in Tasks.ToArray())
            {
                if(task is NumberSequenceTask nst)
                {
                    task.Update(currentTime, deltaTime);
                    Values.Add(nst.TraitName, nst.InProgressNumber);
                }
            }
        }


        public override void Unexecute()
        {
            base.Unexecute();
           // MouseAgent.WorkspaceMapper.RemoveDomainMapper(DomainMapper);
        }

        public override void Completed()
        {
            Trace.WriteLine("complete");
        }
    }
}
