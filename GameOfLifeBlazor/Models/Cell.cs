namespace GameOfLifeBlazor.Models;

public enum CellState
{
    On,
    Off,
    Wall
}

public class Cell(CellState initialState)
{
    public HashSet<Cell> Neighbours { get; } = new();
    public CellState CurrentState { get; set; } = initialState;
    private CellState _nextState;

    public void Toggle()
    {
        CurrentState = CurrentState switch
        {
            CellState.On => CellState.Off,
            CellState.Off => CellState.On,
            _ => CurrentState
        };
    }
    
    public void GetNextState()
    {
        var activeNeighbours = Neighbours.Count(cell => cell.CurrentState is CellState.On);

        _nextState = (CurrentState, activeNeighbours) switch
        {
            // Any live cell with fewer than two live neighbours dies, as if by underpopulation.
            (CellState.On, < 2) => CellState.Off,

            // Any live cell with two or three live neighbours lives on to the next generation.
            (CellState.On, 2 or 3) => CellState.On,

            // Any live cell with more than three live neighbours dies, as if by overpopulation.
            (CellState.On, _) => CellState.Off,

            // Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction
            (CellState.Off, 3) => CellState.On,

            _ => CurrentState
        };
    }

    public void MoveToNextState()
    {
        CurrentState = _nextState;
    }

    public void ToggleWall()
    {
        CurrentState = CurrentState is CellState.Wall ? CellState.Off : CellState.Wall;
    }
}