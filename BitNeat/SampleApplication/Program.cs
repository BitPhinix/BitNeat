using System;
using BitNeat;
using BitNeat.DataClasses;

namespace SampleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new Manager
            {
                EvaluationFuntion = Evaluate,
                BaseGenome = Genome.Generate(1, 1, true),
                Mutator = new Mutator(),
            };

            manager.OnLifecycleFinishedEvent += ManagerOnOnLifecycleFinishedEvent;
            var network = manager.TrainUntil(0.98);

            Console.WriteLine("\nTraining Finished !\nTest:\n");

            while (true)
            {
                Console.Write("Input: ");
                var input = Convert.ToDouble(Console.ReadLine());
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
                avg += Math.Abs(Math.Cos(i) - network.Calculate(new [] { i })[0]);
 
            return 1 - avg / 100;
        }
    }
}
