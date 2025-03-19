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
	public class Individual
	{
		private GAWorld _world { get; }
		public List<SymmetricNumber> TraitRanges { get; } = new List<SymmetricNumber>();
		public List<Focal> TraitValues { get; } = new List<Focal>();
		public SymmetricNumber EnergyLevel { get; set; }
		private List<double> IndexWeights;

		public Dictionary<Individual, double> InteractionScores { get; } = new Dictionary<Individual, double>();

		public Individual(GAWorld world, params SymmetricNumber[] traits)
		{
			TraitRanges = traits.ToList();
			_world = world;
			EnergyLevel = world.DefaultEnergy();
			IndexWeights = Enumerable.Repeat(1.0, traits.Length).ToList();
			SampleValues();
		}
		public static Individual RandomIndividual(GAWorld world, IEnumerable<IUnit> units)
		{
			var nums = new List<SymmetricNumber>();
			foreach(var unit in units)
			{
				var midPoint = (long)(unit.Limits.InteriorSample(world.RND) * 0.8);
				var len = (long)(unit.Limits.InteriorSample(world.RND) * 0.2);//world.RND.Next(100);// 
				var sn = new SymmetricNumber(unit, -(midPoint - len), midPoint + len, world.Resolution);
				nums.Add(sn);
			}
			return new Individual(world, nums.ToArray());
		}

		private void SampleValues() 
		{ 
			TraitValues.Clear();
			foreach(var trait in TraitRanges)
			{
				var sample = trait.TopFocal.InteriorSample(_world.RND);
				TraitValues.Add(new Focal(sample, sample));
			}
		}

		public void Update() { }
		private List<int> _lastCompareIndexes = new List<int>();
		public SymmetricNumber Compare(Individual other) 
		{
			var pos = 0.0;
			var neg = 0.0;
			_lastCompareIndexes.Clear();
			var compareCount = 4;
			for (var i = 0; i < compareCount; i++)
			{
				var index = _world.RND.Next(0, TraitValues.Count);
				_lastCompareIndexes.Add(index);
				var dif = other.TraitValues[index].EndTick - TraitValues[index].EndTick;
				if(dif >= 0)
				{
					pos += dif * IndexWeights[i];
				}
				else
				{
					neg += -dif * IndexWeights[i];
				}
			}
			return new SymmetricNumber(_world.WorkingScalar, (long)-neg, (long)pos, _world.Resolution);
		}
		public void CombineWith(Individual other) 
		{
			var bump = 0.1;
			if (_lastCompareIndexes.Count > 0)
			{
				for (int i = 0; i < IndexWeights.Count; i++)
				{
					if (_lastCompareIndexes.Contains(i))
					{
						IndexWeights[i] = Math.Min(2.0, IndexWeights[i] + bump);
					}
					else
					{
						IndexWeights[i] = Math.Max(0.1, IndexWeights[i] - bump);
					}
				}
			}
			_lastCompareIndexes.Clear();
		}
		public void Mutate() { }

		// Create
		// EnergyLevel
		// TraitRanges
		// TraitValues
		// Compare
		// Update
		// Combine
		// Mutate
		// History

	}
}
