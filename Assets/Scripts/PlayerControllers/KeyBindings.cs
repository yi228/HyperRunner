using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindings : MonoBehaviour
{
    [Header("Basic Move")]
    public KeyCode jump = KeyCode.Space;
    public KeyCode run = KeyCode.LeftShift;
    public KeyCode crouch = KeyCode.LeftControl;
    [Header("Wall Running")]
    public KeyCode wallJump = KeyCode.Space;
    public KeyCode upRunning = KeyCode.LeftShift;
    public KeyCode downRunning = KeyCode.LeftControl;
    [Header("Grapple & Swing")]
    public KeyCode grapple = KeyCode.Mouse1;
    public KeyCode swing = KeyCode.Mouse0;
    [Header("Katana")]
    public KeyCode KatAttack = KeyCode.F;
}
