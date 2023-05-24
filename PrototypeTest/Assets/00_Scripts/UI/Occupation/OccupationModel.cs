using UniRx;
using UnityEngine;

public static class OccupationModel
{
    public static ReactiveProperty<bool> IsOccupated = new();
    public static ReactiveProperty<float> CurOccupationTime = new();
    public static float MaxOccupationTime { get; set; }

    public static void SetOccuapationState(bool curState)
    {
        IsOccupated.Value = curState;
    }

    public static void SetCurOccupationTime(float elapsedTime)
    {
        CurOccupationTime.Value = elapsedTime;
    }
}