using UnityEngine;

[RequireComponent(typeof(Path), typeof(PlayerMover), typeof(PlayerInput))]
public class Player : MonoBehaviour
{    
    [SerializeField] private CellGrid _grid;
    [SerializeField] private int _initialTerritoryRadius;

    private Path _path;
    private PlayerMover _mover;
    private bool _hasInitialTerritory;    

    private void Awake()
    {
        _path = GetComponent<Path>();
        _mover = GetComponent<PlayerMover>();
        _hasInitialTerritory = false;
    }    

    public void OnTouched(Cell cell)
    {
        switch (cell.State)
        {
            case CellState.Free:
                OnTouchedFree(cell);
                break;            

            case CellState.Path:
                OnTouchedPath(cell);
                break;

            case CellState.Owned:
                OnTouchedOwned(cell);
                break;
        }
    }

    public CellGrid GetCellGrid()
    {
        return _grid;
    }

    private void OnTouchedFree(Cell cell)
    {
        if (_hasInitialTerritory == false)
        {
            _grid.AppropriateInitialTerritoryFrom(cell, _initialTerritoryRadius);
            _mover.SetCurrent(cell);
            _hasInitialTerritory = true;
            return;
        }
        
        _path.Add(cell);        
    }    

    private void OnTouchedPath(Cell cell)
    {
        _path.CutTo(cell);
    }    

    private void OnTouchedOwned(Cell cell)
    {
        if (_path.Count != 0)
           StartCoroutine(_path.Appropriate());
    }           
}
