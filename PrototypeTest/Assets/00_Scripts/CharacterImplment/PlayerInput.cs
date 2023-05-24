using Photon.Pun;
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
		public Character _character;

		#endregion

		#region MonoBehaviour CallBacks

		void Awake()
		{
			_player = GetComponent<PlayerController>();
			_character = GetComponentInChildren<Character>();
		}

		private void Update()
		{
			if (true == _character.photonView.IsMine)
			{
				ProcessInput();
			}
		}

		#endregion

		#region Public Methods

		public void ProcessInput()
		{
			if (Input.GetMouseButtonDown(1))
			{
				RaycastHit hit;
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
				{
					if (null != hit.collider && hit.collider.gameObject.layer != LayerMask.NameToLayer("Default"))
					{
						Debug.Log("추격");
						_player.ChaseTarget(hit.collider.transform);
					}
					else
					{
						Debug.Log("해당 좌표로 이동");
						_player.MoveToPosition(hit.point);
					}
				}
			}
		}

		#endregion
	}
}