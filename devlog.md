# Neural Network

What is to be decided?
- Flee
- Chase
- Patrol

Because these outcomes are very simple and understanding when to do what is trivial, a neural network is really overkill and will likely perform worse than our other brains. What might make the neural network more impactful is if we add more precise decision making for how fast or slow to chase/flee/patrol and how hard to steer (and in which direction?).

An alternative that should seem relatively simple would be to have the neural network simply decide which direction it should go.

## 1. Training
- Genetic algorithm?

## 2. Running
1. send in variables
    - have one input node for each variable
2. Let it do its calculations and things
3. have `NeuralNetworkBrain` read and choose a behaviour associated with the highest value output node