using System;
using UnityEngine;

namespace com.Morph.Game
{
	public class GameManager : MonoBehaviour
	{
		GameManagerSettings _Settings;
		static GameManager _Instance;
		int _CoinCount = 0;

		public Action<int> _OnCoinAdded;
		
		public static GameManager GetInstance()
		{
			if (Application.isPlaying) {
				if (_Instance == null || !FindObjectOfType<GameManager>())
				{
					GameObject singletonObj = (GameObject) Instantiate(Resources.Load("GameManager"), Vector3.zero,
						Quaternion.identity, null);
					_Instance = singletonObj.AddComponent<GameManager>();
					singletonObj.name = "GameManager (Singleton)";
					// DontDestroyOnLoad(singletonObj);
				}
			}
			
			return _Instance;
		}

		void Awake()
		{
			_Settings = Resources.Load<GameManagerSettings>("GameManager Settings");
		}

		public void AddCoin()
		{
			_CoinCount++;
			_OnCoinAdded?.Invoke(_CoinCount);
		}

		public void AddCoins(int amount)
		{
			_CoinCount += amount;
			_OnCoinAdded?.Invoke(_CoinCount);
		}

		public void OnDestroy()
		{
			_Instance = null;
		}
	}
}