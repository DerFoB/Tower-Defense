using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CanvasTypes;

public class GameOverCanvasController : Singleton<GameOverCanvasController>, ICanvasController
{
    public CanvasType CanvasType => CanvasType.GameOverScreen;

    [SerializeField]
    private GameObject WastedImage;

    [SerializeField]
    private GameObject WinImage;

    [SerializeField]
    private GameObject LoseImage;

    [SerializeField]
    private TextMeshProUGUI PointsText;


    private const float CountingTime = 4f;
    private float RemainingCountingTime = 0f;

    public void ConfigGameOverScreen(bool isWin, int points)
    {
        this.gameObject.SetActive(true);
        if (isWin)
        {
            this.WinImage.SetActive(true);
            this.LoseImage.SetActive(false);
            this.WastedImage.SetActive(false);
            this.PointsText.enabled = true;
            StartCoroutine(CountToValue(0f, points));
        }
        else
        {
            this.WinImage.SetActive(false);
            this.LoseImage.SetActive(false);
            this.WastedImage.SetActive(true);
            this.PointsText.enabled = false;
            StartCoroutine(CountToValue(4.5f, points));
        }
    }

    private IEnumerator CountToValue(float waitingTime, int value)
    {
        this.RemainingCountingTime = CountingTime;
        yield return new WaitForSecondsRealtime(waitingTime);

        this.WastedImage.SetActive(false);
        this.LoseImage.SetActive(value < 100);
        this.WinImage.SetActive(value >= 100);
        this.PointsText.enabled = true;

        int lastValue = 0;
        while (this.RemainingCountingTime > 0f)
        {
            int v = Mathf.RoundToInt(Mathf.SmoothStep(0, value, Mathf.Pow(1 - (this.RemainingCountingTime / CountingTime), 0.2f)));
            if (v != lastValue)
            {
                SoundManager.GetInstance().PlaySFX(Sounds.Sound.Canon, true);
            }
            lastValue = v;
            this.PointsText.text = v.ToString() + "%";
            this.RemainingCountingTime -= Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
