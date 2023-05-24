using UnityEngine;
using UnityEngine.UI;

public class OccuapationView : View
{
    #region OccupationGage Component
    public Image ProgressBar { get; private set; }
    public Image WholeNumber { get; private set; }
    public Image FractionalNumber { get; private set; }
    public Sprite[] Numbers;

    #endregion

    private void Awake()
    {
        ProgressBar = transform.Find("ProgressBar").GetComponentAssert<Image>();
        Transform timer = transform.Find("OccupationTimer");
        WholeNumber = timer.Find("WholeNumber").GetComponentAssert<Image>();
        FractionalNumber = timer.Find("FractionalNumber").GetComponentAssert<Image>();
    }
}