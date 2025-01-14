using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AI_for_digital_games
{
    internal class NeuralNetworkBrain : IBehaviourSystem
    {
        BabyNeuralNetwork babyNeuralNetwork;

        double distanceToNearestAgent = 0;
        double[] inputs = { 0.1, 0.2, 0.3 };
        double[] outputs;

        // Predefined behaviours to choose from
        double flee;
        double chase;
        double patrol;

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
            distanceToNearestAgent = handler.GetDistanceToNearestAgent(body);
                // 2. direction.x point to nearest agent
                // 3. direction.y to nearest agent

            inputs[0] = distanceToNearestAgent;
            // inputs[1] = ;
            // inputs[2] = ;

            UpdateOutputs();

            TriggerOutput(handler, body);
            
            // Console.WriteLine("Outputs: ");
            // foreach (var output in outputs)
            // {
            //     Console.WriteLine(output + " ");
            // }
        }

        void UpdateOutputs()
        {
            outputs = babyNeuralNetwork.ComputeOutputs(inputs);
            flee = outputs[0];
            chase = outputs[1];
            patrol = outputs[2];
        }

        void TriggerOutput(AgentHandler handler, Agent body)
        {
            double winningOutput = outputs.Max();

            // if(winningOutput < 0.1) { body.Idle(); }
            if(winningOutput == flee) { handler.FleeDanger(body); }
            else if(winningOutput == chase) { handler.ChaseFood(body); }
            else if(winningOutput == patrol) { body.Patrol(); }
        }
    }
}