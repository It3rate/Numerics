using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericsCore.Interfaces
{
    public enum TileMode
    {
        Ignore, // continue past segment
        Clamp,  // use endpoint value
        Bounce, // reverse
        Invert, // change polarity and reverse
        Loop,   // back to start
    }
}
