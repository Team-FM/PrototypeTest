using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class CharHeadHealthViewPresenter : Presenter
{
    private CharHeadHealthView _headHealthView;
    private CompositeDisposable _compositeDisposable = new();
    public override void OnInitialize(View view)
    {
        _headHealthView = view as CharHeadHealthView;

        InitializeRx();
    }

    public override void OnRelease()
    {
        _headHealthView = default;
        _compositeDisposable.Dispose();
    }

    protected override void OnOccuredUserEvent()
    {
        
    }

    protected override void OnUpdatedModel()
    {
        HealthModel.CurHealth.Subscribe(UpdateHealthGauge).AddTo(_compositeDisposable);
        HealthModel.MaxHealth.Subscribe(UpdateHealthGauge).AddTo(_compositeDisposable);
    }
    private void UpdateHealthGauge(int value)
    {
        _headHealthView.HealthImage.fillAmount = (float)HealthModel.CurHealth.Value / HealthModel.MaxHealth.Value;
        UpdateInstanceHealthGaugeCoroutine().Forget();
    }
    private async UniTaskVoid UpdateInstanceHealthGaugeCoroutine()
    {
        await UniTask.Delay(200);

        float elapsedTime = 0;
        float startAmount = _headHealthView.InstanceHealthImage.fillAmount;
        float endAmount = _headHealthView.HealthImage.fillAmount;

        while (elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;
            _headHealthView.InstanceHealthImage.fillAmount = Mathf.Lerp(startAmount, endAmount, elapsedTime / 0.2f);
            await UniTask.Yield();
        }
    }
}
