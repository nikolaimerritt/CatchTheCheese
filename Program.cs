using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using NeuralNetLearning;
using NeuralNetLearning.Maths;
using DeepLearning;

namespace CatchTheCheese
{
    using Vector = Vector<double>;
    using TrainingPair = Tuple<Vector<double>, Vector<double>>;
    class Program
    {
        static Move ReadMove()
        => Console.ReadKey().Key switch
            {
                ConsoleKey.W or ConsoleKey.UpArrow => Move.Up,
                ConsoleKey.A or ConsoleKey.LeftArrow => Move.Backwards,
                ConsoleKey.S or ConsoleKey.DownArrow => Move.Down,
                ConsoleKey.D or ConsoleKey.RightArrow => Move.Forwards,
                _ => Move.Forwards
            };

        static void UserPlays()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            MouseMaze game = GameStateFactory.ReadFromCSV("../../../level2.csv");
            Console.WriteLine(game);
            for (int gameNum = 1; gameNum <= 3; gameNum++)
            {
                while (!game.IsTerminal())
                {
                    game.MakeMove(ReadMove());
                    Console.Clear();
                    Console.WriteLine(game);
                }
                Console.WriteLine(game.Reward());
                game.Reset();
            }
        }

        static TrainingPair[] GetTrainingPairs(int count)
        {
            TrainingPair[] pairs = new TrainingPair[count];
            for (int i = 0; i < count; i++)
            {
                Vector input = 50 * VectorFunctions.StdUniform(dim: 1);
                Vector desiredOutput = input.PointwisePower(2);
                pairs[i] = new (input, desiredOutput);
            }
            return pairs;
        }

        static NeuralNet GetNet()
        {
            List<NeuralLayerConfig> layers = new()
            {
                new InputLayer(1),
                new HiddenLayer(8, new ReluActivation()),
                new HiddenLayer(8, new ReluActivation()),
                new OutputLayer(1, new IdentityActivation())
            };
            var cost = new HuberCost();
            var descender = new AdamGradientDescender();
            var sampleInputs = Enumerable.Range(0, 100)
                .Select(_ => 50 * VectorFunctions.StdUniform(dim: 1));
            return NeuralNetFactory.RandomCustomisedForMiniBatch(layers, sampleInputs, descender, cost);
        }

        static void LearnSquare()
        {
            NeuralNet net = GetNet();
            net.GradientDescentParallel(GetTrainingPairs(10000), batchSize: 256, numEpochs: 15000);

            Console.WriteLine("Finished learning! Do you want to see how well it does?");
            Console.ReadKey();

            foreach ((Vector input, Vector desiredOutput) in GetTrainingPairs(10))
                Console.WriteLine($"{input[0]:0.0000} \t --> \t {net.Output(input)[0]:0.0000} \t (actual: {desiredOutput[0]:0.0000}");
        }

        static void Main()
        {
            QLearner<MouseMaze, Move> learner = new(GameStateFactory.ReadFromCSV("../../../level2.csv"));
            Console.WriteLine("Started learning");
            learner.Learn(totalGames: 1000);
            learner.WriteToDirectory("../../../learner");
            
            Console.WriteLine("\n\nFinished learning!! Do you want to see a demo?");
            Console.ReadKey();
            learner.ShowDemo();
        }
    }
}