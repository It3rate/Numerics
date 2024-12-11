using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericsCore.Interfaces
{
    public enum TileMode
    {
        OneShot, // only run once no matter what
        Continue, // continue past segment
        Clamp,  // use endpoint value
        Bounce, // reverse
        Loop,   // back to start
        InvertLoop, // change polarity and back to start
        InvertBounce, // change polarity and reverse
    }
}
