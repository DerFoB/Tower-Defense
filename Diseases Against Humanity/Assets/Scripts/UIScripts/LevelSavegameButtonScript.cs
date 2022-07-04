using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSavegameButtonScript : Singleton<LevelSavegameButtonScript>
{
    [SerializeField]
    private Sprite[] _LevelSprites;

    [SerializeField]
    protected Button SavegameButton;

    [SerializeField]
    private TextMeshProUGUI LevelDisplay;


    public void UpdateImage(LevelName levelName)
    {
        switch (levelName)
        {
            case LevelName.Level_1_1:
                SavegameButton.GetComponent<Image>().sprite = _LevelSprites[0];
                LevelDisplay.text = "1.1";
                break;
            case LevelName.Level_1_2:
                SavegameButton.GetComponent<Image>().sprite = _LevelSprites[1];
                LevelDisplay.text = "1.2";
                break;
            case LevelName.Level_1_3:
                SavegameButton.GetComponent<Image>().sprite = _LevelSprites[2];
                LevelDisplay.text = "1.3";
                break;
            case LevelName.Level_2_1:
                SavegameButton.GetComponent<Image>().sprite = _LevelSprites[3];
                LevelDisplay.text = "2.1";
                break;
            case LevelName.Level_2_2:
                SavegameButton.GetComponent<Image>().sprite = _LevelSprites[4];
                LevelDisplay.text = "2.2";
                break;
            case LevelName.Level_2_3:
                SavegameButton.GetComponent<Image>().sprite = _LevelSprites[5];
                LevelDisplay.text = "2.3";
                break;
            case LevelName.Level_3_1:
                SavegameButton.GetComponent<Image>().sprite = _LevelSprites[6];
                LevelDisplay.text = "3.1";
                break;
            case LevelName.Level_3_2:
                SavegameButton.GetComponent<Image>().sprite = _LevelSprites[7];
                LevelDisplay.text = "3.2";
                break;
            case LevelName.Level_3_3:
                SavegameButton.GetComponent<Image>().sprite = _LevelSprites[8];
                LevelDisplay.text = "3.3";
                break;
            case LevelName.Level_4_1:
                SavegameButton.GetComponent<Image>().sprite = _LevelSprites[9];
                LevelDisplay.text = "4.1";
                break;
            case LevelName.Level_4_2:
                SavegameButton.GetComponent<Image>().sprite = _LevelSprites[10];
                LevelDisplay.text = "4.2";
                break;
            case LevelName.Level_4_3:
                SavegameButton.GetComponent<Image>().sprite = _LevelSprites[11];
                LevelDisplay.text = "4.3";
                break;
            default:
                SavegameButton.GetComponent<Image>().sprite = _LevelSprites[0];
                LevelDisplay.text = "1.1";
                break;
        }
    }
}
