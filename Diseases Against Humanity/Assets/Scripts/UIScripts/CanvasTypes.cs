using System;

public class CanvasTypes
{
    /// <summary>
    /// Here you can specify the types for the canvases.
    /// Note: This is a flag enum!
    /// </summary>
    [Flags]
    public enum CanvasType : int
    {
        None = int.MinValue,
        MainMenuBackground = 1 << 0,
        MainMenu = 1 << 1,
        LevelSelection = 1 << 2,
        SettingsMenu = 1 << 3,
        BuildMenu = 1 << 4,
        GameOverScreen = 1 << 5,
        HelpScreen = 1 << 6,
    }
}
