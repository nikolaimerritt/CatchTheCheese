using System;
using System.Collections.Generic;
using System.Linq;

namespace CatchTheCheese
{
    public enum CellType
    {
        Empty, Block, Enemy, Player, Goal
    }
    public record Cell
    {
        public readonly CellType CellType;
        public static int NumCellTypes => _cellTypes.Count;

        private static readonly Dictionary<CellType, string> _cellTypeToString = new ()
        {
            [CellType.Empty]  = "=",
            [CellType.Block]  = "&",
            [CellType.Player] = "*",
            [CellType.Enemy]  = "O",
            [CellType.Goal]   = "C"
        };

        private static readonly List<CellType> _cellTypes = _cellTypeToString.Keys.ToList();

        public Cell(CellType cellType) 
            => CellType = cellType;

        public Cell(string str) 
            => CellType = _cellTypeToString.FirstOrDefault(pair => pair.Value == str).Key;

        public override string ToString()
            => _cellTypeToString[CellType];
    }
}
