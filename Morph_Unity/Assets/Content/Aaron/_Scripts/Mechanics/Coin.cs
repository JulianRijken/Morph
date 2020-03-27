using System;
using System.Collections.Generic;
using System.Linq;
using com.Morph.Game;
using UnityEngine;

namespace com.Morph.Mechanics
{
	public class Coin : MonoBehaviour
	{
		[SerializeField] float _Radius;
		
		void Update()
		{
			List<Collider2D> hits = Physics2D.OverlapCircleAll(transform.position, _Radius).ToList();

			hits.ForEach(col =>
			{
				if (col.CompareTag("Player"))
				{
					GameManager.GetInstance(gameObject).AddCoin();
					Destroy(gameObject);
				}
			});

			if (Input.GetKeyDown(KeyCode.C))
			{
				GameManager.GetInstance(gameObject).AddCoin();
				Destroy(gameObject);
			}
		}

#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, _Radius);
		}
#endif
	}
}