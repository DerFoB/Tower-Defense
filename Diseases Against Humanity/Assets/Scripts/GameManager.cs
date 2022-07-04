using Assets.Scripts.DefenseScripts;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CanvasTypes;
using static Sounds;

public class GameManager : Singleton<GameManager>
{
    private PointsInfo _PointsInfo = null;

    private int _ResearchPoints = 300;
    public int ResearchPoints { get => _ResearchPoints; set { this._ResearchPoints = value; UpdateStatusPointDisplay(value, this.ResearchPointsDisplay); } }

    [SerializeField]
    private int _HealthPoints = 100;
    public int HealthPoints { get => _HealthPoints; set { this._HealthPoints = value; UpdateStatusPointDisplay(value, this.HealthPointsDisplay); LevelBuilder.GetInstance().EndTile.ShowSuitableCrowd(value); } }

    [SerializeField]
    private Color BuildableColor;

    [SerializeField]
    private Color NonBuildableColor;

    [SerializeField]
    private Color DefaultColor;

    [SerializeField]
    private TextMeshProUGUI ResearchPointsDisplay;

    [SerializeField]
    private TextMeshProUGUI HealthPointsDisplay;

    [SerializeField]
    private Button RotateButton;

    [SerializeField]
    private RectTransform BuildMenu;

    [SerializeField]
    private Sprite DemolishSprite;

    [SerializeField]
    private GameObject[] _TowerPrefabs;
    public GameObject[] TowerPrefabs => _TowerPrefabs;

    [SerializeField]
    private TextMeshProUGUI[] TowerPriceLabels;

    [SerializeField]
    private LevelProgressScript LevelProgress;

    [SerializeField]
    private int _MaxHealth;
    public int MaxHealth => _MaxHealth;

    [SerializeField]
    private int HealthKitRegen;

    private int[] TowerCount = new int[9];

    public bool AllowDebugCheats = true;

    public bool IsPlacingTower { get; private set; } = false;
    public bool IsDemolishingTower { get; private set; } = false;
    public bool IsPlacingItem { get; private set; } = false;

    private GameObject SelectedPrefab = null;

    private GameSpeedEnum LastGameSpeed = GameSpeedEnum.Play1;
    public GameSpeedEnum GameSpeed { get; private set; }
    public delegate void GameSpeedChangedDelegate(GameSpeedEnum newValue);

    public event GameSpeedChangedDelegate GameSpeedChanged;

    // Needed for rotating turrets while placing
    private Direction DirectionOfTower = Direction.West;

    private bool _IsRotatingSelection = false;
    public bool IsRotatingSelection { get => this._IsRotatingSelection; private set { this._IsRotatingSelection = value; this.RotateButton.gameObject.SetActive(value); } }

    public bool IsMouseOverBuildMenu { get; private set; }

    public LevelName CurrentLevel { get; private set; }

    public void SavegameLoaded()
    {
        if (WaveManager.GetInstance().CurrentWaveIndex < WaveManager.GetInstance().WaveCount - 1)
            this.CheckWaveEnd();
        this.LevelProgress.UpdateProgress();

        // Update price tags for towers and items
        foreach (var tower in this.TowerPrefabs)
        {
            UpdateStatusPointDisplay(CalculateConstructionCost(tower.TowerBase()), this.TowerPriceLabels[tower.TowerBase().Index]);
        }
    }

