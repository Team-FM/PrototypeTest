public class CharacterStatusViewController : ViewController
{
    protected override void Initialize()
    {
        View = transform.Find("Character Status View").GetComponentAssert<CharacterStatusView>();
        Presenter = new CharacterStatusViewPresenter();
    }
}