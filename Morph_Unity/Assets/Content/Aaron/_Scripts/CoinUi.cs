using UnityEngine;

namespace Content.Aaron._Scripts
{
	public class CoinUi : MonoBehaviour
	{
		public Transform _Target;
		
		void Update()
		{
			if (Camera.main == null) return;
			
			Vector3 newPos;
			newPos = Camera.main.WorldToScreenPoint(_Target.position);
			
			print(newPos);
			transform.position = newPos;
		}
	}
}