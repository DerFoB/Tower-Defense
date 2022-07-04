using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Savegame : Singleton<Savegame>
{
    private const string filename = "Level_Savegame.txt";
    private string path; // must be Application.persistentDataPath to work on Android, this must be set in Awake() method
    public LevelName LevelName { private set; get; } = LevelName.Level_1_1;
    public int Wave { private set; get; } = 0;
    public List<TowerInfo> Towers { private set; get; } = new List<TowerInfo>();
    public int ResearchPoints { private set; get; } = 350;
    public int Health { private set; get; } = 20;
    public List<ItemCountInfo> ItemCounts { get; private set; } = new List<ItemCountInfo>();

    protected override void Awake()
    {
        base.Awake();
        path = Application.persistentDataPath + "/" + filename;
    }

    public void WriteSavegame(LevelName levelName, int wave, List<TowerInfo> towers, int researchPoints, int health, List<ItemCountInfo> itemCounts)
    {
        this.LevelName = levelName;
        this.Wave = wave;
        this.Towers = towers;
        this.ResearchPoints = researchPoints;
        this.Health = health;
        this.ItemCounts = itemCounts;

        if (Util.RunsOnDesktop || Util.RunsOnAndroid)
        {
            try
            {
                string contentLevel = "# Level\n" + levelName.ToString() + "\n\n# Wave\n" + wave + "\n\n# Towers\n";
                string contentMeta = "\n# Research Points\n" + researchPoints + "\n\n# Health\n" + health;
                string contentTower = "";
                foreach (TowerInfo tower in towers)
                {
                    contentTower += tower.gp.X + " " + tower.gp.Y + " " + tower.type + " " + tower.direction + " " + tower.hp + "\n";
                }
                string contentItems = "\n\n# Item Prices\n";
                foreach (var ipi in itemCounts)
                {
                    contentItems += ipi.ItemIndex.ToString() + " " + ipi.Count.ToString() + "\n";
                }

                File.WriteAllText(path, contentLevel + contentTower + contentMeta + contentItems);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
        else
        {
            Debug.Log("Cannot write savegame file on non-desktop builds");
        }
    }

    public void LoadSavegame()
    {
        if (Util.RunsOnDesktop || Util.RunsOnAndroid)
        {
            try
            {
                var textData = File.ReadAllText(path);
                var textLines = textData.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < textLines.Length; i++)
                {
                    textLines[i] = textLines[i].Trim(' ', '\r', '\n').Trim(' ', '\r', '\n');
                }

                Towers = new List<TowerInfo>();
                ItemCounts = new List<ItemCountInfo>();

                int lineNo = 0;
                while (lineNo < textLines.Length)
                {
                    ParseSavegameText(textLines, ref lineNo);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
        else
        {
            Debug.Log("Cannot load savegame file on non-desktop builds");
        }
    }

    private void ParseSavegameText(string[] textLines, ref int lineNo)
    {
        var line = textLines[lineNo++];
        switch (line)
        {
            case "# Level":
                ParseLevel(textLines, ref lineNo);
                break;
            case "# Wave":
                ParseWave(textLines, ref lineNo);
                break;
            case "# Towers":
                ParseTowers(textLines, ref lineNo);
                break;
            case "# Research Points":
                ParseResearchPoints(textLines, ref lineNo);
                break;
            case "# Health":
                ParseHealth(textLines, ref lineNo);
                break;
            case "# Item Prices":
                ParseItemPrices(textLines, ref lineNo);
                break;
            case "":
                break;
            default:
                throw new Exception("Did not expect '" + textLines[lineNo] + "'");
        }
    }

    private void ParseLevel(string[] textLines, ref int lineNo)
    {
        switch (textLines[lineNo++])
        {
            case "Level_1_1":
                this.LevelName = LevelName.Level_1_1;
                break;
            case "Level_1_2":
                this.LevelName = LevelName.Level_1_2;
                break;
            case "Level_1_3":
                this.LevelName = LevelName.Level_1_3;
                break;
            case "Level_2_1":
                this.LevelName = LevelName.Level_2_1;
                break;
            case "Level_2_2":
                this.LevelName = LevelName.Level_2_2;
                break;
            case "Level_2_3":
                this.LevelName = LevelName.Level_2_3;
                break;
            case "Level_3_1":
                this.LevelName = LevelName.Level_3_1;
                break;
            case "Level_3_2":
                this.LevelName = LevelName.Level_3_2;
                break;
            case "Level_3_3":
                this.LevelName = LevelName.Level_3_3;
                break;
            case "Level_4_1":
                this.LevelName = LevelName.Level_4_1;
                break;
            case "Level_4_2":
                this.LevelName = LevelName.Level_4_2;
                break;
            case "Level_4_3":
                this.LevelName = LevelName.Level_4_3;
                break;
            default:
                this.LevelName = LevelName.None;
                break;
        }
    }

    private void ParseWave(string[] textLines, ref int lineNo)
    {
        this.Wave = int.Parse(textLines[lineNo++]);
    }

    private void ParseTowers(string[] textLines, ref int lineNo)
    {
        while (lineNo < textLines.Length && !string.IsNullOrWhiteSpace(textLines[lineNo]) && !textLines[lineNo].StartsWith("#"))
        {
            int[] values = Array.ConvertAll(textLines[lineNo].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), int.Parse);
            if (values.Length != 5)
            {
                throw new Exception("Not parsable to tower: '" + textLines[lineNo] + "'");
            }
            else
            {
                // x, y, type, dir, hp
                Towers.Add(new TowerInfo(new GridPoint(values[0], values[1]), values[2], values[3], values[4]));
            }
            lineNo++;
        }
    }

    private void ParseResearchPoints(string[] textLines, ref int lineNo)
    {
        this.ResearchPoints = int.Parse(textLines[lineNo++]);
    }

    private void ParseHealth(string[] textLines, ref int lineNo)
    {
        this.Health = int.Parse(textLines[lineNo++]);
    }

    private void ParseItemPrices(string[] textLines, ref int lineNo)
    {
        while (lineNo < textLines.Length && !string.IsNullOrWhiteSpace(textLines[lineNo]) && !textLines[lineNo].StartsWith("#"))
        {
            int[] values = Array.ConvertAll(textLines[lineNo].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), int.Parse);
            if (values.Length != 2)
            {
                throw new Exception("Not parsable to item price: '" + textLines[lineNo] + "'");
            }
            else
            {
                // idx, price
                this.ItemCounts.Add(new ItemCountInfo { ItemIndex = values[0], Count = values[1] });
            }
            lineNo++;
        }
    }
}

public class TowerInfo
{
    public GridPoint gp;
    public int type;
    public int direction;
    public int hp;

    public TowerInfo(GridPoint gp, int type, int direction, int hp)
    {
        this.gp = gp;
        this.type = type;
        this.direction = direction;
        this.hp = hp;
    }
}

public struct ItemCountInfo
{
    public int ItemIndex;
    public int Count;
}
