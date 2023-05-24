using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class CharacterHeaderMovement : MonoBehaviour
{
    public Transform TargetTransform;
    public Vector3 Offset = new(0, 2.2f, 0);

    private void Start()
    {
        FollowTarget();
    }
    private void FollowTarget()
    {
        this.LateUpdateAsObservable().Subscribe(Follow);
    }
    private void Follow(Unit unit)
    {
        transform.position = TargetTransform.position + Offset;
    }
}