using System.Collections.Generic;
using com.Morph.Game;
using UnityEngine;
using UnityEngine.UI;

public class PointsUi : MonoBehaviour
{
	[SerializeField] GameObject _SpriteUi;
	[SerializeField] GridLayoutGroup _GridLayout;
	readonly List<GameObject> _PointObjects = new List<GameObject>();
	int _Points = 0;

	void Start()
	{
		GameManager.GetInstance(gameObject)._OnCoinAdded += OnCoinAdded;
		_GridLayout = FindObjectOfType<GridLayoutGroup>();
	}

	void OnCoinAdded(int obj)
	{
		_Points = obj;
		_PointObjects.Add(Instantiate(_SpriteUi, Vector3.zero, Quaternion.identity, _GridLayout.transform));
	}
}