using System;
using UnityEngine;

namespace Content.Aaron._Scripts
{
	public class Platform : MonoBehaviour
	{
		[SerializeField] float _MovementRange = 3f;
		[SerializeField] float _MovementSpeed = 1f;

		float _T;
		void Update()
		{
			// if (transform.GetChild(0) == null) return;
			
			Vector3 pos = transform.GetChild(0).localPosition;
			transform.GetChild(0).localPosition = new Vector3(Mathf.PingPong(_T, _MovementRange), pos.y, pos.z);

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