using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class Path : MonoBehaviour
{
    [SerializeField] private float _pathAppropriationSpeed;
    [SerializeField] private float _cellsAppropriationSpeed;

    private CellGrid _grid;
    private List<(Cell cell, RelativeDirection direction)> _cells;
    private int _pathTurn;

    public bool AppropriationInProgress { get; private set; }

    public int Count => _cells.Count;

    private void Awake()
    {
        _grid = GetComponent<Player>().GetCellGrid();
        _cells = new List<(Cell, RelativeDirection)>();
        _pathTurn = 0;
    }    

    public void Add(Cell cell)
    {
        cell.SetState(CellState.Path);

        if (_cells.Count < 2)
        {
            _cells.Add((cell, RelativeDirection.Forward));
            return;
        }

        RelativeDirection direction = GetLocalDirection(cell, _cells[_cells.Count - 1].cell, _cells[_cells.Count - 2].cell);
        _pathTurn += (int)direction;
        _cells.Add((cell, direction));        
    }

    public IEnumerator Appropriate()
    {       
        WaitForSeconds wait = new WaitForSeconds(1 / _pathAppropriationSpeed);
        List<Cell> currentCellsList = new List<Cell>();

        foreach (var cell in _cells)
        {
            currentCellsList.Add(cell.cell);
        }

        _cells.Clear();

        currentCellsList[0].SetState(CellState.Owned);

        for (int i = 1; i < currentCellsList.Count; i++)
        {
            yield return wait;
            Cell insideCell = GetInsideCellFrom(currentCellsList[i], currentCellsList[i - 1]);

            if (insideCell != null && insideCell.State == CellState.Free)
                StartCoroutine(insideCell.LaunchAppropriation(_cellsAppropriationSpeed));

            currentCellsList[i].SetState(CellState.Owned);                
        }
        
        _pathTurn = 0;        
    }

    public void CutTo(Cell cell)
    {
        while (_cells[_cells.Count - 1].cell != cell)
        {
            _pathTurn -= (int)_cells[_cells.Count - 1].direction;
            _cells[_cells.Count - 1].cell.SetState(CellState.Free);
            _cells.RemoveAt(_cells.Count - 1);
        }
    }    

    private Cell GetInsideCellFrom(Cell currentCell, Cell previousCell)
    {
        AbsoluteDirection direction = GetGlobalDirection(currentCell, previousCell);

        switch (direction)
        {
            case AbsoluteDirection.ZPlus:
                if (_pathTurn > 0)
                    return _grid.GetCell(previousCell.X + 1, previousCell.Z);
                if (_pathTurn < 0)
                    return _grid.GetCell(previousCell.X - 1, previousCell.Z);
                break;

            case AbsoluteDirection.ZMinus:
                if (_pathTurn > 0)
                    return _grid.GetCell(previousCell.X - 1, previousCell.Z);
                if (_pathTurn < 0)
                    return _grid.GetCell(previousCell.X + 1, previousCell.Z);
                break;

            case AbsoluteDirection.XPlus:
                if (_pathTurn > 0)
                    return _grid.GetCell(previousCell.X, previousCell.Z - 1);
                if (_pathTurn < 0)
                    return _grid.GetCell(previousCell.X, previousCell.Z + 1);
                break;

            case AbsoluteDirection.XMinus:
                if (_pathTurn > 0)
                    return _grid.GetCell(previousCell.X, previousCell.Z + 1);
                if (_pathTurn < 0)
                    return _grid.GetCell(previousCell.X, previousCell.Z - 1);
                break;
        }

        return null;
    }

    private RelativeDirection GetLocalDirection(Cell currentCell, Cell previousCell, Cell prevpreviousCell)
    {
        AbsoluteDirection currentGlobalDirection = GetGlobalDirection(currentCell, previousCell);
        AbsoluteDirection previousGlobalDirection = GetGlobalDirection(previousCell, prevpreviousCell);

        if (previousGlobalDirection == AbsoluteDirection.ZPlus)
        {
            if (currentGlobalDirection == AbsoluteDirection.XPlus)
                return RelativeDirection.Right;

            if (currentGlobalDirection == AbsoluteDirection.XMinus)
                return RelativeDirection.Left;
        }

        if (previousGlobalDirection == AbsoluteDirection.ZMinus)
        {
            if (currentGlobalDirection == AbsoluteDirection.XPlus)
                return RelativeDirection.Left;

            if (currentGlobalDirection == AbsoluteDirection.XMinus)
                return RelativeDirection.Right;
        }

        if (previousGlobalDirection == AbsoluteDirection.XMinus)
        {
            if (currentGlobalDirection == AbsoluteDirection.ZPlus)
                return RelativeDirection.Right;

            if (currentGlobalDirection == AbsoluteDirection.ZMinus)
                return RelativeDirection.Left;
        }

        if (previousGlobalDirection == AbsoluteDirection.XPlus)
        {
            if (currentGlobalDirection == AbsoluteDirection.ZPlus)
                return RelativeDirection.Left;

            if (currentGlobalDirection == AbsoluteDirection.ZMinus)
                return RelativeDirection.Right;
        }

        return RelativeDirection.Forward;
    }

    private AbsoluteDirection GetGlobalDirection(Cell currentCell, Cell previousCell)
    {
        if (currentCell.Z > previousCell.Z)
            return AbsoluteDirection.ZPlus;
        else if (currentCell.Z < previousCell.Z)
            return AbsoluteDirection.ZMinus;
        else if (currentCell.X > previousCell.X)
            return AbsoluteDirection.XPlus;
        else
            return AbsoluteDirection.XMinus;
    }
}

public enum RelativeDirection
{
    Left = -1,
    Forward = 0,
    Right = 1
}

public enum AbsoluteDirection
{
    ZPlus,
    ZMinus,
    XPlus,
    XMinus
}
