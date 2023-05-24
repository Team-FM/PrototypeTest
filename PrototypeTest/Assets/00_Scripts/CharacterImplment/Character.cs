using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Threading;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

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

		public enum TargetKind
		{
			None,
			InteractObject,
			Enemy
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
		public float InteractRange = 1f;

		public float _stopDistance = 0f;
		public Transform _chaseTarget;
		public Transform chaseTarget
		{
			get => _chaseTarget;
			set
			{
				if (value == _chaseTarget)
				{
					return;
				}

				if( null != _chaseTarget)
				{
					if (_chaseTarget.gameObject.layer == LayerMask.NameToLayer("InteractObject"))
					{
						_chaseTarget.GetComponent<OccupationObject>().CancelInteract();

						_chaseTarget.GetComponent<OccupationObject>().OnOccupationComplete -= SuccessOccupation;
						_chaseTarget.GetComponent<OccupationObject>().OnOccupationComplete += SuccessOccupation;
					}
				}

				_chaseTarget = value;
				if (null != _chaseTarget)
				{
					if (_chaseTarget.gameObject.layer == LayerMask.NameToLayer("Character"))
					{
						ChaseTargetKind = TargetKind.Enemy;
						_stopDistance = _attackRange;
					}
					else if (_chaseTarget.gameObject.layer == LayerMask.NameToLayer("InteractObject"))
					{
						ChaseTargetKind = TargetKind.InteractObject;
						_stopDistance = InteractRange;
					}
					else
					{
						ChaseTargetKind = TargetKind.None;
					}
				}
				else
				{
					ChaseTargetKind = TargetKind.None;
				}
			}
		}
		public MoveKind _moveKind;
		public TargetKind ChaseTargetKind;

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
				CharacterStatusModel.SetCurHealth(CurHP);
				CharacterStatusModel.SetMaxHealth(MaxHP);
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
				_agent.isStopped = false;
				_stopDistance = 0.1f;
				_agent.destination = destination;

				_animator.SetBool("isMove", true);
				chaseTarget = null;

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
			if (null == target)
			{
				return;
			}

			if ((true == _isSkill1State && false == _isSkillActionMoveLock) ||
				false == _isSkill1State)
			{
				chaseTarget = target;

				_agent.isStopped = false;
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
			while (null != _chaseTarget && _moveKind == MoveKind.ChaseCharacter)
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
					switch(ChaseTargetKind)
					{
						case TargetKind.InteractObject:
							{
								CharacterAnimComponent.SetTrigger(CharacterAnimation.TriggerKind.OnInteract);

								Vector3 position = _chaseTarget.transform.position;
								position.y = transform.position.y;
								transform.LookAt(position);

								_chaseTarget.GetComponent<OccupationObject>().Interact();
								_chaseTarget.GetComponent<OccupationObject>().OnOccupationComplete -= SuccessOccupation;
								_chaseTarget.GetComponent<OccupationObject>().OnOccupationComplete += SuccessOccupation;
							}

							break;
						case TargetKind.Enemy:
							{
								CharacterAnimComponent.SetTrigger(CharacterAnimation.TriggerKind.OnAttack);

								Vector3 position = _chaseTarget.transform.position;
								position.y = transform.position.y;
								transform.LookAt(position);

								if (null != _chaseTarget.GetComponent<Character>())
								{
									_chaseTarget.GetComponent<Character>().TakeHit(AtkStat);
								}

								_chaseTargetToken = _source2.Token;
								chaseTarget = null;
							}

							break;
					}

					break;
			}

			_agent.isStopped = true;

			_moveKind = MoveKind.None;
			_arriveCheckToken = _source2.Token;
			_animator.SetBool("isMove", false);
		}

		private void SuccessOccupation()
		{
			CharacterAnimComponent.SetTrigger(CharacterAnimation.TriggerKind.OnAnimationEnd);
			Debug.Log("SuccessOccupation");
		}

		public void TakeHit(int damage)
		{
            //if (true == photonView.IsMine)
            {
				CurHP = System.Math.Max(CurHP - damage, 0);
                photonView.RPC("UpdateCurHPRPC", RpcTarget.All, CurHP);
            }
		}

		[PunRPC]
		public void UpdateCurHPRPC(int newHP)
		{
			if (photonView.IsMine)
			{
				CharacterStatusModel.SetCurHealth(newHP);
			}
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