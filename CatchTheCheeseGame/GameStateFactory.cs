using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace CatchTheCheese
{
    public static class GameStateFactory
    {
        public static GameState ReadFromCSV(string filepath)
        {
            List<Cell[]> rows = new();
            using TextFieldParser parser = new(filepath);
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
                rows.Add(parser.ReadFields().Select(field => new Cell(field)).ToArray());

            return new GameState(JaggedToGrid(rows), null);
        }

        private static T[,] JaggedToGrid<T> (IList<T[]> jagged)
        {
            T[,] grid = new T[jagged.Count, jagged[0].Length];
            for (int r = 0; r < jagged.Count; r++)
            {
                for (int c = 0; c < jagged[0].Length; c++)
                {
                    grid[r, c] = jagged[r][c];
                }
            }
            return grid;
        }

    }
}
