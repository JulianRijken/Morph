using System;
using com.Morph.Game;
using UnityEngine;
using UnityEngine.Events;

namespace com.Morph.Mechanics
{
	public class Door : MonoBehaviour
	{
		[SerializeField] int _CoinsRequired;
		[SerializeField] bool _InvokeOnce = true;
		[SerializeField] UnityEvent _OnCoinAmmountReached;
		public void Start()
		{
			GameManager.GetInstance()._OnCoinAdded += OnCoinAdded;
		}

		void OnCoinAdded(int amount)
		{
			if (amount == _CoinsRequired)
			{
				_OnCoinAmmountReached.Invoke();
				if (_InvokeOnce)
					GameManager.GetInstance()._OnCoinAdded -= OnCoinAdded;
			}
		}
		
		void OnDestroy()
		{
			GameManager.GetInstance()._OnCoinAdded -= OnCoinAdded;
		}
	}
}