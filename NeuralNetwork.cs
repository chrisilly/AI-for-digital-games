using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AI_for_digital_games
{
    internal class NeuralNetwork
    {
        int layers;
        int neuronsPerLayer;

        public NeuralNetwork()
        {

        }

        // public void Propagate() {}
        // public void BackPropagate(){}
        // public void AdjustWeights(){}

        internal class Neuron
        {
            public List<Dendrite> Dendrites { get; set; }
            public Signal Output { get; set; }
            double weight;

            public Neuron()
            {
                Dendrites = new List<Dendrite>();
                Output = new Signal();
            }

            public void Fire()
            {
                Output.Value = Sum();
                Output.Value = Activation(Output.Value);
            }

            public void Compute(double learningRate, double delta)
            {
                weight += learningRate * delta;
                foreach(var dendrite in Dendrites)
                {
                    dendrite.Weight = weight;
                }
            }

            /// <summary>
            /// Sums up all incoming Signals by taking all the Values from the Dendrites and multiplying them with their assigned Weights
            /// </summary>
            /// <returns></returns>
            double Sum()
            {
                double sum = 0.0f;
                foreach(var incoming in Dendrites)
                {
                    sum += incoming.Input.Value * incoming.Weight;
                }

                // we could add bias here

                return sum;
            }

            double Activation(double input)
            {
                double threshold = 1;
                return input >= threshold ? 0 : threshold;
            }
        }

        internal class Signal
        {
            public double Value { get; set; }
        }

        internal class Dendrite
        {
            public Signal Input { get; set; }
            public double Weight { get; set; }
            public bool Learnable { get; set; } = true;
        }

        internal class NeuralLayer
        {
            public List<Neuron> Neurons { get; set; }
            public string Name { get; set; }
            public double Weight { get; set; }

            public NeuralLayer(int count, double initialWeight, string name = "")
            {
                Neurons = new List<Neuron>();
                for (int i = 0; i < count; i++)
                {
                    Neurons.Add(new Neuron());
                }

                Weight = initialWeight;
                Name = name;
            }

            public void Compute(double learningRate, double delta)
            {
                foreach(var neuron in Neurons)
                    neuron.Compute(learningRate, delta);
            }

            public void Log()
            {
                Console.WriteLine("{0}, Weight: {1}", Name, Weight);
            }

            public int Count() { return Neurons.Count; }
            // public int operator [](int index) { return Neurons[index] };
            public Neuron this[int index]
            {
                get { return Neurons[index]; }
                set { Neurons[index] = value; }
            }
        }
    }

    internal class BabyNeuralNetwork
    {
        private const int INPUT_NODES = 3;
        private const int HIDDEN_NODES = 4;
        private const int OUTPUT_NODES = 3;

        private double[] inputLayer;
        private double[] hiddenLayer;
        private double[] outputLayer;
        private double[,] weightsIH;
        private double[,] weightsHO;

        public void InitializeNetwork()
        {
            inputLayer = new double[INPUT_NODES];
            hiddenLayer = new double[HIDDEN_NODES];
            outputLayer = new double[OUTPUT_NODES];

            weightsIH = new double[INPUT_NODES, HIDDEN_NODES];
            weightsHO = new double[HIDDEN_NODES, OUTPUT_NODES];

            // Initialize weights randomly
            Random rand = new Random();
            for (int i = 0; i < INPUT_NODES; i++)
                for (int j = 0; j < HIDDEN_NODES; j++)
                    weightsIH[i, j] = (rand.NextDouble() * 2) - 1;

            for (int i = 0; i < HIDDEN_NODES; i++)
                for (int j = 0; j < OUTPUT_NODES; j++)
                    weightsHO[i, j] = (rand.NextDouble() * 2) - 1;
        }

        public double[] ComputeOutputs(double[] inputs)
        {
            if (inputs.Length != INPUT_NODES)
                throw new ArgumentException("Invalid number of inputs");

            // Copy inputs to input layer
            Array.Copy(inputs, inputLayer, INPUT_NODES);

            // Forward propagation
            for (int i = 0; i < HIDDEN_NODES; i++)
            {
                double sum = 0;
                for (int j = 0; j < INPUT_NODES; j++)
                {
                    sum += inputLayer[j] * weightsIH[j, i];
                }
                hiddenLayer[i] = Sigmoid(sum);
            }

            // Compute output layer
            for (int i = 0; i < OUTPUT_NODES; i++)
            {
                double sum = 0;
                for (int j = 0; j < HIDDEN_NODES; j++)
                {
                    sum += hiddenLayer[j] * weightsHO[j, i];
                }
                outputLayer[i] = Sigmoid(sum);
            }

            return outputLayer;
        }

        private double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }
    }
}