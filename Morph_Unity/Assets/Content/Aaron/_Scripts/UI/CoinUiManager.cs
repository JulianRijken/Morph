using System.Collections.Generic;
using com.Morph.Game;
using UnityEngine;
using UnityEngine.UI;

public class UiCoin : MonoBehaviour
{
	Vector3 _TargetPosition;
	float _T = 0;

	void Update()
	{
		// if (Input.GetKeyDown(KeyCode.P))
		// {
		// 	SetNewPosition(Camera.main.WorldToScreenPoint(Vector3.right));
		// }
		//
		// if (Vector3.Distance(_TargetPosition, transform.position) < .25)
		// {
		// 	transform.position = Vector3.Lerp(transform.position, _TargetPosition, _T);
		// 	_T += Time.deltaTime;
		// }
	}
	
	public void SetNewPosition(Vector3 screenPosition)
	{
		_T = 0;
		_TargetPosition = screenPosition;
	}
}