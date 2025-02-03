using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.CoreConcepts.Time;
using NumericsCore.Interfaces;

namespace NumericsCore.Motions
{
    /// <summary>
    /// Operations that have a duration and wrap mode
    /// </summary>
    public class Motion
    {
        public SymmetricNumber Result { get; }
        public SymmetricNumber InProgressNumber { get; private set; }
        public int RepeatCount { get; } = -1;
        public int _repeatIndex = 0;
        public TileMode TileMode { get; } = TileMode.OneShot;
        public MillisecondNumber? Duration { get; }
        public float InterpolationT;
        public bool IsRepeatsComplete => TileMode != TileMode.Continue && _repeatIndex >= RepeatCount;
        public bool IsDynamic { get; set; } = false;

    }
}
