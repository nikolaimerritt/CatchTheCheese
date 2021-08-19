using MathNet.Numerics.LinearAlgebra;
using DeepLearning;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace CatchTheCheese
{
    public class Move : IMove
    {
        public int LayerSize => _allMoveTypes.Count;
        public Vector<double> ToLayer()
        {
            Vector<double> layer = Vector<double>.Build.Dense(length: LayerSize, value: 0);
            layer[_allMoveTypes.FindIndex(mv => mv == _moveType)] = 1;
            return layer;
        }

        public static readonly Move Forwards = new(MoveType.Forwards);
        public static readonly Move Backwards = new(MoveType.Backwards);
        public static readonly Move Up = new(MoveType.Up);
        public static readonly Move Down = new(MoveType.Down);

        public static readonly ReadOnlyCollection<Move> AllPossibleMoves = new List<Move>()
        { Forwards, Backwards, Up, Down}.AsReadOnly();

        private enum MoveType
        {
            Forwards,
            Backwards,
            Up,
            Down
        }
        private static readonly List<MoveType> _allMoveTypes = new() { MoveType.Forwards, MoveType.Backwards, MoveType.Up, MoveType.Down };
        private readonly MoveType _moveType;
        private Move(MoveType moveType)
            => _moveType = moveType;

        
    }
}