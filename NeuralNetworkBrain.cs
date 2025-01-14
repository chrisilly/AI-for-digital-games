using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AI_for_digital_games
{
    internal class NeuralNetworkBrain : IBehaviourSystem
    {
        BabyNeuralNetwork babyNeuralNetwork;
        // Inputs
        // What does this brain see?
            // Agents in the surroundings
                // Distance to each agent
                // Relative size to each agent
                // Speed and/or direction of each agent?


        public NeuralNetworkBrain()
        {
            babyNeuralNetwork = new BabyNeuralNetwork();
            babyNeuralNetwork.InitializeNetwork();
        }
        
        public void Update(AgentHandler handler, Agent body, GameTime gameTime)
        {
            body.Move(gameTime);

            // Which variables to have as input nodes?
                // 1. Distance to nearest agent
                // 2. direction.x point to nearest agent
                // 3. direction.y to nearest agent
            double[] inputs = { 0.1, 0.2, 0.3 };
            double[] outputs = babyNeuralNetwork.ComputeOutputs(inputs);

            double flee = outputs[0];
            double chase = outputs[1];
            double patrol = outputs[2];

            double winningOutput = outputs.Max();

            // if(winningOutput < 0.1) { body.Idle(); }
            if(winningOutput == flee) { handler.FleeDanger(body); }
            else if(winningOutput == chase) { handler.ChaseFood(body); }
            else if(winningOutput == patrol) { body.Patrol(); }
            
            // Console.WriteLine("Outputs: ");
            // foreach (var output in outputs)
            // {
            //     Console.WriteLine(output + " ");
            // }
        }
    }
}