public class ElevatorBase : IElevator
{
    private int _currentFloor;
    private ElevatorDirection _direction;
    private ElevatorState _state;
    private int _passengerCount;

    public int CurrentFloor => _currentFloor;
    public ElevatorDirection Direction => _direction;
    public ElevatorState State => _state;
    public int PassengerCount => _passengerCount;
    public bool CanAcceptPassengers => _passengerCount < Capacity;
    public int Capacity { get; }

    protected ElevatorBase(int capacity, int startFloor = 1)
    {
        Capacity = capacity;
        _currentFloor = startFloor;
        _direction = ElevatorDirection.Stationary;
        _state = ElevatorState.Idle;
        _passengerCount = 0;
    }

    public virtual void MoveToFloor(int floor)
    {
        if (_currentFloor == floor)
        {
            _direction = ElevatorDirection.Stationary;
            return;
        }
        if (floor > _currentFloor)
        {
            _direction = ElevatorDirection.Up;
        }
        else if (floor < _currentFloor)
        {
            _direction = ElevatorDirection.Down;
        }
        _currentFloor = floor;
        _state = ElevatorState.Idle;
    }

    public void AddPassengers(int count)
    {
        if (_passengerCount + count > Capacity)
        {
            // @TODO: throw custome exception for capacity being exceeded
        }

        _passengerCount += count;
    }

    public void RemovePassengers(int count)
    {
        if (count > _passengerCount)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Cannot remove {count} passengers, only {_passengerCount} onboard.");
        }

        _passengerCount -= count;
    }
}
