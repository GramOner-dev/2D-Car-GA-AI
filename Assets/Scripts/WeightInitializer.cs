using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class WeightInitializer
{
    private static System.Random random = new System.Random();
    private static float RandomGaussian()
    {
        double r1 = random.NextDouble();
        double r2 = random.NextDouble();

        //box muller transform
        double randomGuassianVarience = Math.Sqrt(-2.0 * Math.Log(r1)) * Math.Cos(2.0 * Math.PI * r2);

        return (float)randomGuassianVarience;
    }

    private static float HeInitialization(int numberOfInputs)
    {
        //standard deviation for the normal distribution
        float stdDev = (float)Math.Sqrt(2.0 / numberOfInputs);

        return RandomGaussian() * stdDev;
    }

    public static float RandomFloatInRange(float min, float max)
    {
        return (float)(min + random.NextDouble() * (max - min));
    }

    public static float[] GetRandomWeights(int numberOfInputs)
    {
        float[] initializedWeights = new float[numberOfInputs];
        for (int i = 0; i < numberOfInputs; i++)
        {
            // initializedWeights[i] = HeInitialization(numberOfInputs) / 8;
            initializedWeights[i] = UnityEngine.Random.Range(-1.0f, 1f);

        }
        return initializedWeights;
    }
}