    public void ResetOnLevelStart(PointsInfo pointsInfo, LevelName ln)
    {
        this.CurrentLevel = ln;
        this.TowerCount = new int[9];
        this._PointsInfo = pointsInfo;
        this.ResearchPoints = this._PointsInfo.StartPoints;
        this.HealthPoints = this.MaxHealth;
        this.IsPlacingTower = this.IsDemolishingTower = this.IsPlacingItem = false;
        this.IsRotatingSelection = false;

        // Update price tags for towers and items
        foreach (var tower in this.TowerPrefabs)
        {
            UpdateStatusPointDisplay(CalculateConstructionCost(tower.TowerBase()), this.TowerPriceLabels[tower.TowerBase().Index]);
        }

        this.SelectedPrefab = null;
        this.ChangeGameSpeed(GameSpeedEnum.Pause);
        WaveManager.GetInstance().LoadWaves("Waves/Waves_" + ln.ToString());
        WaveManager.GetInstance().IsActive = true;
        this.LevelProgress.StartUpdating();
        this.InvokeRepeating(nameof(CheckWaveEnd), 0f, 0.5f);
    }

    private int CalculateConstructionCost(TowerBase towerBase)
    {
        return (int)(towerBase.ConstructionCost * Math.Pow(towerBase.ConstructionCostIncreaseFactor, this.TowerCount[towerBase.Index]));
    }

    public List<ItemCountInfo> GetCurrentItemCount()
    {
        var lst = new List<ItemCountInfo>();
        for (int i = 0; i < this.TowerPrefabs.Length; i++)
        {
            var tb = this.TowerPrefabs[i].TowerBase();
            if (tb.IsItem)
            {
                lst.Add(new ItemCountInfo() { ItemIndex = i, Count = this.TowerCount[i] });
            }
        }
        return lst;
    }

    public void UpdateItemCounts(List<ItemCountInfo> itemCounts)
    {
        foreach (var ici in itemCounts)
        {
            this.TowerCount[ici.ItemIndex] = ici.Count;
        }
    }

    public void ExitLevel()
    {
        this.CurrentLevel = LevelName.None;
        this.CancelInvoke(nameof(CheckWaveEnd));
        WaveManager.GetInstance().IsActive = false;
        LevelBuilder.GetInstance().ExitLevel();
        this.LevelProgress.StopUpdating();
    }

    private void GameOver(bool isWin)
    {
        this.CancelInvoke(nameof(CheckWaveEnd));
        WaveManager.GetInstance().IsActive = false;
        SoundManager.GetInstance().StopBackgroundMusic();
        SoundManager.GetInstance().PlaySFX(isWin ? Sound.WinClaps : Sound.Wasted);
        GameOverCanvasController.GetInstance().ConfigGameOverScreen(isWin, WaveManager.GetInstance().WaveClearedPercentage);
        CanvasManager.GetInstance().SwitchCanvas(CanvasManager.GetInstance().GetActiveCanvasAsSingleType() | CanvasType.GameOverScreen);
    }

    public void EnemyDied(int killReward, int researchKillReward)
    {
        this.ResearchPoints += (int)(killReward * this._PointsInfo.KillMultiplier + researchKillReward * this.TowerCount[5] * this._PointsInfo.KillResearchMultiplier);
    }

