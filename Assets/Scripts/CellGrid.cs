using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid : MonoBehaviour
{
    [SerializeField] private float _cellSize;
    [SerializeField] private int _gridLength;
    [SerializeField] private int _gridWidth;
    [SerializeField] private float _gapWigth;
    [SerializeField] private Cell _cellTemplate;    

    public Cell[,] Cells { get; private set; }

    private void Start()
    {
        BuildGrid();
    }

    public void BuildGrid()
    {
        Cells = new Cell[_gridLength, _gridWidth];

        Vector3 startPosition = new Vector3(transform.position.x - (((_gridLength - 1) * _cellSize) / 2f ), transform.position.y, transform.position.z - (((_gridWidth - 1) * _cellSize) / 2f));

        for (int x = 0; x < _gridLength; x++)
        {
            for (int z = 0; z < _gridWidth; z++)
            {
                Cell cell = Instantiate<Cell>(_cellTemplate, new Vector3(startPosition.x + x * _cellSize, startPosition.y, startPosition.z + z * _cellSize), Quaternion.identity);
                Cells[x, z] = cell;
                cell.Init(x, z, this);
                cell.transform.localScale = new Vector3(cell.transform.localScale.x * _cellSize - _gapWigth, cell.transform.localScale.y, cell.transform.localScale.z * _cellSize - _gapWigth);
                
                if (x == 0 || x == _gridLength - 1 || z == 0 || z == _gridWidth - 1)
                    cell.SetState(CellState.Edge);
            }
        }
    }

    public void FillInitialTerritoryFrom(Cell cell, int initialTerritoryRadius)
    {
        Queue<(Cell, int)> queue = new Queue<(Cell, int)>();
        queue.Enqueue((cell, 1));

        while (queue.Count > 0)
        {
            (Cell cell, int number) currentCellInfo = queue.Dequeue();
            currentCellInfo.cell.SetState(CellState.Owned);

            if(currentCellInfo.number < initialTerritoryRadius)
            {
                Cell[] nearestCells = GetNearestCellsFrom(currentCellInfo.cell);

                foreach (var nearestCell in nearestCells)
                {
                    if(nearestCell.State != CellState.Owned)
                    {
                        queue.Enqueue((nearestCell, currentCellInfo.number + 1));
                    }
                }
            }
        }
    }

    private Cell[] GetNearestCellsFrom(Cell cell)
    {
        Cell[] nearestCells = new Cell[4];
        nearestCells[0] = Cells[cell.X + 1, cell.Z];
        nearestCells[1] = Cells[cell.X - 1, cell.Z];
        nearestCells[2] = Cells[cell.X, cell.Z + 1];
        nearestCells[3] = Cells[cell.X, cell.Z - 1];

        return nearestCells;
    }
}
