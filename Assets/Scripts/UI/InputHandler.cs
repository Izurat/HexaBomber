using Signals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InputHandler : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;

    [SerializeField] private Joystick _joystick;
    [SerializeField] private Button _bombButton;

    private E_DirectionType _currentDirection = E_DirectionType.None;

    private void Start()
    {
        _bombButton.onClick.AddListener(HandleSetBomb);
    }

    private void Update()
    {
        float angle = Vector2.SignedAngle(Vector2.up, _joystick.Direction);

        E_DirectionType direction = JoystickToDirection(_joystick.Direction);

        if (direction != _currentDirection) 
        {
            _currentDirection = direction;
            _signalBus.Fire(new JoystickDirectionUpdateSignal(_currentDirection));
        }
    }

    private E_DirectionType JoystickToDirection(Vector2 joystickDirection) 
    {
        if (joystickDirection.sqrMagnitude == 0)
            return E_DirectionType.None;

        float angle = Vector2.SignedAngle(Vector2.up, _joystick.Direction);

        if (angle >= 0 && angle <= 60)
            return E_DirectionType.UpLeft;

        if (angle >= 60 && angle <= 120)
            return E_DirectionType.Left;

        if(angle >= 120 && angle <= 180)
            return E_DirectionType.DownLeft;

        if (angle <= 0 && angle >= -60)
            return E_DirectionType.UpRight;

        if (angle <= -60 && angle >= -120)
            return E_DirectionType.Right;
        
        return E_DirectionType.DownRight;
    }

    private void HandleSetBomb() 
    {
        _signalBus.Fire(new SetBombInputSignal());
    }
}
