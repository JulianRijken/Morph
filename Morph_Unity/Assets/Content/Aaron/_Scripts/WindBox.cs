using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.Morph.Mechanics
{
	public class WindBox : MonoBehaviour
	{
		[SerializeField] LayerMask _LayerMask;
		[SerializeField] float _WindStrength = 5;

		void Start()
		{
			GetComponent<SpriteRenderer>().enabled = false;
		}

		void FixedUpdate()
		{
			List<Collider2D> hits = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0,
				_LayerMask).ToList();

			print(hits.Count);
			
			hits.ForEach(col =>
			{
				if (col.TryGetComponent(out Rigidbody2D rig))
					rig.AddForce(transform.right * (_WindStrength * 10), ForceMode2D.Force);
				
				print("A");
			});
		}

#if UNITY_EDITOR

		void OnDrawGizmos()
		{
			#region Box
		
			Gizmos.DrawWireCube(transform.position, transform.localScale);
		
			#endregion
		
			// #region Arrow
			//
			// Vector2 start = transform.position;
			// start.x -= 0.5f;
			// Vector2 end = transform.position;
			// end.x += 0.25f;
			//
			// Vector2 startA = transform.position;
			// startA.x -= .25f;
			// startA.y -= .5f;
			// Vector2 startB = transform.position;
			// ;
			// startB.x -= .25f;
			// startB.y += .5f;
			//
			// Gizmos.color = Color.magenta;
			// Gizmos.DrawLine(start, end);
			// Gizmos.DrawLine(startA, end);
			// Gizmos.DrawLine(startB, end);
			//
			// #endregion
			
		}

#endif
	}
}