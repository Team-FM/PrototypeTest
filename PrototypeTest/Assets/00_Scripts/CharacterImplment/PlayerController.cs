using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.TextCore.Text;

namespace CharacterImplement
{
	public class PlayerController : MonoBehaviour
	{
		#region Events

		#endregion

		#region Public Fields

		public Character _character;

		#endregion

		#region MonoBehaviour CallBacks

		void Awake()
		{
			_character = GetComponentInChildren<Character>();
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