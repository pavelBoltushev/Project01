using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float _speed;

    private PlayerInput _input;
    private Queue<AbsoluteDirection> _inputQueue;
    private float _cellSize;
    private Cell _currentCell;
    private bool _movementInProgress;
    private float _holdCounter;
    private AbsoluteDirection _currentDirectionHold;

    private float _movementTime => _cellSize / _speed;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _inputQueue = new Queue<AbsoluteDirection>();
        _cellSize = GetComponent<Player>().GetCellGrid().CellSize;
    }

    private void Start()
    {
        StartCoroutine(HandleInputQueue());
    }

    private void OnEnable()
    {
        _input.DirectionButtonPressed += AddToInputQueue;
        _input.DirectionButtonHold += OnDirectionButtonHold;
    }

    private void OnDisable()
    {
        _input.DirectionButtonPressed -= AddToInputQueue;
        _input.DirectionButtonHold += OnDirectionButtonHold;
    }

    public void SetCurrent(Cell cell)
    {
        _currentCell = cell;        
    }

    private void AddToInputQueue(AbsoluteDirection direction)
    {
        _inputQueue.Enqueue(direction);
    }

    private void OnDirectionButtonHold(AbsoluteDirection direction)
    {
        if(_currentDirectionHold != direction)
        {
            _holdCounter = 0;
            _currentDirectionHold = direction;
        }

        _holdCounter += Time.deltaTime;

        if(_holdCounter >= _movementTime)
        {
            _holdCounter = 0;
            AddToInputQueue(direction);
        }
    }

    private IEnumerator HandleInputQueue()
    {
        while (true)
        {
            if(_inputQueue.Count != 0 && _movementInProgress == false)
            {
                _holdCounter = 0;
                MoveIn(_inputQueue.Dequeue());
            }

            yield return null;
        }
    }

    private void MoveIn(AbsoluteDirection direction)
    {       
        Cell targetCell = _currentCell.GetNextCellIn(direction);

        if(targetCell.State != CellState.Edge)
        {
            SetCurrent(targetCell);
            StartCoroutine(MoveTo(targetCell));
        }
    }

    private IEnumerator MoveTo(Cell cell)
    {
        _movementInProgress = true;
        Vector3 targetPosition = new Vector3(cell.transform.position.x, transform.position.y, cell.transform.position.z);

        while(transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
            yield return null;
        }

        _movementInProgress = false;
    }
}
