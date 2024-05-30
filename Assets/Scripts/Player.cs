using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Path))]
public class Player : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private CellGrid _grid;
    [SerializeField] private int _initialTerritoryRadius;

    private CharacterController _characterController;    
    private Path _path;    
    private bool _hasInitialTerritory;    

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _path = GetComponent<Path>();
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

    private void OnTouchedFree(Cell cell)
    {
        if (_hasInitialTerritory == false)
        {
            _grid.AppropriateInitialTerritoryFrom(cell, _initialTerritoryRadius);
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
        _path.Appropriate();
    }           
}
