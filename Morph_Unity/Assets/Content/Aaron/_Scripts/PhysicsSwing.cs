using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.Morph.Mechanics
{
	// [RequireComponent(typeof(Rigidbody2D))]
	public class PhysicsSwing : MonoBehaviour
	{
		[Header("Line settings")]
		[SerializeField] Transform _CenterPoint;
		[SerializeField] Transform[] _LinePoints;
		[SerializeField] LineRenderer _LineRenderer;
		
		readonly List<Vector3> _Points = new List<Vector3>();

		void Start()
		{
			if (!TryGetComponent(out _LineRenderer))
			{
				Debug.LogWarning("Could not find the following Component: 'LineRenderer'", gameObject);
			}
			
			_LineRenderer.positionCount = (_LinePoints.Length * 2);
		}

		void Update()
		{
			SwingHandle();
			if(_LineRenderer) LineHandle();
		}

		void SwingHandle()
		{
			
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
	}
}