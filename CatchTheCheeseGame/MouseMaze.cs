using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using DeepLearning;
using Microsoft.VisualBasic.FileIO;
using MathNet.Numerics.LinearAlgebra;

namespace CatchTheCheese
{
    using Vector = Vector<double>;
    public class MouseMaze : IEnvironment<Move>
    {
        public int LayerSize => _grid.Length;

        private Cell[,] _grid;
        private readonly Cell[,] _initialGrid;
        private readonly Coord _initialPlayerCoord;
        private readonly ReadOnlyCollection<Coord> _enemyCoords;
        private readonly ReadOnlyCollection<Coord> _goalCoords;
        private Coord _playerCoord;
        private Move _prevMove;

        private static readonly Dictionary<Move, Coord> _moveDirection = new()
        {
            [Move.Forwards] = new Coord(1, 0),
            [Move.Backwards] = new Coord(-1, 0),
            [Move.Up] = new Coord(0, -1),
            [Move.Down] = new Coord(0, 1)
        };

        public MouseMaze(Cell[,] initialGrid)
        {
            _initialGrid = initialGrid;
            _grid = GridCopy(_initialGrid);
            _initialPlayerCoord = FindFirst(CellType.Player);
            _playerCoord = _initialPlayerCoord;
            _enemyCoords = FindAll(CellType.Enemy).AsReadOnly();
            _goalCoords = FindAll(CellType.Goal).AsReadOnly();
            _prevMove = null;
        }

        public void MakeMove(Move move)
        {
            if (!IsValidMove(move))
                throw new ArgumentException($"Action {move} is invalid");

            _grid[_playerCoord.Y, _playerCoord.X] = new Cell(CellType.Empty);
            _playerCoord += _moveDirection[move];
            _grid[_playerCoord.Y, _playerCoord.X] = new Cell(CellType.Player);
            _prevMove = move;
        }

        private double WinLossReward() // [-1, 1]
        {
            if (_enemyCoords.Contains(_playerCoord))
                return -1;
            if (_goalCoords.Contains(_playerCoord))
                return 1;
            return -0.1;
        }

        private double DistFromCheeseReward() // [-1, 1]
        {
            double distFromCheese = Coord.Distance(_playerCoord, FindFirst(CellType.Goal));
            double maxDistance = Coord.Distance(new Coord(0, 0), new Coord(_grid.GetLength(1), _grid.GetLength(0)));
            return 1 - 2 * distFromCheese / maxDistance;
        }

        public Vector ToLayer()
        {
            Vector layer = Vector.Build.Dense(length: _grid.Length, value: 0);
            int playerIdx = _grid.GetLength(1) * _playerCoord.Y + _playerCoord.X;
            layer[playerIdx] = 1;
            return layer;
        }

        public bool IsTerminal()
            => _enemyCoords.Contains(_playerCoord) || _goalCoords.Contains(_playerCoord);

        public bool IsValidMove(Move move)
        {
            if (IsTerminal())
                return false;

            if (_prevMove != null && MovesAreInverses(move, _prevMove))
                return false;

            Coord newCoord = _playerCoord + _moveDirection[move];
            if (!CoordIsInGrid(newCoord))
                return false;

            return _grid[newCoord.Y, newCoord.X].CellType != CellType.Block;
        }

        public List<Move> ValidMoves()
            => Move.AllPossibleMoves.Where(IsValidMove).ToList();

        public void Reset()
        {
            _grid = GridCopy(_initialGrid);
            _playerCoord = _initialPlayerCoord;
            _prevMove = null;
        }

        public double Reward()
            => WinLossReward();


        public override string ToString()
        {
            string header = new ('#', _grid.GetLength(1) + 2);
            StringBuilder builder = new(header);
            builder.AppendLine();

            for (int r = 0; r < _grid.GetLength(0); r++)
            {
                builder.Append('#');
                for (int c = 0; c < _grid.GetLength(1); c++)
                {
                    builder.Append(_grid[r, c].ToString());
                }
                builder.Append('#');
                builder.AppendLine();
            }
            builder.Append(header);
            return builder.ToString();
        }

        
        private List<Coord> FindAll(CellType toFind)
        {
            List<Coord> coords = new();
            for (int r = 0; r < _grid.GetLength(0); r++)
            {
                for (int c = 0; c < _grid.GetLength(1); c++)
                {
                    if (_grid[r, c].CellType == toFind)
                        coords.Add(new Coord(c, r));
                }
            }
            return coords;
        }

        private Coord FindFirst(CellType toFind)
        {
            for (int r = 0; r < _grid.GetLength(0); r++)
            {
                for (int c = 0; c < _grid.GetLength(1); c++)
                {
                    if (_grid[r, c].CellType == toFind)
                        return new Coord(c, r);
                }
            }
            throw new ArgumentException($"Could not find the cell of type {toFind} in the grid");
        }

        private bool CoordIsInGrid(int x, int y)
            => y >= 0 && y < _grid.GetLength(0)
            && x >= 0 && x < _grid.GetLength(1);

        private bool CoordIsInGrid(Coord coord)
            => CoordIsInGrid(coord.X, coord.Y);

        private static Cell[,] GridCopy(Cell[,] grid)
        {
            Cell[,] copy = new Cell[grid.GetLength(0), grid.GetLength(1)];
            for (int r = 0; r < grid.GetLength(0); r++)
            {
                for (int c = 0; c < grid.GetLength(1); c++)
                {
                    copy[r, c] = grid[r, c];
                }
            }
            return copy;
        }

        private static bool MovesAreInverses(Move move, Move other)
        {
            if (move == Move.Forwards)
                return other == Move.Backwards;
            if (move == Move.Backwards)
                return other == Move.Forwards;
            if (move == Move.Up)
                return other == Move.Down;
            if (move == Move.Down)
                return other == Move.Up;
            throw new ArgumentException($"Could not recoginse the move {move}");
        }
    }
}   
