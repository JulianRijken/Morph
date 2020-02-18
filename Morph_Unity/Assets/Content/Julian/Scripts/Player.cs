using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Morph.Julian
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {

        [SerializeField] private MovementBehaviour movementBehaviour;

        private Rigidbody2D m_rigidbody2D;
        private BoxCollider2D m_boxCollider2D;

        private void Update()
        {
            movementBehaviour.Move(GetComponent<Rigidbody2D>());
        }
    }
}
