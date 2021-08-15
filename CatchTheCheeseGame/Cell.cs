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
        public CellType CellType { get; private set; }
        public static int NumCellTypes => _cellTypes.Count;

        private static readonly Dictionary<CellType, string> _cellTypeToString = new ()
        {
            [CellType.Empty]  = "⬛",
            [CellType.Block]  = "🧱",
            [CellType.Player] = "🐁",
            [CellType.Enemy]  = "💀",
            [CellType.Goal]   = "🧀"
        };

        private static readonly List<CellType> _cellTypes = _cellTypeToString.Keys.ToList();

        public Cell(CellType cellType) 
            => CellType = cellType;

        public Cell(string str) 
            => CellType = _cellTypeToString.FirstOrDefault(pair => pair.Value == str).Key;

        public override string ToString() 
            => _cellTypeToString[CellType];

        public List<double> OneHotEncode()
        {
            List<double> encoding = new(NumCellTypes);
            for (int i = 0; i < NumCellTypes; i++)
            {
                encoding.Add(_cellTypes[i] == CellType ? 1 : 0);
            }
            return encoding;
        }
    }
}