    private void Update()
    {
        if (this.AllowDebugCheats)
        {
            if (Input.GetKeyDown(KeyCode.F8) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F8)))
                this.ResearchPoints += this.ResearchPoints < 10000 ? 1000 : 100000;
            if (Input.GetKeyDown(KeyCode.F9) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F9)))
                this.HealthPoints += this.HealthPoints < 100000 ? 1000 : 100000;
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
            this.PickTower(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            this.PickTower(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            this.PickTower(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            this.PickTower(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            this.PickTower(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            this.PickTower(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            this.PickTower(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            this.PickTower(7);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            this.BuyHealthKit();

        if (!this.IsDemolishingTower && Input.GetKeyDown(KeyCode.Delete))
        {
            PickDemolishTower();
        }

        if ((this.IsPlacingTower || this.IsDemolishingTower || this.IsPlacingItem) && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Mouse1)))
        {
            AbortPickTower();
        }

        if (this.IsRotatingSelection && Mathf.Abs(Input.mouseScrollDelta.y) > 0)
        {
            RotatePickedTower(Mathf.Sign(Input.mouseScrollDelta.y) < 0);
        }

        this.IsMouseOverBuildMenu = this.BuildMenu.rect.Contains(this.BuildMenu.InverseTransformPoint(Input.mousePosition));
    }

    private void CheckWaveEnd()
    {
        if (WaveManager.GetInstance().IsWaiting && GameObject.FindGameObjectsWithTag(Constants.EnemyTag).Length == 0)
        {
            // Wave is over, all enemies killed -> pause the game
            this.ChangeGameSpeed(GameSpeedEnum.Pause);

            WaveManager.GetInstance().Resume();
            if (WaveManager.GetInstance().AllWavesEnded)
            {
                this.GameOver(true);
            }
            else
            {
                this.ResearchPoints += this._PointsInfo.WaveBonus;
            }
        }
    }

    public void PickTower(int index)
    {
        // do not use AbortTower() method before this if statement
        if (this.SelectedPrefab == this.TowerPrefabs[index])
        {
            // abort picking tower if button is selected twice
            this.AbortPickTower();
            return;
        }

        this.AbortPickTower();
        this.IsPlacingTower = !this.TowerPrefabs[index].TowerBase().IsItem;
        this.IsPlacingItem = this.TowerPrefabs[index].TowerBase().IsItem;
        this.SelectedPrefab = this.TowerPrefabs[index];
        Hover.GetInstance().Activate(this.SelectedPrefab.TowerBase().Thumbnail);
        foreach (var ts in LevelBuilder.GetInstance().Tiles.Values)
        {
            ApplyTileColoring(ts);
        }

        if (this.SelectedPrefab.TowerBase() is DirectionDependentTower ddt && !ddt.IgnoreDirection)
        {
            this.DirectionOfTower = Direction.East; // Start: East (because default sprite rotation is right)
            this.IsRotatingSelection = true;
        }
    }

    public void RotatePickedTower(bool turnRight)
    {
        this.DirectionOfTower = turnRight ? this.DirectionOfTower.TurnRight() : this.DirectionOfTower.TurnLeft();
        Hover.GetInstance().SetRotation(-((int)this.DirectionOfTower - 1) * 90f);  // -1 because sprites are rotated to face east, prefabs are rotated to face north
    }

    public void PickDemolishTower()
    {
        if (this.IsDemolishingTower)
        {
            // abort demolishing if this button is pressed twice
            AbortPickTower();
            return;
        }

        this.IsDemolishingTower = true;
        Hover.GetInstance().Activate(this.DemolishSprite);
        foreach (var ts in LevelBuilder.GetInstance().Tiles.Values)
        {
            ApplyTileColoring(ts);
        }
    }

    public void DemolishTower(GridPoint gp)
    {
        var tower = GetTowerOnTile(gp);
        if (tower != null)
        {
            ChangeTowerCount(false, tower.TowerBase());
            this.ResearchPoints += (int)(CalculateConstructionCost(tower.TowerBase()) * tower.TowerBase().ReimbursementFactor);
            SoundManager.GetInstance().PlaySFX(Sound.Demolish);
            GameObject.Destroy(tower.gameObject);
            LevelBuilder.GetInstance().Tiles[gp].CanBuild = true;
            AbortPickTower();
            return;
        }

        var item = GetItemOnTile(gp);
        if (item != null)
        {
            ChangeTowerCount(false, item.TowerBase());
            this.ResearchPoints += (int)(CalculateConstructionCost(item.TowerBase()) * item.TowerBase().ReimbursementFactor);
            SoundManager.GetInstance().PlaySFX(Sound.Demolish);
            GameObject.Destroy(item.gameObject);
            AbortPickTower();
            return;
        }
    }

    public void AbortPickTower()
    {
        this.IsRotatingSelection = false;
        this.IsPlacingTower = false;
        this.IsDemolishingTower = false;
        this.IsPlacingItem = false;
        Hover.GetInstance().Deactivate();
        this.SelectedPrefab = null;
        foreach (var ts in LevelBuilder.GetInstance().Tiles.Values)
        {
            ResetTileColoring(ts);
        }
    }

    public void BuyTower(GridPoint gp)
    {
        if (this.SelectedPrefab != null)
        {
            int constructionCost = CalculateConstructionCost(this.SelectedPrefab.TowerBase());
            if (constructionCost <= this.ResearchPoints && (!this.IsPlacingItem || GetItemOnTile(gp) == null))
            {
                this.ResearchPoints -= constructionCost;
                LevelBuilder.GetInstance().Tiles[gp].CanBuild = false;
                ChangeTowerCount(true, this.SelectedPrefab.TowerBase());
                CreateTower(this.SelectedPrefab, gp);
            }
        }

        AbortPickTower();
    }

    private void CreateTower(GameObject towerPrefab, GridPoint gp)
    {
        var ts = Instantiate(towerPrefab).GetComponent<TowerBase>();
        if (ts is DirectionDependentTower ddt && !ddt.IgnoreDirection)
        {
            ddt.Direction = this.DirectionOfTower;
        }

        ts.Init(gp, LevelBuilder.GetInstance().Map);
    }

    public void BuyHealthKit()
    {
        if (this.HealthPoints >= this.MaxHealth) return;

        int constructionCost = CalculateConstructionCost(this.TowerPrefabs[8].TowerBase());
        if (constructionCost <= this.ResearchPoints)
        {
            this.ResearchPoints -= constructionCost;
            ChangeTowerCount(true, this.TowerPrefabs[8].TowerBase());
            this.HealthPoints = Math.Min(this.MaxHealth, this.HealthPoints + this.HealthKitRegen);
            SoundManager.GetInstance().PlaySFX(Sound.HealUp);
        }
    }

    public void ChangeTowerCount(bool isIncrease, TowerBase tower)
    {
        if (isIncrease)
        {
            this.TowerCount[tower.Index]++;
        }
        else
        {
            if ((tower is MaskBarrier mask) && !mask.IsFullHealth) { return; } // for balancing: construction cost will only be decreased if mask was at full health
            this.TowerCount[tower.Index]--;
        }

        UpdateStatusPointDisplay(CalculateConstructionCost(tower), this.TowerPriceLabels[tower.Index]);
    }

    public void DecreaseHealth(int damage = 1)
    {
        if (this.HealthPoints > 0)
        {
            this.HealthPoints -= damage;
            if (this.HealthPoints <= 0)
            {
                this.HealthPoints = 0;
                this.GameOver(false);
            }
        }
    }

    private GameObject GetTowerOnTile(GridPoint gp)
    {
        foreach (var tower in GameObject.FindGameObjectsWithTag(Constants.TowerTag))
        {
            if (tower.GetComponent<TowerBase>().LocationOnMap == gp) { return tower; }
        }

        return null;
    }

    private GameObject GetItemOnTile(GridPoint gp)
    {
        foreach (var item in GameObject.FindGameObjectsWithTag(Constants.ItemTag))
        {
            if (item.GetComponent<TowerBase>().LocationOnMap == gp) { return item; }
        }

        return null;
    }

    public void HighlightTileOnMouseHover(TileScript ts)
    {
        if (Hover.GetInstance().IsActivated)
        {
            if (this.IsPlacingTower)
            {
                ts.SpriteRenderer.color = ts.CanBuild ? this.DefaultColor : this.NonBuildableColor;
            }
            else if (this.IsDemolishingTower)
            {
                ts.SpriteRenderer.color = GetTowerOnTile(ts.GridPosition) != null ? this.DefaultColor : this.NonBuildableColor;
            }
            else if (this.IsPlacingItem)
            {
                ts.SpriteRenderer.color = ts.IsWay && GetItemOnTile(ts.GridPosition) == null ? this.DefaultColor : this.NonBuildableColor;
            }
        }
    }

    public void RedoTileColoring(TileScript ts)
    {
        if (Hover.GetInstance().IsActivated)
        {
            ApplyTileColoring(ts);
        }
    }

    public void ApplyTileColoring(TileScript ts)
    {
        if (ts.IsBorder) { return; }

        if (this.IsPlacingTower)
        {
            ts.SpriteRenderer.color = ts.CanBuild ? this.BuildableColor : this.NonBuildableColor;
        }
        else if (this.IsDemolishingTower)
        {
            ts.SpriteRenderer.color = GetTowerOnTile(ts.GridPosition) != null ? this.BuildableColor : this.NonBuildableColor;
        }
        else if (this.IsPlacingItem)
        {
            ts.SpriteRenderer.color = ts.IsWay && GetItemOnTile(ts.GridPosition) == null ? this.BuildableColor : this.NonBuildableColor;
        }
    }

    public void ResetTileColoring(TileScript ts)
    {
        if (ts.IsBorder) { return; }
        ts.SpriteRenderer.color = this.DefaultColor;
    }

    private void UpdateStatusPointDisplay(int value, TextMeshProUGUI display)
    {
        int val = value;
        if (value >= 1000000)
        {
            int millions = 0;
            while (val >= 1000000)
            {
                val -= 1000000;
                millions++;
            }

            if (value >= 10000000)
            {
                display.text = millions.ToString() + "M";
            }
            else
            {
                int hundredThousands = 0;
                while (val >= 100000)
                {
                    val -= 100000;
                    hundredThousands++;
                }

                display.text = millions.ToString() + "." + hundredThousands.ToString() + "M";
            }
        }
        else if (value >= 1000)
        {
            int thousands = 0;
            while (val >= 1000)
            {
                val -= 1000;
                thousands++;
            }

            if (value >= 100000)
            {
                display.text = thousands.ToString() + "K";
            }
            else
            {
                int hundreds = 0;
                while (val >= 100)
                {
                    val -= 100;
                    hundreds++;
                }

                if (value >= 10000)
                {
                    display.text = thousands.ToString() + "." + hundreds.ToString() + "K";
                }
                else
                {
                    int tens = 0;
                    while (val >= 10)
                    {
                        val -= 10;
                        tens++;
                    }

                    display.text = thousands.ToString() + "." + hundreds.ToString() + tens.ToString() + "K";
                }
            }
        }
        else if (value >= 0)
        {
            display.text = value.ToString();
        }
        else
        {
            display.text = "A BUG!";
        }
    }


    public void ResumeWithLastGameSpeed()
    {
        this.ChangeGameSpeed(this.LastGameSpeed);
    }

    public void ChangeGameSpeed(GameSpeedEnum? changeTo)
    {
        if (!changeTo.HasValue)
        {
            switch (this.GameSpeed)
            {
                case GameSpeedEnum.Pause:
                    changeTo = GameSpeedEnum.Play1;
                    break;
                case GameSpeedEnum.Play1:
                    changeTo = GameSpeedEnum.Play2;
                    break;
                case GameSpeedEnum.Play2:
                    changeTo = GameSpeedEnum.Play4;
                    break;
                case GameSpeedEnum.Play4:
                    // Player cannot pause the game via timeplapse-button
                    changeTo = GameSpeedEnum.Play1;
                    break;
                default:
                    break;
            }
        }

        this.LastGameSpeed = this.GameSpeed;
        this.GameSpeed = changeTo.Value;
        this.GameSpeedChanged?.Invoke(this.GameSpeed);
        switch (changeTo)
        {
            case GameSpeedEnum.Pause:
                Time.timeScale = 0f;
                break;
            case GameSpeedEnum.Play1:
                Time.timeScale = 1f;
                break;
            case GameSpeedEnum.Play2:
                Time.timeScale = 2f;
                break;
            case GameSpeedEnum.Play4:
                Time.timeScale = 4f;
                break;
            default:
                break;
        }
    }
}
