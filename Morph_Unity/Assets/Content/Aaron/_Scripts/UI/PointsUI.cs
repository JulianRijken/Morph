using com.Morph.Game;
using UnityEngine;

public class PointsUi : MonoBehaviour
{
	[SerializeField] Texture _CoinSprite;
	int _Points = 0;

	void Start()
	{
		GameManager.GetInstance()._OnCoinAdded += OnCoinAdded;
	}

	void OnCoinAdded(int obj)
	{
		_Points = obj;
	}

	void OnGUI()
	{
		if (_Points > 0)
		{
			for (int i = 1; i <= _Points; i++)
			{
				GUI.DrawTexture(
					new Rect((Screen.width / _Points * (i + 1)), (Screen.height / _Points * (i + 1)), 32, 32),
					_CoinSprite, ScaleMode.ScaleAndCrop, true);
			}
		}
	}
}