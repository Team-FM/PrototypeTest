using UnityEngine;

[RequireComponent(typeof(Canvas))]
public abstract class ViewController : MonoBehaviour
{
    #region Properties
    protected View View { get; set; }
    protected Presenter Presenter { get; set; }
    #endregion

    #region MonoBehaviour CallBacks
    private void Awake() => Initialize();
    private void Start() => Presenter.OnInitialize(View);
    private void OnDestroy() => Presenter.OnRelease();
    #endregion

    #region Private Methods
    /// <summary>
    /// View와 Presenter를 초기화하기 위한 함수입니다. Awake()에서 호출됩니다.
    /// </summary>
    protected abstract void Initialize();
    #endregion
}