using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class OccupationObject : MonoBehaviour
{
    public event Action OnOccupationComplete;

    public readonly float MaxOccupationTime = 3.0f;
    public bool IsOccupated = false;

    public void Interact()
    {
        IsOccupated = true;
        OccupationModel.SetOccuapationState(IsOccupated);
        OccupationModel.MaxOccupationTime = MaxOccupationTime;
        OccupationModel.SetCurOccupationTime(default);
        UpdateOccupationTime().Forget();
    }

    public void CancelInteract()
    {
        IsOccupated = false;
        OccupationModel.SetOccuapationState(IsOccupated);
    }

    public async UniTaskVoid UpdateOccupationTime()
    {
        float elapsedTime = 0;
        
        while (IsOccupated)
        {
            await UniTask.NextFrame();
            elapsedTime += Time.deltaTime;
            if (elapsedTime <= MaxOccupationTime)
            {
                OccupationModel.SetCurOccupationTime(elapsedTime);
            }
            else
            {
                IsOccupated = false;
                OnOccupationComplete?.Invoke();
                OccupationModel.SetOccuapationState(IsOccupated);
            }
        }
    }
}