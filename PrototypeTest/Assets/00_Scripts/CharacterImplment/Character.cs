using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace CharacterImplement
{
	public class Character : MonoBehaviourPunCallbacks, IPunObservable
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

		public CharacterAnimation CharacterAnimComponent;
		public Animator _animator;
		public NavMeshAgent _agent;

		public int MaxHP;
		public int CurHP;
		public float _moveSpeed = 0f;
        public float _attackRange = 0f;
		public int AtkStat;

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
			CharacterAnimComponent = GetComponentInChildren<CharacterAnimation>();

			_source = new CancellationTokenSource();
			_source2 = new CancellationTokenSource();

			_source2.Cancel();

			CurHP = MaxHP;

			if (true == photonView.IsMine)
			{
				HealthModel.SetCurHealth(CurHP);
				HealthModel.SetMaxHealth(MaxHP);
			}
        }

		void Start()
        {
			_agent.speed = _moveSpeed;
		}

		private void Update()
		{
			if (true == photonView.IsMine)
			{
				if (Input.GetKeyDown(KeyCode.Q))
				{
					if (false == _isSkill1State)
					{
						CharacterAnimComponent.SetTrigger(CharacterAnimation.TriggerKind.OnSkill1);

						_isSkill1State = true;
					}
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
				_chaseTarget = null;

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
				case MoveKind.ChaseCharacter:
					CharacterAnimComponent.SetTrigger(CharacterAnimation.TriggerKind.OnAttack);

					Vector3 position = _chaseTarget.transform.position;
					position.y = transform.position.y;
					transform.LookAt(position);

					_agent.ResetPath();

					_chaseTarget.GetComponent<Character>().TakeHit(AtkStat);

					_chaseTargetToken = _source2.Token;
					_chaseTarget = null;

					break;
			}

			_moveKind = MoveKind.None;
			_arriveCheckToken = _source2.Token;
			_animator.SetBool("isMove", false);
		}

		public void TakeHit(int damage)
		{
            if (true == photonView.IsMine)
            {
                CurHP -= damage;
                HealthModel.SetCurHealth(CurHP);
                photonView.RPC("UpdateCurHPRPC", RpcTarget.All, CurHP);
            }
		}

		[PunRPC]
		public void UpdateCurHPRPC(int newHP)
		{
			CurHP = newHP;
		}

		public void OnAnimationEnd()
		{
			CharacterAnimComponent.SetTrigger(CharacterAnimation.TriggerKind.OnAnimationEnd);

			_isSkill1State = false;
		}

		#endregion

		#region Photon Implement Methods

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			
		}

		#endregion
	}
}