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
		public int TraitCount;
		public int PopulationCount;
		public int CompareCount = 4;
		public int LoopCount;
		public Individual TempTarget;
		public GAWorld(int traits, int population, int compareCount, int loopCount) 
		{
			TraitCount = traits;
			PopulationCount = population;
			CompareCount = compareCount;
			LoopCount = loopCount;
			Reset();
		}
		public void Reset()
		{
			CreateUnits(TraitCount);
			CreatePopulation(PopulationCount);
			for (int i = 0; i < LoopCount; i++)
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
				if(true)//RND.Next(2) == 0)
				{
					Population.Add(Individual.RandomIndividual(this, Units));
				}
				else
				{
					Population.Add(Individual.AlignedIndividual(this, Units));
				}
			}
			TempTarget = Individual.AlignedIndividual(this, Units);
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
		public void MoveCloserTo(int source, int target)
		{
			if(source != target)
			{
				var src = Population[source];
				var trg = Population[target];

				src.CombineWith(trg);
				trg.CombineWith(src);
				var dir = source > target ? -1 : 1;
				MovePosition(source, dir);
				MovePosition(target, -dir);
			}
		}
		private void MovePosition(int src, int shift)
		{
			var srcPop = Population[src];
			if (src + shift < Population.Count && src + shift >= 0)
			{
				var temp = Population[src + shift];
				Population[src + shift] = srcPop;
				Population[src] = temp;
			}
			srcPop.ResampleValues();
		}
	}
}
