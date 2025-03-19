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

		public Random RND = new Random(1000);
		public SymmetricNumber DefaultEnergy() => new SymmetricNumber(WorkingScalar, 0, 10000, Resolution);
		public ScalarUnit WorkingScalar = ScalarUnit.Scalar10K;
		public long Resolution = 100;
		public Focal TraitFocal = new Focal(-1000, 1000);
		public List<IUnit> Traits { get; } = new List<IUnit>();

		public List<Individual> Population = new List<Individual>();
		public GAWorld(int traits, int population) 
		{
			CreateTraits(traits);
			CreatePopulation(population);
		}
		private void CreateTraits(int count)
		{
			Traits.Clear();
			for (int i = 0; i < count; i++)
			{
				Traits.Add(new DefaultUnit( ((char)(i + 65)).ToString(), TraitFocal) );
			}
		}

		private void CreatePopulation(int count)
		{
			Population.Clear();
			for (int i = 0; i < count; i++)
			{
				Population.Add(Individual.RandomIndividual(this, Traits));
			}
		}

	}
}
