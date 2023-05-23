using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class OccupationGage : MonoBehaviour
{
    #region OccupationGage Component
    public Image ProgressBar;
    public Transform OccupationTimer;

    public Image WholeNumber;
    public Image FractionalNumber;

    public Sprite[] Numbers;

    #endregion

    public float Amount;
    public float OccupationTime = 3.0f;
    public float ElapsedTime;
    public float RemainTime;

    public int WholeNumberValue;
    public int FractionalNumberValue;

    public bool OccupationEnd;
    public bool OccupationCancel;

    private void Awake()
    {
        ProgressBar = transform.Find("ProgressBar").GetComponent<Image>();
        OccupationTimer = transform.Find("OccupationTimer");
        WholeNumber = OccupationTimer.Find("WholeNumber").GetComponent<Image>();
        FractionalNumber = OccupationTimer.Find("FractionalNumber").GetComponent<Image>();
    }

    private void OnEnable()
    {
        Amount = 0;
        ElapsedTime = 0;
        OccupationEnd = false;
        FillOccupationGage().Forget();
    }

    public async UniTaskVoid FillOccupationGage()
    {
        while (false == OccupationCancel)
        {
            RemainTime = OccupationTime - ElapsedTime;
            WholeNumber.sprite = Numbers[(int)RemainTime];
            int fractionalNumberIndex = (int)((RemainTime * 10) % 10);
            FractionalNumber.sprite = Numbers[fractionalNumberIndex];
            Amount = ElapsedTime / OccupationTime;
            ProgressBar.fillAmount = Amount;
            ElapsedTime += Time.deltaTime;
            if (ElapsedTime >= OccupationTime)
            {
                OccupationEnd = true;
                this.gameObject.SetActive(false);
                break;
            }
            await UniTask.NextFrame();
        }
    }


}