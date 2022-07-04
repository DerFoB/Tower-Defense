using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressScript : MonoBehaviour
{
    [SerializeField]
    public Slider LevelProgressSlider;

    [SerializeField]
    private float ProgressUpdateTime = 0.5f;

    [SerializeField]
    private GameObject WaveMarker;

    private Vector2 ScreenResolution;

    public void StartUpdating()
    {
        InvokeRepeating(nameof(UpdateProgress), 0f, this.ProgressUpdateTime);
        // Update the progress bar once (e.g. when game is paused, InvokeRepeating would not get fired)
        UpdateProgress();
        this.ScreenResolution = new Vector2(Screen.width, Screen.height);
        AddWaveMarkers();
    }
    public void StopUpdating()
    {
        CancelInvoke(nameof(UpdateProgress));
        DestroyWaveMarkers();
    }


    private void AddWaveMarkers()
    {
        var waves = WaveManager.GetInstance().WaveCount;

        var parentRect = (this.WaveMarker.transform.parent.transform as RectTransform).rect;

        float y = -parentRect.height * 0.7f;
        float xIncrease = parentRect.width / waves;
        (this.WaveMarker.transform as RectTransform).localPosition = new Vector3(parentRect.xMin, y, 0);
        for (int i = 0; i < waves; i++)
        {
            var wm = Instantiate(this.WaveMarker, this.WaveMarker.transform.parent);
            wm.transform.localPosition = new Vector3(parentRect.xMin + (i + 1) * xIncrease, y, 0);
            wm.SetActive(true);
            wm.GetComponent<RectTransform>().sizeDelta = i % 5 == 4 ? new Vector2(5, 20) : new Vector2(5, 10);
        }
    }

    private void DestroyWaveMarkers()
    {
        for (int i = 0; i < this.WaveMarker.transform.parent.childCount; i++)
        {
            var child = this.WaveMarker.transform.parent.GetChild(i);
            if (child != this.WaveMarker.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void UpdateProgress()
    {
        LevelProgressSlider.value = WaveManager.GetInstance().CalculateWaveProgress();
    }

    private void Update()
    {
        if (this.ScreenResolution.x != Screen.width || this.ScreenResolution.y != Screen.height)
        {
            this.ScreenResolution = new Vector2(Screen.width, Screen.height);
            DestroyWaveMarkers();
            AddWaveMarkers();
        }
    }
}
