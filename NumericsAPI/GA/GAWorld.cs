using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Motions;
using NumericsCore.Motions.Units;

namespace NumericsAPI.GA
{
	public class GAWorld
	{
		// CreatePopulation
		// WorldLoop
		// Interact
		// Evaluate
		// FightOrBreed
		// Crossover
		// Mutate
		// EnergizeAndExpire
		// Display

		public Random RND = new Random(1234);
		public SymmetricNumber DefaultEnergy() => new SymmetricNumber(WorkingScalar, 0, 10000, Resolution);
		public ScalarUnit WorkingScalar = ScalarUnit.Scalar10K;
		public long Resolution = 10;
		public Focal UnitLimits = new Focal(0000, 10000);
		public List<IUnit> Units { get; } = new List<IUnit>();

		public List<Individual> Population = new List<Individual>();
		public int _traitCount;
		public int _populationCount;
		public GAWorld(int traits, int population) 
		{
			_traitCount = traits;
			_populationCount = population;
			Reset();
		}
		public void Reset()
		{
			CreateUnits(_traitCount);
			CreatePopulation(_populationCount);
			for (int i = 0; i < loopCount; i++)
			{
				Step();
			}
		}
		private void CreateUnits(int count)
		{
			Units.Clear();
			for (int i = 0; i < count; i++)
			{
				Units.Add(new DefaultUnit( ((char)(i + 65)).ToString(), UnitLimits) );
			}
		}

		private void CreatePopulation(int count)
		{
			Population.Clear();
			for (int i = 0; i < count; i++)
			{
				Population.Add(Individual.RandomIndividual(this, Units));
			}
		}
		public void Step()
		{
			var source = RND.Next(Population.Count);
			var a = RND.Next(Population.Count);
			var b = RND.Next(Population.Count);
			var closer = GetCloserItem(source, a, b);
			MoveCloserTo(source, closer);
		}
		public int GetCloserItem(int source, int a, int b)
		{
			var s = Population[source];
			var ai = Population[a];
			var bi = Population[b];
			var ca = s.Compare(ai);
			var cb = s.Compare(bi);
			var result = (ca.AbsLength <= cb.AbsLength) ? a : b;
			return result;
		}
		int loopCount = 1000000;
		public void MoveCloserTo(int source, int target)
		{
			if(source != target)
			{
				Population[source].CombineWith(Population[target]);
				Population[target].CombineWith(Population[source]);
				var dir = source > target ? -1 : 1;
				var anchor = source;
				//dir = dir * (int)(Math.Abs(source - target) * 0.2);
				if (anchor + dir < Population.Count && anchor + dir >= 0)
				{
					var temp = Population[anchor + dir];
					Population[anchor + dir] = Population[anchor];
					Population[anchor] = temp;
				}
				anchor = target;
				dir = -dir;
				//dir = dir * (int)(Math.Abs(source - target) * 0.2);
				if (anchor + dir < Population.Count && anchor + dir >= 0)
				{
					var temp = Population[anchor + dir];
					Population[anchor + dir] = Population[anchor];
					Population[anchor] = temp;
				}
			}
		}
	}
}
