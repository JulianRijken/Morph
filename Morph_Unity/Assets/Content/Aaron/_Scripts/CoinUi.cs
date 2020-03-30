using UnityEngine;

public enum TransitionState
{
	Static,
	FollowTarget
}

public class CoinUi : MonoBehaviour
{
	[SerializeField] float _TransitionSpeed = 1f;
	Transform _TargetTransform;
	Vector3 _TargetPosition;
	TransitionState _State;
	float _T = 0;

	Vector3 _NewPos;

	void Update()
	{
		if (Camera.main == null) return;
		
		switch (_State)
		{
			case TransitionState.Static:
				if (Vector3.Distance(_TargetPosition, transform.position) > .25)
				{
					_NewPos = Vector3.Lerp(transform.position, _TargetPosition, _T);
				}

				break;

			case TransitionState.FollowTarget:
				Vector3 targetTransformPos = Camera.main.WorldToScreenPoint(_TargetTransform.position);
				_NewPos = Vector3.Lerp(transform.position, targetTransformPos, _T);
				break;
		}

		transform.position = _NewPos;
		_T += Time.deltaTime * _TransitionSpeed;
	}

	public void SetNewPosition(Vector3 screenPosition)
	{
		_T = 0;
		_NewPos = transform.position;
		_State = TransitionState.Static;
		_TargetPosition = screenPosition;
	}

	public void SetFollowTransform(Transform targetTransform)
	{
		_T = 0;
		_State = TransitionState.FollowTarget;
		_TargetTransform = targetTransform;
	}
}