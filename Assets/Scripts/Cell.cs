using System.Collections;
using System.Collections.Generic;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
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