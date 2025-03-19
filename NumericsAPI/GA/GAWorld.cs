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
		public long Resolution = 16;
		public Focal UnitLimits = new Focal(-10000, 10000);
		public List<IUnit> Units { get; } = new List<IUnit>();

		public List<Individual> Population = new List<Individual>();
		public GAWorld(int traits, int population) 
		{
			CreateUnits(traits);
			CreatePopulation(population);
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

	}
}
