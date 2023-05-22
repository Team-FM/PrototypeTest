using UnityEngine;

public class FollowCam : MonoBehaviour
{
    #region Public Fields

    public Transform _target;
	public Vector3 _offsetPos;

	#endregion

	#region MonoBehaviour CallBacks

	private void Start()
	{
		_offsetPos = transform.position - _target.position;
	}

	private void LateUpdate()
	{
		transform.position = _target.position + _offsetPos;
	}

	#endregion
}