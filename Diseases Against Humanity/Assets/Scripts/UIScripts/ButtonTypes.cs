public class ButtonTypes
{
    /// <summary>
    /// Here you can specify the types for the buttons.
    /// The numbering is done according to the following scheme:
    /// - Hundreds digit for superordinate group
    /// - Tens digit for subordinate group
    /// - Unit digit for specific function
    /// </summary>
    public enum ButtonType : int
    {
        NoFunction = 0,
        ExitApplication = 1,
        OpenLevelSelection = 100,
        CloseLevelSelection = 101,
        NextLevelSelection = 102,
        PreviousLevelSelection = 103,
        StartLevel_Savegame = 110,
        StartLevel_1_1 = 111,
        StartLevel_1_2 = 112,
        StartLevel_1_3 = 113,
        StartLevel_2_1 = 121,
        StartLevel_2_2 = 122,
        StartLevel_2_3 = 123,
        StartLevel_3_1 = 131,
        StartLevel_3_2 = 132,
        StartLevel_3_3 = 133,
        StartLevel_4_1 = 141,
        StartLevel_4_2 = 142,
        StartLevel_4_3 = 143,
        OpenSettingsMenu = 200,
        CloseSettingsMenu = 201,
        ToggleFullscreen = 202,
        OpenMainMenu = 300,
        BuildTower0 = 400,
        BuildTower1 = 401,
        BuildTower2 = 402,
        BuildTower3 = 403,
        BuildTower4 = 404,
        BuildTower5 = 405,
        UseItem0 = 410,
        UseItem1 = 411,
        UseItem2 = 412,
        DemolishTower = 420,
        RotateTower = 421,
        Timelapse = 500,
        OpenHelpScreen = 501,
        CloseHelpScreen = 502,
        RetryLevel = 600,
    }
}
