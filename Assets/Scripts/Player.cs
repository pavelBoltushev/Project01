using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private CellGrid _grid;
    [SerializeField] private int _initialTerritoryRadius;

    private CharacterController _characterController;    
    private Stack<Cell> _path;
    private bool _hasInitialTerritory;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();        
        _path = new Stack<Cell>();
        _hasInitialTerritory = false;
    }

    private void Update()
    {
        Vector3 playerInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        _characterController.SimpleMove(playerInput * _speed);
    }

    public void OnTouched(Cell cell)
    {
        switch (cell.State)
        {
            case CellState.Free:
                OnTouchedFreeCell(cell);
                break;            

            case CellState.Path:
                OnTouchedPathCell(cell);
                break;

            case CellState.Edge:
                OnTouchedEdgeCell(cell);
                break;
        }
    }

    private void OnTouchedFreeCell(Cell cell)
    {
        if (_hasInitialTerritory == false)
        {
            _grid.FillInitialTerritoryFrom(cell, _initialTerritoryRadius);
            _hasInitialTerritory = true;
            return;
        }

        cell.SetState(CellState.Path);
        _path.Push(cell);

        if (_path.Count > 1 && CheckTerritoryAppropriation(cell))
        {
            while(_path.Count > 0)
            {
                _path.Pop().SetState(CellState.Owned);
            }
        }            
    }    

    private void OnTouchedPathCell(Cell cell)
    {       
        while (_path.Peek() != cell)
        {
            _path.Pop().SetState(CellState.Free);
        }
    }

    private void OnTouchedEdgeCell(Cell cell)
    {
        transform.position = _path.Peek().transform.position;        
    }

    private bool CheckTerritoryAppropriation(Cell cell)
    {
        Cell[] perimeterCells = GetPerimeterCellsFrom(cell);

        if (HasCellWithState(perimeterCells, CellState.Owned))
        {
            List<Cell> borderCells = GetBorderCellsFrom(perimeterCells);

            foreach (var borderCell in borderCells)
            {
                List<Cell> enclosedFreeCells = GetEnclosedFreeCellsFrom(borderCell, out bool isTouchesEdge);

                if (isTouchesEdge == false)
                    AppropriateTerritory(enclosedFreeCells);                
            }

            return true;
        }

        return false;
    }
    
    private Cell[] GetPerimeterCellsFrom(Cell cell)
    {
        Cell[] cellPerimeter = new Cell[8];
        int x = cell.X;
        int z = cell.Z;
        cellPerimeter[0] = _grid.Cells[++x, z];
        cellPerimeter[1] = _grid.Cells[x,++z];
        cellPerimeter[2] = _grid.Cells[--x,z];
        cellPerimeter[3] = _grid.Cells[--x,z];
        cellPerimeter[4] = _grid.Cells[x,--z];
        cellPerimeter[5] = _grid.Cells[x,--z];
        cellPerimeter[6] = _grid.Cells[++x,z];
        cellPerimeter[7] = _grid.Cells[++x,z];        

        return cellPerimeter;
    }

    private bool HasCellWithState(IEnumerable<Cell> cells, CellState state)
    {       
        foreach (var cell in cells)
        {
            if (cell.State == state)            
                return true;                
        }

        return false;
    }    

    private List<Cell> GetBorderCellsFrom(Cell[] perimeterCells)
    {
        List<Cell> borderCells = new List<Cell>();

        for (int i = 0; i < perimeterCells.Length; i++)
        {
            Cell cell1 = perimeterCells[i];
            Cell cell2 = perimeterCells[(i + 1) % perimeterCells.Length];

            if (cell1.State == CellState.Owned && cell2.State == CellState.Free && borderCells.Contains(cell2) == false)
                borderCells.Add(cell2);

            if (cell1.State == CellState.Free && cell2.State == CellState.Owned && borderCells.Contains(cell1) == false)
                borderCells.Add(cell1);
        }
        
        return borderCells;
    }

    private List<Cell> GetEnclosedFreeCellsFrom(Cell cell, out bool isTouchesEdge)
    {
        List<Cell> enclosedFreeCells = new List<Cell>();
        HashSet<Cell> visited = new HashSet<Cell>();
        Queue<Cell> queue = new Queue<Cell>();
        queue.Enqueue(cell);
        isTouchesEdge = false;

        while (queue.Count > 0)
        {
            Cell currentCell = queue.Dequeue();
            visited.Add(currentCell);
            enclosedFreeCells.Add(currentCell);

            foreach (var nextCell in GetNearestCellsFrom(currentCell))
            {
                if (nextCell.State == CellState.Edge)
                    isTouchesEdge = true;

                if (nextCell.State == CellState.Free && visited.Contains(nextCell) == false)
                    queue.Enqueue(nextCell);
            }
        }

        return enclosedFreeCells;
    }

    private Cell[] GetNearestCellsFrom(Cell cell)
    {
        Cell[] nearestCells = new Cell[4];
        nearestCells[0] = _grid.Cells[cell.X + 1, cell.Z];
        nearestCells[1] = _grid.Cells[cell.X - 1, cell.Z];
        nearestCells[2] = _grid.Cells[cell.X, cell.Z + 1];
        nearestCells[3] = _grid.Cells[cell.X, cell.Z - 1];

        return nearestCells;
    }    

    private void AppropriateTerritory(List<Cell> cells)
    {
        foreach (var cell in cells)
        {
            cell.SetState(CellState.Owned);
        }
    }
}
