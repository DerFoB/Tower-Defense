using Assets.Scripts.DefenseScripts;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    [SerializeField]
    private GameObject[] EnemyPrefabs;

    private List<Wave> Waves;
    private int _CurrentWaveIndex;
    public int CurrentWaveIndex {
        get
        {
            return this._CurrentWaveIndex;
        }
        set
        {
            this._CurrentWaveIndex = Math.Min(this.Waves.Count, value);
        }
    }
    public int WaveCount => this.Waves.Count;

    public Wave CurrentWave => AllWavesEnded ? null : this.Waves[this.CurrentWaveIndex];
    public bool AllWavesEnded => this.CurrentWaveIndex >= this.Waves.Count;

    public bool IsActive { get; set; }
    public bool IsWaiting { get; set; }
    public int WaveClearedPercentage => Mathf.RoundToInt(100f * this.CurrentWaveIndex / this.WaveCount);

    public int KilledEnemiesInWave = 0;

    private void Update()
    {
        if (this.IsActive && !this.IsWaiting)
        {
            if (this.CurrentWave?.Update() ?? false)
            {
                // Wait until all enemies are dead
                this.IsWaiting = true;
            }
        }

        if (GameManager.GetInstance().AllowDebugCheats)
        {
            if (Input.GetKeyDown(KeyCode.F1) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F1)))
                SpawnEnemy(this.EnemyPrefabs[0]);
            if (Input.GetKeyDown(KeyCode.F2) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F2)))
                SpawnEnemy(this.EnemyPrefabs[1]);
            if (Input.GetKeyDown(KeyCode.F3) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F3)))
                SpawnEnemy(this.EnemyPrefabs[2]);
            if (Input.GetKeyDown(KeyCode.F4) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F4)))
                SpawnEnemy(this.EnemyPrefabs[3]);
            if (Input.GetKeyDown(KeyCode.F5) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F5)))
                SpawnEnemy(this.EnemyPrefabs[4]);
            if (Input.GetKeyDown(KeyCode.F6) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F6)))
                SpawnEnemy(this.EnemyPrefabs[5]);
            if (Input.GetKeyDown(KeyCode.F7) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F7)))
                SpawnEnemy(this.EnemyPrefabs[6]);
        }
    }

    public void Resume()
    {
        CreateSavegame();
        this.CurrentWaveIndex++;
        this.IsWaiting = false;
        this.KilledEnemiesInWave = 0;
    }

    private void CreateSavegame()
    {
        var towerList = new List<TowerInfo>();
        int direction;
        int hp;
        // Get all Towers
        foreach (var towerObject in GameObject.FindGameObjectsWithTag("Tower"))
        {
            direction = 0;
            var tower = towerObject.GetComponent(typeof(TowerBase)) as TowerBase;
            if (tower is DirectionDependentTower ddt && !ddt.IgnoreDirection)
            {
                direction = (int)ddt.Direction;
            }
            towerList.Add(new TowerInfo(tower.LocationOnMap, tower.Index, direction, 0));
        }
        // Get all Items
        foreach (var itemObject in GameObject.FindGameObjectsWithTag("Item"))
        {
            hp = 0;
            var item = itemObject.GetComponent(typeof(TowerBase)) as TowerBase;
            if (item is MaskBarrier mb)
            {
                hp = mb.ActualHealth;
            }
            towerList.Add(new TowerInfo(item.LocationOnMap, item.Index, 0, hp));
        }
        var gm = GameManager.GetInstance();
        Savegame.GetInstance().WriteSavegame(gm.CurrentLevel, CurrentWaveIndex, towerList, gm.ResearchPoints, gm.HealthPoints, gm.GetCurrentItemCount());
    }

    public void LoadWaves(string filename)
    {
        var ta = Resources.Load(filename) as TextAsset;
        var text = ta.text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < text.Length; i++)
        {
            text[i] = text[i].Trim(' ', '\r', '\n').Trim(' ', '\r', '\n');
        }
        var textLines = new List<string>();
        foreach (var line in text)
        {
            if (!line.StartsWith("#") && !string.IsNullOrWhiteSpace(line)) // Ignore comments and empty lines
                textLines.Add(line);
        }


        this.Waves = new List<Wave>();

        int lineNo = 0;
        while (lineNo < textLines.Count)
        {
            ParseWaveLine(textLines, ref lineNo);
        }
        this.IsWaiting = false;
        this.CurrentWaveIndex = 0;
        this.KilledEnemiesInWave = 0;
    }

    private void ParseWaveLine(List<string> text, ref int lineNo)
    {
        if (text[lineNo].StartsWith("Wave")) // Start a new Wave
        {
            lineNo++;
            ParseWave(text, ref lineNo);
        }
    }

    private void ParseWave(List<string> text, ref int lineNo)
    {
        try
        {
            var wave = new Wave();
            while (lineNo < text.Count && text[lineNo] != "Wave") // EOF or Next wave
            {
                var bunchLine = text[lineNo++];
                var bunchElements = bunchLine.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var delayBefore = float.Parse(bunchElements[0], CultureInfo.InvariantCulture);
                var delayBetween = float.Parse(bunchElements[1], CultureInfo.InvariantCulture);
                var enemies = new List<GameObject>();
                for (int i = 2; i < bunchElements.Length; i++)
                {
                    var enemyDescription = bunchElements[i].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var enemy = WaveManager.GetInstance().EnemyPrefabs[int.Parse(enemyDescription[0])];
                    if (enemyDescription.Length > 1)
                    {
                        int count = int.Parse(enemyDescription[1]);
                        for (int j = 0; j < count; j++)
                            enemies.Add(enemy);
                    }
                    else
                    {
                        enemies.Add(enemy);
                    }
                }
                wave.Bunches.Add(new EnemyBunch(enemies.ToArray(), delayBefore, delayBetween));
            }
            wave.EndInit();
            this.Waves.Add(wave);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parsing failed at line {lineNo}, Successfully parsed waves: {this.Waves.Count}");
            Debug.LogException(ex);
            throw new Exception();
        }
    }

    private void SpawnEnemy(GameObject prefab)
    {
        var es = Instantiate(prefab).GetComponent<EnemyBase>();
        es.Init(new PathFinder(LevelBuilder.GetInstance().PathInfo), LevelBuilder.GetInstance().Map);
    }

    public class Wave
    {
        public List<EnemyBunch> Bunches { get; private set; }
        private int CurrentBunchIndex;

        public EnemyBunch CurrentBunch => this.WaveEnded ? null : this.Bunches[this.CurrentBunchIndex];
        public bool WaveEnded => this.CurrentBunchIndex >= this.Bunches.Count;

        public int EnemyCount { get; private set; }

        public bool Update()
        {
            if (this.CurrentBunch?.Update() ?? false)
            {
                this.CurrentBunchIndex++;
            }
            return this.WaveEnded;
        }

        public Wave()
        {
            this.Bunches = new List<EnemyBunch>();
            this.CurrentBunchIndex = 0;
        }

        public void EndInit()
        {
            var count = 0;
            foreach(var bunch in this.Bunches)
            {
                foreach(var e in bunch.Enemies)
                {
                    if(e.GetComponent<EnemyBase>() is Bacterium_2 b2)
                    {
                        count += b2.PepegaSpawnCount;
                    }
                    count++;
                }
            }
            this.EnemyCount = count;
        }
    }

    public class EnemyBunch
    {
        public GameObject[] Enemies { get; private set; }
        private int NextEnemyIndex;
        public GameObject NextEnemy => this.BunchEnded ? null : this.Enemies[this.NextEnemyIndex];
        public bool BunchEnded => this.NextEnemyIndex >= this.Enemies.Length;

        private float DelayTimer;
        public float DelayBetween { get; private set; }

        public EnemyBunch(GameObject[] enemies, float delayBefore, float delayBetween)
        {
            this.Enemies = enemies;
            this.DelayTimer = delayBefore;
            this.DelayBetween = delayBetween;
            this.NextEnemyIndex = 0;
        }

        public bool Update()
        {
            this.DelayTimer -= Time.deltaTime;
            if (this.DelayTimer <= 0f)
            {
                SpawnNextEnemy();
                this.NextEnemyIndex++;
                this.DelayTimer = this.DelayBetween;
            }
            return this.BunchEnded;
        }

        private void SpawnNextEnemy()
        {
            WaveManager.GetInstance().SpawnEnemy(this.NextEnemy);
        }
    }

    public float CalculateWaveProgress()
    {
        float baseValue = 1f / this.Waves.Count;
        float min = CurrentWaveIndex * baseValue;
        float enemies = this.CurrentWave?.EnemyCount ?? 0;
        return min + this.KilledEnemiesInWave / enemies * baseValue;
    }
}
