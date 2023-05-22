using CharacterImplement;
using UnityEngine;
using static CharacterImplement.Character;
using static Unity.VisualScripting.Member;

public class CharacterAnimation : MonoBehaviour
{
    #region public Fields

    public Character _character;

    #endregion

    #region MonoBehaviour CallBacks

    void Awake()
    {
        _character = GetComponentInParent<Character>();
    }

	#endregion

	#region Public Methods

	public void OnAnimationEnd()
	{
		Debug.Log("OnAnimationEnd");
		_character.OnAnimationEnd();
	}

	#endregion
}