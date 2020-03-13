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

		public void Start()
		{
			GameManager.GetInstance()._OnCoinAdded += OnCoinAdded;
		}

		void Update()
		{
			List<Collider2D> hits = Physics2D.OverlapCircleAll(transform.position, _Radius).ToList();

			hits.ForEach(col =>
			{
				if (col.CompareTag("Player"))
				{
					GameManager.GetInstance().AddCoin();
					Destroy(gameObject);
				}
			});

			if (Input.GetKeyDown(KeyCode.C))
			{
				GameManager.GetInstance().AddCoin();
				Destroy(gameObject);
			}
		}

		void OnCoinAdded(int coins)
		{
			print(coins);
		}

		void OnDestroy()
		{
			GameManager.GetInstance()._OnCoinAdded -= OnCoinAdded;
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