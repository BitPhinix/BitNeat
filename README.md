# What is BitNeat ?
A lightweight and beginner friendly implementation of the neat algorithm

# Usage

```C#
//Create a manager with all necessary properties
//The Evaluate function has to a function that returns the fitness of the current network
//The BaseGenome, Mutator can be created outside the obj initializer and be customized
//to your needs.
var manager = new Manager
{
  EvaluationFunction = Evaluate,
  BaseGenome = Genome.Generate(1, 1, true),
  Mutator = new Mutator(),
};

//Setup ManagerOnOnLifecycleFinishedEvent 
//This event is called a lifecycle is finished
manager.LifecycleFinishedEvent += OnLifecycleFinishedEvent;

//This method evolves the network untill the given fitness is reached
//and returns the network that has reached the given fittnes
var network = manager.TrainUntil(0.98);

//After the process is finished you can use the network.
//The network can also be saved and restored using the 
//toString and fromString method.
while (true)
{
  Console.Write("Input: ");
  var input = Convert.ToDouble(Console.ReadLine());

  //Calculate and print out the result
  Console.WriteLine("Output: " + String.Join(", ", network.Calculate(new[] { input })) + "\n");
}
```

Or take a look at the sample project [here](https://github.com/BitPhinix/BitNeat/blob/master/BitNeat/SampleApplication/Program.cs).

# How it works

The network is evolved by a slightly modified version of the [NEAT algorithm](https://de.wikipedia.org/wiki/NeuroEvolution_of_Augmented_Topologies).

If you want to learn more about it you can read the official [PDF](http://nn.cs.utexas.edu/downloads/papers/stanley.ec02.pdf) or watch this youtube video:


<a href="http://www.youtube.com/watch?feature=player_embedded&v=H4WnRLEG73Q
" target="_blank"><img src="http://img.youtube.com/vi/H4WnRLEG73Q/0.jpg" 
alt="NEAT FlapPyBi/o" width="240" height="180" border="10" /></a>
