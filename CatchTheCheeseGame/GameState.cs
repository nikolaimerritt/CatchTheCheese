using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DeepLearning;
using Microsoft.VisualBasic.FileIO;
using MathNet.Numerics.LinearAlgebra;

namespace CatchTheCheese
{
    using Vector = Vector<double>;
    public class GameState : IState<Move>
    {
        public int InputLayerSize => Cell.NumCellTypes * _grid.Length;
        public List<Move> AllPossibleActions => new()
        {
            Move.Forwards,
            Move.Backwards,
            Move.Up,
            Move.Down
        };

        private readonly Cell[,] _grid;
        private readonly Coord _playerCoord;
        private readonly Move? _prevMove;

        private static readonly Dictionary<Move, Coord> _moveDirection = new()
        {
            [Move.Forwards] = new Coord(1, 0),
            [Move.Backwards] = new Coord(-1, 0),
            [Move.Up] = new Coord(0, -1),
            [Move.Down] = new Coord(0, 1)
        };

        public GameState(Cell[,] grid, Move? prevMove)
        {
            _grid = grid;
            _prevMove = prevMove;
            _playerCoord = Find(_grid, CellType.Player);
        }

        public GameState AfterAction(Move move)
        {
            if (!IsValidAction(move))
                throw new ArgumentException($"Action {move} is invalid");

            Coord newCoord = _playerCoord + _moveDirection[move];
            Cell[,] newGrid = Copy(_grid);

            newGrid[_playerCoord.Y, _playerCoord.X] = new Cell(CellType.Empty);
            newGrid[newCoord.Y, newCoord.X] = new Cell(CellType.Player);
            return new GameState(newGrid, move);
        }

        IState<Move> IState<Move>.AfterAction(Move action)
            => AfterAction(action);

        public bool IsTerminalState()
          => CellsPlayerIsTouching(_grid, _playerCoord)
            .Any(cell => cell.CellType == CellType.Enemy || cell.CellType == CellType.Goal);
       

        private double WinLossReward() // [-1, 1]
        {
            List<Cell> cellsTouching = CellsPlayerIsTouching(_grid, _playerCoord);
            if (cellsTouching.Any(c => c.CellType == CellType.Enemy))
                return -1;
            if (cellsTouching.Any(c => c.CellType == CellType.Goal))
                return 1;
            return 0;
        }

        private double DistFromCheeseReward() // [-1, 1]
        {
            double distFromCheese = Coord.Distance(_playerCoord, Find(_grid, CellType.Goal));
            double maxDistance = Coord.Distance(new Coord(0, 0), new Coord(_grid.GetLength(0), _grid.GetLength(1)));
            return 0.5 - distFromCheese / maxDistance;
        }

        public double Reward() // [-1, 1]
            => (2/3) * WinLossReward() + (1/3) * DistFromCheeseReward();


        public Vector ToInputLayer()
        {
            List<double> input = new(InputLayerSize);
            foreach (Cell cell in _grid)
            {
                List<double> encoding = cell.OneHotEncode();
                input.AddRange(encoding);
            }
            return Vector.Build.DenseOfEnumerable(input);
        }

        public bool IsValidAction(Move action)
        {
            if (IsTerminalState())
                return false;

            if (_prevMove.HasValue && ActionsAreInverses(action, _prevMove.Value))
                return false;

            Coord newCoord = _playerCoord + _moveDirection[action];
            if (!IsValidCoord(_grid, newCoord))
                return false;

            Cell newCell = _grid[newCoord.Y, newCoord.X];
            return newCell.CellType == CellType.Empty;
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            for (int r = 0; r < _grid.GetLength(0); r++)
            {
                for (int c = 0; c < _grid.GetLength(1); c++)
                {
                    builder.Append(_grid[r, c].ToString() + " ");
                }
                builder.AppendLine();
            }
            return builder.ToString();
        }

        private static Coord Find(Cell[,] grid, CellType toFind)
        {
            for (int r = 0; r < grid.GetLength(0); r++)
            {
                for (int c = 0; c < grid.GetLength(1); c++)
                {
                    if (grid[r, c].CellType == toFind)
                        return new Coord(c, r); // (0, 0) corresponds to the top-left of the grid
                }
            }
            throw new ArgumentException($"Could not find the player in the grid");
        }

        private static Cell[,] Copy(Cell[,] grid)
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

        private static List<Cell> CellsPlayerIsTouching(Cell[,] grid, Coord player)
        {
            List<Cell> cells = new();
            for (int r = player.Y - 1; r <= player.Y + 1; r++)
            {
                for (int c = player.X - 1; c <= player.X + 1; c++)
                {
                    if (IsValidCoord(grid, new Coord(c, r)) && !(r == player.Y && c == player.X))
                    {
                        cells.Add(grid[r, c]);
                    }
                }
            }
            return cells;
        }

        private static bool IsValidCoord(Cell[,] grid, Coord coord)
            => coord.Y >= 0 && coord.Y < grid.GetLength(0) && coord.X >= 0 && coord.X < grid.GetLength(1);

        private static bool ActionsAreInverses(Move move, Move other)
            => move switch
            {
                Move.Forwards => other == Move.Backwards,
                Move.Down => other == Move.Up,
                Move.Backwards => other == Move.Forwards,
                Move.Up => other == Move.Down,
                _ => false
            };
    }
}   
