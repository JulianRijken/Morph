using System;
using com.Morph.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace com.Morph.Mechanics
{
	public class Door : MonoBehaviour
	{
		[SerializeField] int _CoinsRequired;
		[SerializeField] Transform[] _CoinPositions;
		[SerializeField] UnityEvent _OnCoinAmountReached;
		[SerializeField] Collider2D _OpenCollider, _ClosedCollider;
		bool _AmountReached;
		bool _Invoked;

		void Start()
		{
			_OpenCollider.enabled = false;
			_ClosedCollider.enabled = true;
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if(!other.CompareTag("Player"))
				return;
			
			if (_Invoked)
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				return;
			}
			
			for (int i = 0; i < _CoinPositions.Length; i++)
			{
				if(!CoinUiManager.Instance.UseCoin(_CoinPositions[i]))
					return;
			}

			_AmountReached = true;
		}

		void Update()
		{
			if (!_Invoked && _AmountReached)
			{
				_Invoked = true;
				_OnCoinAmountReached.Invoke();
				_OpenCollider.enabled = true;
				_ClosedCollider.enabled = false;
				return;
			}
			//
			// if (_Invoked)
			// {
			// 	SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			// }
		}

		void OnTriggerExit2D(Collider2D other)
		{
			if(!other.CompareTag("Player"))
				return;
			
			if(_Invoked) return;
			
			CoinUiManager.Instance.ResetCoins();
			_AmountReached = false;
		}
	}
}
