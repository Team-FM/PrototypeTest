using UnityEngine;

public class CharacterHeaderViewController : ViewController
{
    protected override void Initialize()
    {
        View = transform.Find("Character Header View").GetComponentAssert<CharacterHeaderView>();
        Presenter = new CharacterHeaderViewPresenter();
    }
}
