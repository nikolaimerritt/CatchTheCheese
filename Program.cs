using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using DeepLearning;
using NeuralNetLearning;

namespace CatchTheCheese
{
    using Vector = Vector<double>;
    using Matrix = Matrix<double>;
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
            IState<Move> game = GameStateFactory.ReadFromCSV("../../../level.csv");
            Console.WriteLine(game);
            while (!game.IsTerminalState())
            {
                game = game.AfterAction(ReadMove());
                Console.Clear();
                Console.WriteLine(game);
            }
        }

        static void Main()
        {
            QLearner<Move> learner = new(initialState: GameStateFactory.ReadFromCSV("../../../level.csv"));
            Console.WriteLine("Started learning");
            learner.Learn(numGames: 10000);
            Console.WriteLine("\n\nFinished learning!! Do you want to see a demo?");
            Console.ReadKey();
            learner.ShowDemo();
        }
    }
}
