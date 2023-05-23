using UniRx;

public class HealthViewPresenter : Presenter
{
    private HealthView _healthView;
    private CompositeDisposable _compositeDisposable = new();

    public override void OnInitialize(View view)
    {
        _healthView = view as HealthView;

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
        HealthModel.CurHealth.Subscribe(UpdateGauge).AddTo(_compositeDisposable);
        HealthModel.CurHealth.Subscribe(UpdateCurText).AddTo(_compositeDisposable);
        HealthModel.MaxHealth.Subscribe(UpdateGauge).AddTo(_compositeDisposable);
        HealthModel.MaxHealth.Subscribe(UpdateMaxText).AddTo(_compositeDisposable);
    }
    private void UpdateGauge(int value)
    {
        _healthView.HealthImage.fillAmount = (float)HealthModel.CurHealth.Value / HealthModel.MaxHealth.Value;
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