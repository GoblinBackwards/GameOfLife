using System.Globalization;
using System.Text;

namespace GameOfLifeBlazor.Models;

public class Board {
    public Cell[,] Cells { get; }

    public delegate CellState CellInitializer(int cellX, int cellY);

    public Board(int width, int height, CellInitializer cellInitializer)
    {
        Cells = new Cell[width, height];

        for (var w = 0; w < Cells.GetLength(0); w++)
        {
            for (var h = 0; h < Cells.GetLength(1); h++)
            {
                Cells[w, h] = new Cell(cellInitializer(w, h));
            }
        }
        
        RegisterCellNeighbours();
    }

    private void RegisterCellNeighbours()
    {
        for (var w = 0; w < Cells.GetLength(0); w++)
        {
            for (var h = 0; h < Cells.GetLength(1); h++)
            {
                var directions = new[] {(1,1),(1,0),(1,-1),(0,1),(0,-1),(-1,1),(-1,0),(-1,-1)};
                foreach (var (x, y) in directions)
                {
                    var checkX = w + x;
                    var checkY = h + y;
                    
                    if (checkX < 0 || checkX >= Cells.GetLength(0))
                        continue;
                    if (checkY < 0 || checkY >= Cells.GetLength(1))
                        continue;
                    
                    Cells[w,h].Neighbours.Add(Cells[checkX,checkY]);
                }
            }
        }
    }

    public void Process()
    {
        foreach (var cell in Cells)
        {
            cell.GetNextState();
        }

        foreach (var cell in Cells)
        {
            cell.MoveToNextState();
        }
    }

    public Board Clone()
    {
        return new Board(Cells.GetLength(0), Cells.GetLength(1), CopyFn);

        CellState CopyFn(int x, int y) => Cells[x, y].CurrentState;
    }

    public string Serialize()
    {
        var sb = new StringBuilder();

        for (var y = 0; y < Cells.GetLength(1); y++)
        {
            for (var x = 0; x < Cells.GetLength(0); x++)
            {
                sb.Append((int)Cells[x, y].CurrentState);
            }

            sb.Append(',');
        }

        return sb.ToString().TrimEnd(',');
    }

    public static bool TryDeserialize(string str, out Board? result)
    {
        try
        {
            var states = str.Split(',')
                .Select(s => s.Select(c =>
                {
                    return c switch
                    {
                        '0' => CellState.Off,
                        '1' => CellState.On,
                        '2' => CellState.Wall,
                        _ => throw new FormatException()
                    };
                }).ToArray()).ToArray();
            
            result = new Board(states[0].Length, states.Length,
                (x, y) => states[y][x]);
        }
        catch
        {
            result = null;
            return false;
        }
        
        return true;
    }
}
