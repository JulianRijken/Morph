using System;
using UnityEngine;

namespace Content.Aaron._Scripts
{
	public class Platform : MonoBehaviour
	{
		[SerializeField] float _MovementRange = 3f;
		[SerializeField] float _MovementSpeed = 1f;

		Vector3 _StartPosition;
		float _T;

		void Start()
		{
			_StartPosition = transform.localPosition;
		}
		
		void Update()
		{
			// if (transform.GetChild(0) == null) return;
			
			Vector3 pos = transform.localPosition;
			transform.localPosition = new Vector3(_StartPosition.x + Mathf.PingPong(_T, _MovementRange), pos.y, pos.z);

			_T += Time.deltaTime;
		}

		void OnCollisionEnter2D(Collision2D other)
		{
			if (other.transform.CompareTag("Player"))
			{
				other.transform.parent = transform;
			}
		}

		void OnCollisionExit2D(Collision2D other)
		{
			if (other.transform.CompareTag("Player"))
			{
				other.transform.parent = null;
			}
		}
	}
}