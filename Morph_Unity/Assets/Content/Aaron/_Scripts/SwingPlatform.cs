using System.Collections.Generic;
using UnityEngine;

public class SwingPlatform : MonoBehaviour
{
	[Header("Platform settings")]
	[SerializeField] Transform _AnchorPoint;
	[SerializeField] Transform _Platform;
	[SerializeField] bool _Animated;
	[SerializeField] float _SwingSpeed;
	[SerializeField] Vector3 _SwingAngle;

	[Header("Line settings")]
	[SerializeField] Transform _CenterPoint;
	[SerializeField] Transform[] _LinePoints;
	[SerializeField] LineRenderer _LineRenderer;

	readonly List<Vector3> _Points = new List<Vector3>();
	bool _SwingForward = true;
	float _T;
	
	void Start()
	{
		_LineRenderer.positionCount = (_LinePoints.Length * 2);
		if (!_Animated && TryGetComponent(out Animator anim))
		{
			anim.enabled = false;
		}
	}

	void Update()
	{
		if(!_Animated) SwingHandle();
		LineHandle();
	}

	void LineHandle()
	{
		if (_LineRenderer == null) return;

		_Points.Clear();

		for (int i = 0; i < _LinePoints.Length; i++)
		{
			_Points.Add(_LinePoints[i].position);
			_Points.Add(_CenterPoint.position);
		}

		_LineRenderer.SetPositions(_Points.ToArray());
	}

	//TODO Add damping
	void SwingHandle()
	{
		_AnchorPoint.eulerAngles = Vector3.Lerp(-_SwingAngle, _SwingAngle, _T);
		_Platform.localEulerAngles = _AnchorPoint.localEulerAngles * -1;
		if (_T >= 1)
		{
			_SwingForward = false;
		} else if (_T <= 0)
		{
			_SwingForward = true;
		}
		
		_T = _SwingForward ? _T + Time.deltaTime * _SwingSpeed :_T - Time.deltaTime * _SwingSpeed;
	}
}