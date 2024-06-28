using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{  
    public event Action<AbsoluteDirection> DirectionButtonPressed;
    public event Action<AbsoluteDirection> DirectionButtonHold;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            DirectionButtonPressed.Invoke(AbsoluteDirection.ZPlus);

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            DirectionButtonPressed.Invoke(AbsoluteDirection.ZMinus);

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            DirectionButtonPressed.Invoke(AbsoluteDirection.XMinus);

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            DirectionButtonPressed.Invoke(AbsoluteDirection.XPlus);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            DirectionButtonHold.Invoke(AbsoluteDirection.ZPlus);

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            DirectionButtonHold.Invoke(AbsoluteDirection.ZMinus);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            DirectionButtonHold.Invoke(AbsoluteDirection.XMinus);

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            DirectionButtonHold.Invoke(AbsoluteDirection.XPlus);
    }
}
