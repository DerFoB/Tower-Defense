using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static ButtonTypes;
using static CanvasTypes;
using static Sounds;

/// <summary>
/// This class has to be added to every button in the UI to add its functionality.
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonController : MonoBehaviour
{
    [SerializeField]
    protected ButtonType ButtonType;
    protected Button ThisButton;

    /// <summary>
    /// This method gets called once at the beginning for initializing purposes.
    /// </summary>
    protected virtual void Start()
    {
        this.ThisButton = GetComponent<Button>();
        this.ThisButton.onClick.AddListener(OnButtonClicked);
    }

    /// <summary>
    /// This method is triggered by each button when it is clicked. The button can be determined by its ButtonType.
    /// </summary>
    protected virtual void OnButtonClicked()
    {
        switch (this.ButtonType)
        {
            case ButtonType.OpenLevelSelection:
                if (CanvasManager.GetInstance().IsSettingsMenuActive()) { return; }
                Savegame.GetInstance().LoadSavegame();
                LevelSavegameButtonScript.GetInstance().UpdateImage(Savegame.GetInstance().LevelName);
                CanvasManager.GetInstance().SwitchCanvas(CanvasType.MainMenuBackground | CanvasType.LevelSelection);
                break;

            case ButtonType.CloseLevelSelection:
                CanvasManager.GetInstance().SwitchCanvas(CanvasType.MainMenuBackground | CanvasType.MainMenu);
                break;

            case ButtonType.NextLevelSelection:
                LevelSelectionController.GetInstance().Next();
                break;
            case ButtonType.PreviousLevelSelection:
                LevelSelectionController.GetInstance().Previous();
                break;

            case ButtonType.StartLevel_1_1:
            case ButtonType.StartLevel_1_2:
            case ButtonType.StartLevel_1_3:
            case ButtonType.StartLevel_2_1:
            case ButtonType.StartLevel_2_2:
            case ButtonType.StartLevel_2_3:
            case ButtonType.StartLevel_3_1:
            case ButtonType.StartLevel_3_2:
            case ButtonType.StartLevel_3_3:
            case ButtonType.StartLevel_4_1:
            case ButtonType.StartLevel_4_2:
            case ButtonType.StartLevel_4_3:
            case ButtonType.StartLevel_Savegame:
                StartLevel();
                break;

            case ButtonType.RetryLevel:
                var lvl = GameManager.GetInstance().CurrentLevel;
                SoundManager.GetInstance().StopBackgroundMusic();
                GameManager.GetInstance().ExitLevel();
                StartLevel(lvl);
                break;

            case ButtonType.OpenSettingsMenu:
                if (CanvasManager.GetInstance().IsSettingsMenuActive()) { return; }
                if (CanvasManager.GetInstance().GetActiveCanvasTypes().Contains(CanvasType.BuildMenu))
                {
                    CameraMovement.GetInstance().SetIsRunningFlag(false);
                    GameManager.GetInstance().ChangeGameSpeed(GameSpeedEnum.Pause);
                    GameManager.GetInstance().AbortPickTower();
                    CanvasManager.GetInstance().SwitchCanvas(CanvasType.BuildMenu | CanvasType.SettingsMenu);
                }
                else if (CanvasManager.GetInstance().GetActiveCanvasTypes().Contains(CanvasType.MainMenuBackground))
                {
                    CanvasManager.GetInstance().SwitchCanvas(CanvasType.MainMenuBackground | CanvasType.SettingsMenu);
                }

                break;

            case ButtonType.CloseSettingsMenu:
                if (CanvasManager.GetInstance().GetActiveCanvasTypes().Contains(CanvasType.BuildMenu))
                {
                    CameraMovement.GetInstance().SetIsRunningFlag(true);
                    GameManager.GetInstance().ResumeWithLastGameSpeed();
                    CanvasManager.GetInstance().SwitchCanvas(CanvasType.BuildMenu);
                }
                else if (CanvasManager.GetInstance().GetActiveCanvasTypes().Contains(CanvasType.MainMenuBackground))
                {
                    CanvasManager.GetInstance().SwitchCanvas(CanvasType.MainMenuBackground | CanvasType.MainMenu);
                }

                break;

            case ButtonType.OpenHelpScreen:
                if (CanvasManager.GetInstance().IsSettingsMenuActive()) { return; }
                if (CanvasManager.GetInstance().GetActiveCanvasTypes().Contains(CanvasType.BuildMenu))
                {
                    CameraMovement.GetInstance().SetIsRunningFlag(false);
                    GameManager.GetInstance().ChangeGameSpeed(GameSpeedEnum.Pause);
                    GameManager.GetInstance().AbortPickTower();
                    CanvasManager.GetInstance().SwitchCanvas(CanvasType.BuildMenu | CanvasType.HelpScreen);
                }
                else if (CanvasManager.GetInstance().GetActiveCanvasTypes().Contains(CanvasType.MainMenuBackground))
                {
                    CanvasManager.GetInstance().SwitchCanvas(CanvasType.MainMenuBackground | CanvasType.HelpScreen);
                }

                break;

            case ButtonType.CloseHelpScreen:
                if (CanvasManager.GetInstance().GetActiveCanvasTypes().Contains(CanvasType.BuildMenu))
                {
                    CameraMovement.GetInstance().SetIsRunningFlag(true);
                    GameManager.GetInstance().ResumeWithLastGameSpeed();
                    CanvasManager.GetInstance().SwitchCanvas(CanvasType.BuildMenu);
                }
                else if (CanvasManager.GetInstance().GetActiveCanvasTypes().Contains(CanvasType.MainMenuBackground))
                {
                    CanvasManager.GetInstance().SwitchCanvas(CanvasType.MainMenuBackground | CanvasType.MainMenu);
                }

                break;

            case ButtonType.ToggleFullscreen:
                if (!Screen.fullScreen)
                {
                    Screen.SetResolution(Screen.resolutions.Last().width, Screen.resolutions.Last().height, FullScreenMode.FullScreenWindow);
                }
                else
                {
                    Screen.fullScreen = !Screen.fullScreen;
                }

                break;

            case ButtonType.OpenMainMenu:
                if (CanvasManager.GetInstance().GetActiveCanvasTypes().Contains(CanvasType.GameOverScreen))
                {
                    GameManager.GetInstance().ExitLevel();
                    SoundManager.GetInstance().PlayMenuBackgroundMusic();
                    CanvasManager.GetInstance().SwitchCanvas(CanvasType.MainMenuBackground | CanvasType.MainMenu);
                }
                else if (CanvasManager.GetInstance().GetActiveCanvasTypes().Contains(CanvasType.BuildMenu))
                {
                    // TODO: Maybe save game before exit.
                    GameManager.GetInstance().ExitLevel();
                    SoundManager.GetInstance().PlayMenuBackgroundMusic();
                    CanvasManager.GetInstance().SwitchCanvas(CanvasType.MainMenuBackground | CanvasType.MainMenu);
                }
                else if (CanvasManager.GetInstance().GetActiveCanvasTypes().Contains(CanvasType.MainMenuBackground))
                {
                    CanvasManager.GetInstance().SwitchCanvas(CanvasType.MainMenuBackground | CanvasType.MainMenu);
                }

                break;

            case ButtonType.ExitApplication:
                if (CanvasManager.GetInstance().IsSettingsMenuActive()) { return; }
                // TODO: Confirmation dialog
                Application.Quit();
                break;

            case ButtonType.DemolishTower:
                if (CanvasManager.GetInstance().IsSettingsMenuActive()) { return; }
                GameManager.GetInstance().PickDemolishTower();
                return; // return instead of break to prevent playing the click sound

            case ButtonType.RotateTower:
                if (!GameManager.GetInstance().IsRotatingSelection) { return; }
                GameManager.GetInstance().RotatePickedTower(true);
                return; // return instead of break to prevent playing the click sound

            case ButtonType.NoFunction:
                Debug.LogWarning(string.Format("Class '{0}', Method '{1}': {2}", nameof(ButtonController), nameof(OnButtonClicked), "The pressed button has no declared function!"));
                return;

            default:
                break;
        }

        SoundManager.GetInstance().PlaySFX(Sound.ButtonClick);
    }

    private void StartLevel(LevelName level = LevelName.None)
    {
        if (level == LevelName.None)
        {
            level = Util.GetLevelName(ButtonType);
        }

        SoundManager.GetInstance().StopBackgroundMusic();
        LevelBuilder.GetInstance().StartLevel(level);
        CameraMovement.GetInstance().SetIsRunningFlag(true);
        CanvasManager.GetInstance().SwitchCanvas(CanvasType.BuildMenu);

        switch (level)
        {
            case LevelName.Level_1_1:
            case LevelName.Level_1_2:
            case LevelName.Level_1_3:
            case LevelName.Level_4_1:
            case LevelName.Level_4_2:
            case LevelName.Level_4_3:
                SoundManager.GetInstance().PlayForestBackgroundMusic();
                break;
            case LevelName.Level_2_1:
            case LevelName.Level_2_2:
            case LevelName.Level_2_3:
                SoundManager.GetInstance().PlayVillageBackgroundMusic();
                break;
            case LevelName.Level_3_1:
            case LevelName.Level_3_2:
            case LevelName.Level_3_3:
                SoundManager.GetInstance().PlayCityBackgroundMusic();
                break;
        }
    }
}
