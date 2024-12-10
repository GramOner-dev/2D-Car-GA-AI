using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network
{
    private State currentState;

    public Layer[] layers;
    public Network(int[] layerSizes)
    {
        int numberOfLayers = layerSizes.Length - 1;
        layers = new Layer[numberOfLayers];
        for (int i = 0; i < numberOfLayers; i++)
        {
            int numberOfNeuronsForLayer = layerSizes[i + 1];
            int numberOfInputsForLayer = layerSizes[i];
            layers[i] = new Layer(numberOfNeuronsForLayer, numberOfInputsForLayer);
        }
    }

    public float[] GetQValues()
    {
        float[] currentOutput = currentState.createInputVector();
        for (int i = 0; i < layers.Length; i++)
        {
            bool isOutputLayer = i == layers.Length - 1;
            currentOutput = layers[i].GetOutputs(currentOutput, isOutputLayer);
        }
        return currentOutput;
    }

    public int FindBestActionIndex(float[] QValues)
    {
        int currentBestActionIndex = 0;
        for (int i = 0; i < QValues.Length; i++)
        {
            if (QValues[i] > QValues[currentBestActionIndex]) currentBestActionIndex = i;
        }
        return currentBestActionIndex;
    }
    public void RandomlyAdjustAllWeightsAndBiases(float weightAdjustmentMultiplier)
    {
        foreach(Layer layer in layers)
        {
            layer.RandomlyAdjustNeuronWeightsAndBiases(weightAdjustmentMultiplier);
        }
    }

    public int GetChosenActionIndex()
    {
        return FindBestActionIndex(GetQValues());
    }
}

public class Layer
{
    private Neuron[] neurons;

    public Layer(int numOfNeurons, int numberOfInputs)
    {
        neurons = new Neuron[numOfNeurons];
        for (int i = 0; i < numOfNeurons; i++)
        {
            neurons[i] = new Neuron(numberOfInputs);
        }
    }

    public float[] GetOutputs(float[] inputs, bool isOutputLayer)
    {
        float[] outputs = new float[neurons.Length];
        for (int i = 0; i < outputs.Length; i++)
        {
            outputs[i] = neurons[i].ComputeOutput(inputs, isOutputLayer);
        }
        return outputs;
    }

    public void RandomlyAdjustNeuronWeightsAndBiases(float weightAdjustmentMultiplier)
    {
        foreach(Neuron neuron in neurons)
        {
            neuron.RandomlyAdjustWeightsAndBias(weightAdjustmentMultiplier);
        }
    }
}

public class Neuron
{

    private float[] weights;
    private float bias;
    public Neuron(int numberOfInputs)
    {
        weights = WeightInitializer.GetRandomWeights(numberOfInputs);
        bias = RandomFloatInRange(0f, 1f);
    }
    public float RandomFloatInRange(float min, float max)
    {
        System.Random rand = new System.Random();
        return (float)(min + rand.NextDouble() * (max - min));
    }

    public float ComputeOutput(float[] inputs, bool isOutputLayer)
    {
        float sum = 0;
        for (int i = 0; i < inputs.Length; i++)
        {
            sum += inputs[i] * weights[i];
        }
        sum += bias;
        return isOutputLayer ? sum : ActivationFunction(sum);
    }


    private float ActivationFunction(float value)
    {
        float negativeGradient = 0.01f;
        return value > 0 ? value : value * negativeGradient;
    }

    public void RandomlyAdjustWeightsAndBias(float weightAdjustmentMultiplier)
    {
        for(int i = 0; i < weights.Length; i++)
        {
            weights[i] += RandomFloatInRange(-1, 1) * weightAdjustmentMultiplier  * RandomFloatInRange(0.5f, 2f);;
        }
    }
}
