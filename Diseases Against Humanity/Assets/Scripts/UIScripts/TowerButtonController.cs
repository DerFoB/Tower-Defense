using UnityEngine;
using static ButtonTypes;

public class TowerButtonController : ButtonController
{
    protected override void OnButtonClicked()
    {
        if (CanvasManager.GetInstance().IsSettingsMenuActive()) { return; }

        switch (this.ButtonType)
        {
            case ButtonType.BuildTower0:
                GameManager.GetInstance().PickTower(0);
                break;
            case ButtonType.BuildTower1:
                GameManager.GetInstance().PickTower(1);
                break;
            case ButtonType.BuildTower2:
                GameManager.GetInstance().PickTower(2);
                break;
            case ButtonType.BuildTower3:
                GameManager.GetInstance().PickTower(3);
                break;
            case ButtonType.BuildTower4:
                GameManager.GetInstance().PickTower(4);
                break;
            case ButtonType.BuildTower5:
                GameManager.GetInstance().PickTower(5);
                break;
            case ButtonType.UseItem0:
                GameManager.GetInstance().PickTower(6);
                break;
            case ButtonType.UseItem1:
                GameManager.GetInstance().PickTower(7);
                break;
            case ButtonType.UseItem2:
                GameManager.GetInstance().BuyHealthKit();
                break;
            default:
                Debug.LogWarning(string.Format("Class '{0}', Method '{1}': {2}", nameof(TowerButtonController), nameof(OnButtonClicked), "The pressed button has no declared function!"));
                break;
        }
    }
}
