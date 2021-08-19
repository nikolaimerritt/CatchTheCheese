using System;
using System.Collections;
using DeepLearning;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace CatchTheCheese
{
    public static class GameStateFactory
    {
        public static MouseMaze ReadFromCSV(string filepath)
        {
            List<Cell[]> rows = new();
            using TextFieldParser parser = new(filepath);
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                Cell[] cells = fields.Select(field => new Cell(field)).ToArray();
                rows.Add(cells);
            }

            Cell[,] grid = JaggedToGrid(rows);
            return new MouseMaze(grid);
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
