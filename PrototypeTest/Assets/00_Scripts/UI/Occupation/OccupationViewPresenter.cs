using UniRx;

public class OccupationViewPresenter : Presenter
{
    private OccuapationView _occupationView;
    private CompositeDisposable _compositeDisposable = new();

    public override void OnInitialize(View view)
    {
        _occupationView = view as OccuapationView;

        InitializeRx();
    }

    public override void OnRelease()
    {
        _occupationView = default;
        _compositeDisposable.Dispose();
    }

    protected override void OnOccuredUserEvent()
    {
        
    }

    protected override void OnUpdatedModel()
    {
        OccupationModel.IsOccupated.Subscribe(SetActiveOccupationView);
        OccupationModel.CurOccupationTime.Subscribe(UpdateGauge);
        OccupationModel.CurOccupationTime.Subscribe(UpdateRemainTimeSprite);
    }

    private void UpdateGauge(float curOccupationTime)
    {
        _occupationView.ProgressBar.fillAmount = curOccupationTime / OccupationModel.MaxOccupationTime;
    }

    private void UpdateRemainTimeSprite(float curOccupationTime)
    {
        float remainTime = OccupationModel.MaxOccupationTime - curOccupationTime;
        _occupationView.WholeNumber.sprite = _occupationView.Numbers[(int)remainTime];
        int fractionalNumberIndex = (int)((remainTime * 10) % 10);
        _occupationView.FractionalNumber.sprite = _occupationView.Numbers[fractionalNumberIndex];
    }

    private void SetActiveOccupationView(bool isOccupated)
    {
        _occupationView.gameObject.SetActive(isOccupated);
    }
}