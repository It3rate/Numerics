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
			ResampleValues();
			ResetShifts();
		}
		public double TraitValueAt(int index) => TraitSamples[index] + TraitShifts[index];
		public static Individual RandomIndividual(GAWorld world, IEnumerable<IUnit> units)
		{
			var nums = new List<SymmetricNumber>();
			foreach (var unit in units)
			{
				nums.Add(RandomTrait(world, unit));
			}
			var result = new Individual(world, nums.ToArray());
			//var index = result.IndexWeights.IndexOf(result.IndexWeights.Min());
			//var index = world.RND.Next(result.IndexWeights.Count);
			//result.IndexWeights[index] = 255;
			return result;
		}
		public static Individual AlignedIndividual(GAWorld world, IEnumerable<IUnit> units)
		{
			var nums = new List<SymmetricNumber>();
			var trait = RandomTrait(world, units.First());
			foreach (var unit in units)
			{
				nums.Add(trait);
			}
			var result = new Individual(world, nums.ToArray());
			return result;
		}
		public static SymmetricNumber RandomTrait(GAWorld world, IUnit unit)
		{
			var midPoint = unit.Limits.InteriorSample(world.RND);
			var len = (long)(unit.Limits.InteriorSample(world.RND) * 0.1);//world.RND.Next(100);// 
			var result = new SymmetricNumber(unit, (midPoint - len), midPoint + len, world.Resolution);
			return result;
		}
		public void ResetShifts()
		{
			TraitShifts.Clear();
			foreach (var trait in TraitRanges)
			{
				TraitShifts.Add(0);
			}
		}
		public void ResampleValues()
		{
			TraitSamples.Clear();
			foreach (var trait in TraitRanges)
			{
				var sample = trait.InteriorSample(_world.RND);
				TraitSamples.Add(sample);
			}
		}

		public void Update() { }
		private List<int> _lastCompareIndexes = new List<int>();
		public SymmetricNumber Compare(Individual other) 
		{
			var pos = 0.0;
			var neg = 0.0;
			_lastCompareIndexes.Clear();
			var compareCount = Math.Min(TraitSamples.Count, _world.CompareCount);
			for (var i = 0; i < compareCount; i++)
			{
				var index = _world.RND.Next(0, TraitSamples.Count);
				_lastCompareIndexes.Add(index);
				var dif = other.TraitSamples[index] - TraitSamples[index];
				if(dif >= 0)
				{
					pos += dif * TraitRanges[i].AbsLength;// IndexWeights[i];
				}
				else
				{
					neg += -dif * TraitRanges[i].AbsLength;//IndexWeights[i];
				}
			}
			return new SymmetricNumber(_world.WorkingScalar, (long)-neg, (long)pos, _world.Resolution);
		}
		public int GetNearestIndex(Individual other)
		{
			var min = double.MaxValue;
			var index = -1;
			for (int i = 0; i < TraitSamples.Count; i++)
			{
				var dif = Math.Abs(TraitSamples[i] - other.TraitSamples[i]);
				if (dif < min)
				{
					min = dif;
					index = i;
				}
			}
			return index;
		}
		public double MaxWeight = 25.5;
		public void CombineWith(Individual other) 
		{
			var shift = 2;
			if (_lastCompareIndexes.Count > 0)
			{
				var index = GetNearestIndex(other);
				//var max = Math.Max(IndexWeights[index], other.IndexWeights[index]) + (_world.TraitCount / _world.CompareCount);
				//var max = IndexWeights.Max();
				//var index = IndexWeights.IndexOf(max);
				//max += 0.02;
				var target = (TraitSamples[index] - other.TraitSamples[index]) / 2.0 + TraitSamples[index];
				//var target = (TraitShifts[index] < other.TraitShifts[index]) ?  TraitSamples[index] : other.TraitSamples[index];
				for (int i = 0; i < TraitSamples.Count; i++)
				{
					if (_lastCompareIndexes.Contains(i))
					{
						//max = Math.Max(IndexWeights[i], other.IndexWeights[i]) + 2;
						//IndexWeights[i] = Math.Min(MaxWeight, max);
						//other.IndexWeights[i] = Math.Min(MaxWeight, max);

						//TraitShifts[i] += TraitValueAt(i) > target ?  -shift : shift;
						//other.TraitShifts[i] += other.TraitValueAt(i) > target ? -shift : shift;


						TraitRanges[i].ShiftFromCenter(TraitValueAt(i) > target ? -shift : shift);
						other.TraitRanges[i].ShiftFromCenter(other.TraitValueAt(i) > target ? -shift : shift);

						if (TraitRanges[i].AbsLength > 40)
						{ 
							TraitRanges[i].ExpandFromCenter(-2);
						}
						if (other.TraitRanges[i].AbsLength > 40)
						{
							other.TraitRanges[i].ExpandFromCenter(-2);
						}
					}
					else
					{
						//var min = Math.Min(IndexWeights[i], other.IndexWeights[i]) * (1.0 - _world.CompareCount / _world.TraitCount);
						//IndexWeights[i] = Math.Max(0, min);
						//other.IndexWeights[i] = Math.Max(0, min);
						//TraitRanges[i].ExpandFromCenter(shift/2.0);
						//other.TraitRanges[i].ExpandFromCenter(shift / 2.0);
						TraitRanges[i].ShiftFromCenter(TraitValueAt(i) > target ? shift/4.0 : -shift / 4.0);
						other.TraitRanges[i].ShiftFromCenter(other.TraitValueAt(i) > target ? shift / 4.0 : -shift / 4.0);
						if (TraitRanges[i].AbsLength > 40)
						{
							TraitRanges[i].ExpandFromCenter(0.7);
						}
						if (other.TraitRanges[i].AbsLength > 40)
						{
							other.TraitRanges[i].ExpandFromCenter(0.7);
						}
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
