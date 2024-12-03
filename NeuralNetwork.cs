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

        // public void Propogate() {}
        // public void BackPropogate(){}
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
        }
    }
}