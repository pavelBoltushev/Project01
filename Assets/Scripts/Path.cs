using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private CellGrid _grid;

    private List<(Cell cell, LocalDirection direction)> _cells;
    private int _pathTurn;    

    private void Awake()
    {
        _cells = new List<(Cell, LocalDirection)>();
        _pathTurn = 0;
    }    

    public void Add(Cell cell)
    {
        cell.SetState(CellState.Path);

        if (_cells.Count < 2)
        {
            _cells.Add((cell, LocalDirection.Forward));
            return;
        }

        LocalDirection direction = GetLocalDirection(cell, _cells[_cells.Count - 1].cell, _cells[_cells.Count - 2].cell);
        _pathTurn += (int)direction;
        _cells.Add((cell, direction));        
    }

    public void Appropriate()
    {
        if(_cells.Count != 0)
        {
            Cell insideFreeCell = GetInsideFreeCell();

            if (insideFreeCell != null)
                _grid.AppropriateEnclosedFreeCellsFrom(insideFreeCell);

            foreach (var cell in _cells)
            {
                cell.cell.SetState(CellState.Owned);
            }

            _cells.Clear();
            _pathTurn = 0;
        }        
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

    private Cell GetInsideFreeCell()
    {
        for (int i = 1; i < _cells.Count; i++)
        {
            Cell insideCell = GetInsideCellFrom(_cells[i].cell, _cells[i - 1].cell);

            if (insideCell != null && insideCell.State == CellState.Free)
                return insideCell;
        }

        return null;
    }

    private Cell GetInsideCellFrom(Cell currentCell, Cell previousCell)
    {
        GlobalDirection direction = GetGlobalDirection(currentCell, previousCell);

        switch (direction)
        {
            case GlobalDirection.ZPlus:
                if (_pathTurn > 0)
                    return _grid.GetCell(previousCell.X + 1, previousCell.Z);
                if (_pathTurn < 0)
                    return _grid.GetCell(previousCell.X - 1, previousCell.Z);
                break;

            case GlobalDirection.ZMinus:
                if (_pathTurn > 0)
                    return _grid.GetCell(previousCell.X - 1, previousCell.Z);
                if (_pathTurn < 0)
                    return _grid.GetCell(previousCell.X + 1, previousCell.Z);
                break;

            case GlobalDirection.XPlus:
                if (_pathTurn > 0)
                    return _grid.GetCell(previousCell.X, previousCell.Z - 1);
                if (_pathTurn < 0)
                    return _grid.GetCell(previousCell.X, previousCell.Z + 1);
                break;

            case GlobalDirection.XMinus:
                if (_pathTurn > 0)
                    return _grid.GetCell(previousCell.X, previousCell.Z + 1);
                if (_pathTurn < 0)
                    return _grid.GetCell(previousCell.X, previousCell.Z - 1);
                break;
        }

        return null;
    }

    private LocalDirection GetLocalDirection(Cell currentCell, Cell previousCell, Cell prevpreviousCell)
    {
        GlobalDirection currentGlobalDirection = GetGlobalDirection(currentCell, previousCell);
        GlobalDirection previousGlobalDirection = GetGlobalDirection(previousCell, prevpreviousCell);

        if (previousGlobalDirection == GlobalDirection.ZPlus)
        {
            if (currentGlobalDirection == GlobalDirection.XPlus)
                return LocalDirection.Right;

            if (currentGlobalDirection == GlobalDirection.XMinus)
                return LocalDirection.Left;
        }

        if (previousGlobalDirection == GlobalDirection.ZMinus)
        {
            if (currentGlobalDirection == GlobalDirection.XPlus)
                return LocalDirection.Left;

            if (currentGlobalDirection == GlobalDirection.XMinus)
                return LocalDirection.Right;
        }

        if (previousGlobalDirection == GlobalDirection.XMinus)
        {
            if (currentGlobalDirection == GlobalDirection.ZPlus)
                return LocalDirection.Right;

            if (currentGlobalDirection == GlobalDirection.ZMinus)
                return LocalDirection.Left;
        }

        if (previousGlobalDirection == GlobalDirection.XPlus)
        {
            if (currentGlobalDirection == GlobalDirection.ZPlus)
                return LocalDirection.Left;

            if (currentGlobalDirection == GlobalDirection.ZMinus)
                return LocalDirection.Right;
        }

        return LocalDirection.Forward;
    }

    private GlobalDirection GetGlobalDirection(Cell currentCell, Cell previousCell)
    {
        if (currentCell.Z > previousCell.Z)
            return GlobalDirection.ZPlus;
        else if (currentCell.Z < previousCell.Z)
            return GlobalDirection.ZMinus;
        else if (currentCell.X > previousCell.X)
            return GlobalDirection.XPlus;
        else
            return GlobalDirection.XMinus;
    }

    private enum LocalDirection
    {
        Left = -1,
        Forward = 0,
        Right = 1
    }

    private enum GlobalDirection
    {
        ZPlus,
        ZMinus,
        XPlus,
        XMinus
    }
}
