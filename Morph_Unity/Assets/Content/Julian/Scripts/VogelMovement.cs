using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VogelMovement", menuName = "VogelMovement")]
public class VogelMovement : MovementBehaviour
{
    public override void Move(Rigidbody2D rigidbody)
    {
        Debug.Log("Vogel Movement");
    }
}
