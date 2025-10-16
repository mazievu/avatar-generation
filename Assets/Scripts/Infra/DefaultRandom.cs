using System;
using LifeSim.Core.Services;


namespace LifeSim.Core.Infra
{
public class DefaultRandom : IRandom
{
private readonly Random _r;
public DefaultRandom(int? seed = null) { _r = seed.HasValue ? new Random(seed.Value) : new Random(); }
public int NextInt(int minInclusive, int maxInclusive) => _r.Next(minInclusive, maxInclusive + 1);
public float NextFloat() => (float)_r.NextDouble();
}
}