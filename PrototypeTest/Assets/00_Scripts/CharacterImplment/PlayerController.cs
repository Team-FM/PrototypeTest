using Photon.Pun;
using UnityEngine;

namespace CharacterImplement
{
	public class PlayerController : MonoBehaviour
	{
		#region Events

		#endregion

		#region Public Fields

		public Character _character;
		public int Health;

		#endregion

		#region MonoBehaviour CallBacks

		void Awake()
		{
			_character = GetComponentInChildren<Character>();
		}

		private void Start()
		{
			Health = _character.CurHP;
		}

		private void Update()
		{
			Health = _character.CurHP;
		}

		#endregion

		#region Public Methods

		public void MoveToPosition(Vector3 destination)
		{
			_character.MoveToPosition(destination);
		}

		public void ChaseTarget(Transform target)
		{
			_character.ChaseTarget(target);
		}

		#endregion
	}
}