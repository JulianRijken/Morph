using System;
using com.Morph.Game;
using UnityEngine;
using UnityEngine.Events;

namespace com.Morph.Mechanics
{
	public class Door : MonoBehaviour
	{
		[SerializeField] int _CoinsRequired;

		[Tooltip("If you want the On Coin Amount event to be run more then once, then uncheck this checkbox")]
		[SerializeField] bool _InvokeOnce = true;

		[SerializeField] UnityEvent _OnCoinAmountReached;

		public void Start()
		{
			GameManager.GetInstance(gameObject)._OnCoinAdded += OnCoinAdded;
		}

		void OnCoinAdded(int amount)
		{
			if (amount == _CoinsRequired)
			{
				_OnCoinAmountReached.Invoke();
				if (_InvokeOnce)
					GameManager.GetInstance(gameObject)._OnCoinAdded -= OnCoinAdded;
			}
		}
	}
}