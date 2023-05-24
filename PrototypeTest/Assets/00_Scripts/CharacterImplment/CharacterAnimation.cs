using CharacterImplement;
using Photon.Pun;
using UnityEngine;
using static CharacterImplement.Character;
using static Unity.VisualScripting.Member;

public class CharacterAnimation : MonoBehaviourPunCallbacks, IPunObservable
{
	public enum TriggerKind
	{
		OnAttack,
		OnSkill1,
		OnAnimationEnd,
		OnInteract,
		MAX
	}

    #region public Fields

    public Character _character;

	public int TriggerData;
	public string[] TriggerTable;

    #endregion

    #region MonoBehaviour CallBacks

    void Awake()
    {
        _character = GetComponentInParent<Character>();
		int triggerCount = (int)TriggerKind.MAX;
		TriggerTable = new string[triggerCount];
		TriggerTable[(int)TriggerKind.OnAttack] = "OnAttack";
		TriggerTable[(int)TriggerKind.OnSkill1] = "OnSkill1";
		TriggerTable[(int)TriggerKind.OnAnimationEnd] = "OnAnimationEnd";
		TriggerTable[(int)TriggerKind.OnInteract] = "OnInteract";
	}

	#endregion

	#region Public Methods

	public void OnAnimationEnd()
	{
		Debug.Log("OnAnimationEnd");
		_character.OnAnimationEnd();
	}

	public void SetTrigger(TriggerKind kind)
	{
		int index = (int)kind;
		int digit = 1 << index;

		TriggerData |= digit;

		GetComponent<Animator>().SetTrigger(TriggerTable[index]);
	}

	#endregion

	#region Photon Implement Methods

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting && photonView.IsMine)
		{
			stream.SendNext(TriggerData);
			TriggerData = 0;
		}
		else
		{
			TriggerData = (int)stream.ReceiveNext();
			if (0 != TriggerData)
			{
				int loopCount = (int)TriggerKind.MAX;
				for (int i = 0; i < loopCount; ++i)
				{
					if ((TriggerData & 1) != 0)
					{
						GetComponent<Animator>().SetTrigger(TriggerTable[i]);
					}

					TriggerData = TriggerData >> 1;
				}
			}
		}
	}

	#endregion
}