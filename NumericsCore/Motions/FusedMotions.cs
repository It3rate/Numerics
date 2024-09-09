using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Expressions;

namespace NumericsCore.Composites
{
    /// <summary>
    /// Multidimensional Motions like 2D paths or color, which are ultimately a form of integration/interpolation.
    /// </summary>
    public class FusedMotions
    {
        // each dimension gets a new domain? Like the same traits, but the domain is mallable to allow weighting, resolution and valence.
        // more than one domain. These can be related by:
        // ratio (division) - like miles per hour, one domain will be unit, the other the measure 
        // area (multiplication) - like scale, size, total over time. The multiplication doesn't need to be proportional and can invert polarity
        // fixed area (mult) - like voltage/current, where total is const and change in one inverts the other
        // combination (addition) - like path distance, meta grouping (track 'fruit' from various kinds). Can be used to combine multiple directives like in ornamental drawing.
        // cancelling (subtraction) - like spoilage when tracking food across types.
        // accellerating (pow) - like m/s/s, growth in volume per width,
        // repeating (eq pow) - like repeating patterns, reusuable formulas, closed paths
        // sin/cos (trig) - in addition to many other special cases, fibonacci, n! etc. These are common equations made of the above, not unique ops. But can be directly encoded.

        // all of these can combine in multiple steps, and two domains can interact differently in different contexts.


        // ornamental line that has 3 units, two horizontal and one vertical. eg:
        // ---------------> repeat
        // ^.v.^.v... repeat
        // <-...<-... repeat
        // can create square wave. Without #3 it's sawtooth. This pattern naturally ends up at next start point.

        public List<Domain> Domains { get; } = new List<Domain>();
        public List<Expression> Equations {  get; } = new List<Expression>();
    }
}
