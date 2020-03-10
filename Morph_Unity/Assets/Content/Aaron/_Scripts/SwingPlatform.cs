using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwingPlatform : MonoBehaviour
{
	[Header("Platform settings")]
	[SerializeField] Transform _CenterPoint;
	[SerializeField] Transform[] _LinePoints;
	[SerializeField] LineRenderer _LineRenderer;
	List<Vector3> _Points = new List<Vector3>();

	void Start()
	{
		_LineRenderer.positionCount = (_LinePoints.Length * 2);
	}

	void Update()
	{
		SwingHandle();
	}

	void SwingHandle()
	{
		if (_LineRenderer == null) return;
		
		_Points.Clear();

		for (int i = 0; i < _LinePoints.Length; i++)
		{
			_Points.Add(_LinePoints[i].position);
			_Points.Add(_CenterPoint.position);
		}

		_LineRenderer.SetPositions(_Points.ToArray());
		// _LineRenderer.positionCount = _Points.Count;
		// _LineRenderer.SetPositions(_Points.ToArray());
	}
}
