using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using static CharacterImplement.Character;

namespace CharacterImplement
{
    public class Character : MonoBehaviour
    {
        public enum StateKind
        {
            Idle,
            Move,
            Attack,
            Skill1,
		}

		public enum MoveKind
		{
			None,
			MoveToPos,
			ChaseCharacter
		}

		#region Events

		#endregion

		#region Public Fields

		public Animator _animator;
		public NavMeshAgent _agent;

		public float _moveSpeed = 0f;
        public float _attackRange = 0f;

		public float _stopDistance = 0f;
		public Transform _chaseTarget;
		public MoveKind _moveKind;

		public CancellationTokenSource _source;
		public CancellationTokenSource _source2;
		public CancellationToken _chaseTargetToken;
		public CancellationToken _arriveCheckToken;

		public float _skill1MoveSpeed = 0f;
		public bool _isSkillActionMoveLock = false;
		public bool _isAtkActionDivideUpperBody = false;
		public bool _isSkill1State = false;

		#endregion

		#region MonoBehaviour CallBacks

		void Awake()
        {
			_animator = GetComponentInChildren<Animator>();
			_agent = GetComponentInParent<NavMeshAgent>();

			_source = new CancellationTokenSource();
			_source2 = new CancellationTokenSource();

			_source2.Cancel();
		}

		void Start()
        {
			_agent.speed = _moveSpeed;
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				_agent.isStopped = !_agent.isStopped;
			}

			if(Input.GetKeyDown(KeyCode.Q))
			{
				if (false == _isSkill1State)
				{
					_animator.SetTrigger("OnSkill1");

					_isSkill1State = true;
				}
			}
		}

		#endregion

		#region Public Methods

		public void MoveToPosition(Vector3 destination)
		{
			if ((true == _isSkill1State && false == _isSkillActionMoveLock) ||
				false == _isSkill1State)
			{
				_stopDistance = 0.1f;
				_agent.destination = destination;

				_animator.SetBool("isMove", true);

				if (_moveKind == MoveKind.None)
				{
					_arriveCheckToken = _source.Token;
					CheckIsArriveDestination().Forget();
				}

				_moveKind = MoveKind.MoveToPos;
			}

			ClickEffect.instance.transform.position = _agent.destination + new Vector3(0f, 0.5f, 0f);
		}

		public void ChaseTarget(Transform target)
		{
			if ((true == _isSkill1State && false == _isSkillActionMoveLock) ||
				false == _isSkill1State)
			{
				_chaseTarget = target;
				_stopDistance = _attackRange;

				_animator.SetBool("isMove", true);

				if (_moveKind == MoveKind.None)
				{
					_arriveCheckToken = _source.Token;
					CheckIsArriveDestination().Forget();
				}

				_moveKind = MoveKind.ChaseCharacter;

				_chaseTargetToken = _source.Token;
				UpdateChaseTarget().Forget();
			}

			ClickEffect.instance.transform.position = _chaseTarget.position + new Vector3(0f, 0.5f, 0f);
		}

		public async UniTaskVoid UpdateChaseTarget()
		{
			while (null != _chaseTarget)
			{
				_agent.destination = _chaseTarget.position;

				await UniTask.NextFrame(_chaseTargetToken);
			}
		}

		public async UniTaskVoid CheckIsArriveDestination()
		{
			await UniTask.NextFrame(_arriveCheckToken);

			while (_moveKind != MoveKind.None)
			{
				if (_agent.remainingDistance <= _stopDistance)
				{
					OnMoveEnd();
					break;
				}

				await UniTask.NextFrame(_arriveCheckToken);
			}
		}

		public void OnMoveEnd()
		{
			Debug.Log($"OnMoveEnd : {_moveKind.ToString()}");

			switch (_moveKind)
			{
				case MoveKind.MoveToPos:
					_animator.SetBool("isMove", false);

					break;

				case MoveKind.ChaseCharacter:
					_animator.SetTrigger("OnAttack");

					Vector3 position = _chaseTarget.transform.position;
					position.y = transform.position.y;
					transform.LookAt(position);

					_agent.ResetPath();

					_chaseTargetToken = _source2.Token;
					_chaseTarget = null;

					break;
			}

			_moveKind = MoveKind.None;
			_arriveCheckToken = _source2.Token;
			_animator.SetBool("isMove", false);
		}

		public void OnAnimationEnd()
		{
			_animator.SetTrigger("OnAnimationEnd");

			_isSkill1State = false;
		}

		#endregion
	}
}