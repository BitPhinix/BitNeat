using System;
using BitNeat;

namespace SampleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create new Manager
            var manager = new Manager
            {
                EvaluationFunction = Evaluate,
                BaseGenome = Genome.Generate(1, 1, true),
                Mutator = new Mutator(),
            };

            //Setup ManagerOnOnLifecycleFinishedEvent 
            manager.LifecycleFinishedEvent += ManagerOnOnLifecycleFinishedEvent;

            //Train network
            var network = manager.TrainUntil(0.98);

            Console.WriteLine("\nTraining Finished !\nTest:\n");

            while (true)
            {
                Console.Write("Input: ");
                var input = Convert.ToDouble(Console.ReadLine());

                //Calculate and print out the result
                Console.WriteLine("Output: " + String.Join(", ", network.Calculate(new[] { input })) + "\n");
            }
        }

        private static void ManagerOnOnLifecycleFinishedEvent(Population currentPopulation)
        {
            Console.WriteLine($"Highest fitness: {currentPopulation.MaxFitness}");
        }

        private static double Evaluate(Network network)
        {
            var avg = 0.0;

            for (var i = 0d; i < 1; i += 0.01)
                //Calculate the networks output -> Get the difference to the value it shoud be -> Add the abs value of the difference to avg 
                avg += Math.Abs(Math.Cos(i) - network.Calculate(new [] { i })[0]);
 
            //Convert avg to fitness
            return 1 - avg / 100;
        }
    }
}
