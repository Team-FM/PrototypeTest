using UniRx;

public class CharacterStatusViewPresenter : Presenter
{
    private CharacterStatusView _healthView;
    private CompositeDisposable _compositeDisposable = new();

    public override void OnInitialize(View view)
    {
        _healthView = view as CharacterStatusView;

        InitializeRx();
    }

    public override void OnRelease()
    {
        _healthView = default;
        _compositeDisposable.Dispose();
    }

    protected override void OnOccuredUserEvent()
    {
        
    }

    protected override void OnUpdatedModel()
    {
        CharacterStatusModel.CurHealth.Subscribe(UpdateGauge).AddTo(_compositeDisposable);
        CharacterStatusModel.CurHealth.Subscribe(UpdateCurText).AddTo(_compositeDisposable);
        CharacterStatusModel.MaxHealth.Subscribe(UpdateGauge).AddTo(_compositeDisposable);
        CharacterStatusModel.MaxHealth.Subscribe(UpdateMaxText).AddTo(_compositeDisposable);
    }
    private void UpdateGauge(int value)
    {
        _healthView.HealthImage.fillAmount = (float)CharacterStatusModel.CurHealth.Value / CharacterStatusModel.MaxHealth.Value;
    }
    private void UpdateCurText(int value)
    {
        _healthView.CurHealthText.text = value.ToString();
    }
    private void UpdateMaxText(int value)
    {
        _healthView.MaxHealthText.text = value.ToString();
    }
}