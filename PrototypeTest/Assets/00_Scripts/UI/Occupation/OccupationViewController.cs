using UnityEngine;

public class OccupationViewController : ViewController
{
    protected override void Initialize()
    {
        View = transform.Find("OccupationView").GetComponentAssert<OccuapationView>();
        Presenter = new OccupationViewPresenter();
    }
}