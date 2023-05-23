public class HealthViewController : ViewController
{
    protected override void Initialize()
    {
        View = transform.Find("HealthView").GetComponentAssert<HealthView>();
        Presenter = new HealthViewPresenter();
    }
}