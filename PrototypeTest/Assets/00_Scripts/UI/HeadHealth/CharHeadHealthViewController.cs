using UnityEngine;

public class CharHeadHealthViewController : ViewController
{
    protected override void Initialize()
    {
        View = transform.Find("Char Head Health View").GetComponentAssert<CharHeadHealthView>();
        Presenter = new CharHeadHealthViewPresenter();
    }
}
