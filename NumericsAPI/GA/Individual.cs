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
		private static int _nextID = 0;
		public int ID { get; }
		private GAWorld _world { get; }
		public List<SymmetricNumber> TraitRanges { get; } = new List<SymmetricNumber>();
		private List<double> TraitShifts { get; } = new List<double>();
		private List<double> TraitSamples { get; } = new List<double>();

		public SymmetricNumber EnergyLevel { get; set; }
		public List<double> IndexWeights;

		public Dictionary<Individual, double> InteractionScores { get; } = new Dictionary<Individual, double>();

		public Individual(GAWorld world, params SymmetricNumber[] traits)
		{
			ID = _nextID++;
			TraitRanges = traits.ToList();
			_world = world;
			EnergyLevel = world.DefaultEnergy();
			IndexWeights = Enumerable.Repeat(1.0, traits.Length).ToList();
			SampleValues();
		}
		public double TraitValueAt(int index) => TraitSamples[index] + TraitShifts[index];
		public static Individual RandomIndividual(GAWorld world, IEnumerable<IUnit> units)
		{
			var nums = new List<SymmetricNumber>();
			foreach(var unit in units)
			{
				var midPoint = unit.Limits.InteriorSample(world.RND);
				midPoint = (long)(midPoint * 0.8 + midPoint * 0.1);
				var len = (long)(unit.Limits.InteriorSample(world.RND) * 0.1);//world.RND.Next(100);// 
				var sn = new SymmetricNumber(unit, -(midPoint - len), midPoint + len, world.Resolution);
				nums.Add(sn);
			}
			return new Individual(world, nums.ToArray());
		}

		private void SampleValues()
		{
			TraitShifts.Clear();
			TraitSamples.Clear();
			foreach (var trait in TraitRanges)
			{
				var sample = trait.InteriorSample(_world.RND);
				TraitSamples.Add(sample);
				TraitShifts.Add(0);
			}
		}

		public void Update() { }
		private List<int> _lastCompareIndexes = new List<int>();
		public SymmetricNumber Compare(Individual other) 
		{
			var pos = 0.0;
			var neg = 0.0;
			_lastCompareIndexes.Clear();
			var compareCount = Math.Min(TraitSamples.Count, 4);
			for (var i = 0; i < compareCount; i++)
			{
				var index = _world.RND.Next(0, TraitSamples.Count);
				_lastCompareIndexes.Add(index);
				var dif = other.TraitSamples[index] - TraitSamples[index];
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
		public double MaxWeight = 25.5;
		public void CombineWith(Individual other) 
		{
			var shift = 1;
			if (_lastCompareIndexes.Count > 0)
				{
				var max = IndexWeights.Max();
				var index = IndexWeights.IndexOf(max);
				max += 0.02;
				var mid = (TraitSamples[index] - other.TraitSamples[index]) / 2.0 + TraitSamples[index];
				for (int i = 0; i < IndexWeights.Count; i++)
				{
					if (_lastCompareIndexes.Contains(i))
					{
						//var max = Math.Max(IndexWeights[i], other.IndexWeights[i]) * 4;
						IndexWeights[i] = Math.Min(MaxWeight, max);
						other.IndexWeights[i] = Math.Min(MaxWeight, max);

						TraitShifts[i] += TraitValueAt(i) > mid ?  -shift : shift;
						other.TraitShifts[i] += other.TraitValueAt(i) > mid ? -shift : shift;
					}
					else
					{
						var min = Math.Min(IndexWeights[i], other.IndexWeights[i]) * 0.4;
						IndexWeights[i] = Math.Max(0, min);
						other.IndexWeights[i] = Math.Max(0, min);
					}
				}
			}
			_lastCompareIndexes.Clear();
		}
		public void Mutate() { }

		public override string ToString()
		{
			return "I:" + ID.ToString() + " " + TraitSamples[0].ToString();
		}
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
