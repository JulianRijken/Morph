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
			
			hits.ForEach(col =>
			{
				if (col.TryGetComponent(out Rigidbody2D rig))
					rig.AddForce(transform.right * (_WindStrength * 10), ForceMode2D.Impulse);
			});
		}
	}
}