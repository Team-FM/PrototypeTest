using UnityEngine;
using UnityEngine.AI;

namespace CharacterImplement
{
	public class PlayerInput : MonoBehaviour
	{
		#region Events

		#endregion

		#region Public Field

		public PlayerController _player;

		#endregion

		#region MonoBehaviour CallBacks

		void Awake()
		{
			_player = GetComponent<PlayerController>();
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(1))
			{
				RaycastHit hit;
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
				{
					if (null != hit.collider && hit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
					{
						Debug.Log("Ãß°Ý");
						_player.ChaseTarget(hit.collider.transform);
					}
					else
					{
						Debug.Log("¾Ó");
						_player.MoveToPosition(hit.point);
					}
				}
			}
		}

		#endregion
	}
}