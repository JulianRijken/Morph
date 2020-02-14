using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{

    [SerializeField] private MovementBehaviour movementBehaviour;
    

    void Update()
    {
        movementBehaviour.Move(GetComponent<Rigidbody2D>());
    }
}
