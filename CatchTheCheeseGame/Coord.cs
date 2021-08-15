using System;
using System.Collections.Generic;
using System.Text;

namespace CatchTheCheese
{
    public record Coord
    {
        public int X;
        public int Y;

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Coord operator +(Coord first, Coord second)
            => new (first.X + second.X, first.Y + second.Y);

        public override string ToString()
            => $"({X}, {Y})";

        public static Coord operator -(Coord first, Coord second)
            => new (first.X - second.X, first.Y - second.Y);

        public double Norm()
            => Math.Sqrt(X * X + Y * Y);

        public static double Distance(Coord first, Coord second)
            => (second - first).Norm();
    }
}
