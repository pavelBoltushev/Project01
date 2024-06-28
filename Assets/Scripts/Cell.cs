using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Cell : MonoBehaviour
{   
    private MeshRenderer _renderer;
    private CellGrid _grid;

    public int X { get; private set; }
    public int Z { get; private set; }
    public CellState State { get; private set; }

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.OnTouched(this);
        }
    }    

    public void Init(int x, int z, CellGrid grid)
    {
        X = x;
        Z = z;
        State = CellState.Free;
        _grid = grid;
    }

    public Cell GetNextCellIn(AbsoluteDirection direction)
    {
        switch (direction)
        {
            case AbsoluteDirection.ZPlus:
                return _grid.GetCell(X, Z + 1);

            case AbsoluteDirection.ZMinus:
                return _grid.GetCell(X, Z - 1);

            case AbsoluteDirection.XPlus:
                return _grid.GetCell(X + 1, Z);

            case AbsoluteDirection.XMinus:
                return _grid.GetCell(X - 1, Z);
        }

        return null;
    }

    public void SetState(CellState state)
    {
        State = state;

        switch (state)
        {
            case CellState.Free:
                SetColor(Color.white);
                break;

            case CellState.Path:
                SetColor(Color.yellow);
                break;

            case CellState.Owned:
                SetColor(Color.red);
                break;

            case CellState.Edge:
                SetColor(Color.black);
                break;
        }
    }

    public IEnumerator LaunchAppropriation(float appropriationSpeed)
    {
        SetState(CellState.Owned);
        yield return new WaitForSeconds(1 / appropriationSpeed);

        foreach (var nearCell in _grid.GetNearestCellsFrom(this))
        {
            if(nearCell.State == CellState.Free)
            {
                StartCoroutine(nearCell.LaunchAppropriation(appropriationSpeed));
            }
        }
    }

    private void SetColor(Color color)
    {
        _renderer.material.color = color;
    }
}

public enum CellState
{
    Free,
    Path,
    Owned,
    Edge
}