using Numerics.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericsCore.Primitives
{
	public class Composite
	{
		public Number Numerator { get; }
		public Number Denominator { get; }

		public Number Normalized => Numerator / Denominator;
		public (Number, Number) ScaleByTrait(Number value)
		{
			// A scalar can op with anything. Comparing two matched traits is always a meaningful scalar.
			// comparing unmatched traits is only 'meaningful' if there is a trend, otherwise it is just data or random.
			// Two+ measures are required to check a trend (more is better), unless the trend is already known.
			// ** Need the math to compare two composites (eg position change over duration) and get speed/accel/offsets etc

			// if trait matches denominator, divide value by denominator, and multiply that scalar by numerator
			// 3 mph : 2hours = 2/1 (h) * 3 miles = (6 miles, 2 hours) // 2 hours is input 
			 
			// if trait matches numerator, divide value by numerator, and multiply that scalar by denominator
			// 3 mph : 6 miles = 6/3 miles * 1 hour = (6 miles, 2 hours) // 6 miles is input

			// if both traits match this value can become a scalar, and can be OPed by the input
			// or they relate like X,Y, and become an area/hypontenuse/sin:cos type force 
			// perhaps matching traits can do all of these, not sure there is a rule other than what user wants?

			// combos: series [a,b,c], dimensional (X,Y), fused (a*b), relational(a:b), offsets (a+b), repeats
			// a series is sampled in ordered times
			// dimensional numbers can sample as ordered (manhattan),  combined (line) or ordered combined (area)
			//   - to get triangles, you combine X with Y continuously (x, as high as current y), but not Y with X.
			// a definition is the ranges of all relations (a:b:c...), and combos (bananas green->yellow:hard->soft)

			throw new NotImplementedException();
		}
	}
}
