using System;

namespace Code.Core.Utils
{
public static class RandomExtensions
{
    public static float NextFloat(this Random random, float minValue, float maxValue)
    {
        double range = maxValue - minValue;
        var sample = random.NextDouble();
        
        return (float)(minValue + (sample * range));
    }
}
}