using System.Collections.Generic;
using System.Linq;
using com.Morph.Game;
using UnityEngine;
using UnityEngine.UI;

public class CoinUiManager : MonoBehaviour
{
	[SerializeField] GameObject _CoinUiPrefab;
	[SerializeField] Vector2 _Offset;
	[SerializeField] Transform _Spawn;
	[SerializeField]Vector3[] _Corners = new Vector3[4];
	Stack<CoinUi> _Coins = new Stack<CoinUi>();
	Stack<CoinUi> _CoinsUse = new Stack<CoinUi>();
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
		
		_Coins.Push(Instantiate(_CoinUiPrefab, _Spawn.position, Quaternion.identity, transform).GetComponent<CoinUi>());
		for (int i = 0; i < newAmmount; i++)
		{
			Vector3 targetPos = Vector3.Lerp(Vector3.zero, _Corners[2], (1f / (newAmmount+1)) * (i+1));
			targetPos.y = _Offset.y;
			_Coins.ToList()[i].SetNewPosition(targetPos);
		}
	}

	public bool UseCoin(Transform targetTransform)
	{
		if (_Coins.Peek() == null) return false;
		
		_CoinsUse.Push(_Coins.Pop());
		CoinUi ui = _CoinsUse.Peek();
		ui.SetFollowTransform(targetTransform);

		return true;
	}
	
	public void ResetCoins()
	{
		for (int i = 0; i < _CoinsUse.Count; i++)
		{
			_Coins.Push(_CoinsUse.Pop());
		}
		
		for (int i = 0; i < _Coins.Count; i++)
		{
			Vector3 targetPos = Vector3.Lerp(Vector3.zero, _Corners[2], (1f / (_Coins.Count)) * (i+1));
			targetPos.y = _Offset.y;
			_Coins.ToList()[i].SetNewPosition(targetPos);
		}
	}
}