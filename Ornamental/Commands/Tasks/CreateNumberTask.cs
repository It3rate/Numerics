using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsAPI.CommandEngine;
using NumericsCore.Primitives;
using NumericsCore.Utils;

namespace Ornamental.Commands.Tasks
{
    public class CreateNumberTask : TaskBase
    {
        public SymmetricNumber Number { get; private set; }
        public Domain Domain { get; private set; }
        public Focal TopFocal { get; private set; }
        public Focal BottomFocal { get; private set; }
        public override bool IsValid => true;
        public CreateNumberTask(Domain domain, Focal topFocal, Focal bottomFocal)
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

        public override void UnRunTask()
        {
            //Domain.RemoveNumber(Number);
        }
    }
}
