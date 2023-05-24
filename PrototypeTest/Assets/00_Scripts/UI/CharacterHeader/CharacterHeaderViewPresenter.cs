using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class CharacterHeaderViewPresenter : Presenter
{
    private CharacterHeaderView _headHealthView;
    private CompositeDisposable _compositeDisposable = new();
    public override void OnInitialize(View view)
    {
        _headHealthView = view as CharacterHeaderView;

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
        CharacterStatusModel.CurHealth.Subscribe(UpdateHealthGauge).AddTo(_compositeDisposable);
        CharacterStatusModel.MaxHealth.Subscribe(UpdateHealthGauge).AddTo(_compositeDisposable);
        CharacterStatusModel.CurLevel.Subscribe(UpdateLevel).AddTo(_compositeDisposable);
        CharacterStatusModel.NickName.Subscribe(UpdateNickName).AddTo(_compositeDisposable);
    }
    private void UpdateHealthGauge(int value)
    {
        _headHealthView.HealthImage.fillAmount = (float)CharacterStatusModel.CurHealth.Value / CharacterStatusModel.MaxHealth.Value;
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
    private void UpdateLevel(int value)
    {
        _headHealthView.LevelText.text = value.ToString();
    }
    private void UpdateNickName(string nickName)
    {
        _headHealthView.NickNameText.text = nickName;
    }
}
