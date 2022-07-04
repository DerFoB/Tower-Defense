using UnityEngine;
using UnityEngine.UI;
using static Sounds;

public class TimelapseButtonController : ButtonController
{
    [SerializeField]
    private Image Image;

    [SerializeField]
    private Sprite PauseSprite;

    [SerializeField]
    private Sprite[] PlaySprites;


    protected override void Start()
    {
        base.Start();
        GameManager.GetInstance().GameSpeedChanged += GameManager_GameSpeedChanged;
        // Event does not get fired when first level is loaded because binding is too late
        // => Quick Fix: Call Eventhandler manually
        GameManager_GameSpeedChanged(GameManager.GetInstance().GameSpeed);
    }

    private void GameManager_GameSpeedChanged(GameSpeedEnum newValue)
    {
        switch (newValue)
        {
            case GameSpeedEnum.Pause:
                this.Image.sprite = PauseSprite;
                break;
            case GameSpeedEnum.Play1:
                this.Image.sprite = PlaySprites[0];
                break;
            case GameSpeedEnum.Play2:
                this.Image.sprite = PlaySprites[1];
                break;
            case GameSpeedEnum.Play4:
                this.Image.sprite = PlaySprites[2];
                break;
            default:
                break;
        }
    }

    protected override void OnButtonClicked()
    {
        if (CanvasManager.GetInstance().IsSettingsMenuActive()) { return; }
        if (this.ButtonType == ButtonTypes.ButtonType.Timelapse)
        {
                SoundManager.GetInstance().PlaySFX(Sound.ButtonClick);
                GameManager.GetInstance().ChangeGameSpeed(null);
        }

        base.OnButtonClicked();
    }
}

public enum GameSpeedEnum
{
    Pause,
    Play1,
    Play2,
    Play4
}
