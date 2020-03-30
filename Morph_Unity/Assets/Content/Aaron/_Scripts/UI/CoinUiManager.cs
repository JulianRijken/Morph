using System.Collections.Generic;
using com.Morph.Game;
using UnityEngine;
using UnityEngine.UI;

public class CoinUiManager : MonoBehaviour
{
	[SerializeField] GameObject _CoinUiPrefab;
	[SerializeField] Vector2 _Offset;
	[SerializeField] Transform _Spawn;
	[SerializeField]Vector3[] _Corners = new Vector3[4];
	List<CoinUi> _Coins = new List<CoinUi>();
	void Start()
	{
		GameManager.GetInstance()._OnCoinAdded += OnCoinAdded;
	}

	void OnCoinAdded(int newAmmount)
	{
		GetComponent<RectTransform>().GetWorldCorners(_Corners);
		for (int i = 0; i < _Corners.Length; i++)
		{
			_Corners[i].y = 0;
		}
		
		_Coins.Add(Instantiate(_CoinUiPrefab, _Spawn.position, Quaternion.identity, transform).GetComponent<CoinUi>());
		for (int i = 0; i < newAmmount; i++)
		{
			Vector3 targetPos = Vector3.Lerp(Vector3.zero, _Corners[2], (1f / (newAmmount+1)) * (i+1));
			targetPos.y = _Offset.y;
			_Coins[i].SetNewPosition(targetPos);
		}
	}
}