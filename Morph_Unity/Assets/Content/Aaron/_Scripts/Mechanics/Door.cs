using System;
using com.Morph.Game;
using UnityEngine;
using UnityEngine.Events;

namespace com.Morph.Mechanics
{
	public class Door : MonoBehaviour
	{
		[SerializeField] int _CoinsRequired;
		[SerializeField] Transform[] _CoinPositions;
		[SerializeField] UnityEvent _OnCoinAmountReached;

		void OnTriggerEnter2D(Collider2D other)
		{
			if(!other.CompareTag("Player"))
				return;
			
			for (int i = 0; i < _CoinPositions.Length; i++)
			{
				if(!CoinUiManager.Instance.UseCoin(_CoinPositions[i]))
					return;
			}
			
			if (Input.GetKeyDown(KeyCode.E))
			{
				_OnCoinAmountReached.Invoke();
			}
		}

		void OnTriggerExit2D(Collider2D other)
		{
			if(!other.CompareTag("Player"))
				return;
			
			CoinUiManager.Instance.ResetCoins();
		}
	}
}