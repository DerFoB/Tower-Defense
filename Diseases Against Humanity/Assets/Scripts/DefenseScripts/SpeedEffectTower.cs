using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DefenseScripts
{
    public abstract class SpeedEffectTower : CCTower
    {
        [SerializeField]
        private int MinimumEnemiesOnTiles = 1;

        private bool ShouldShoot = false;

        [SerializeField]
        private float ReloadDuration;

        private float RemainingReloadDuration;

        public override HashSet<GridPoint> AffectedTiles { get; protected set; }
        // For faster comparison if enemy is on tile
        protected Rect[] AffectedTileRects { get; private set; }

        [SerializeField]
        private float _SpeedMultiplier;
        protected override float SpeedMultiplier => _SpeedMultiplier;


        [SerializeField]
        private float _EffectDuration;
        protected override float? EffectDuration => _EffectDuration;

        protected override void Init()
        {
            this.AffectedTiles = new HashSet<GridPoint>();
            base.Init();
            this.SetAffectedTiles();
            this.AffectedTileRects = new Rect[this.AffectedTiles.Count];
            int i = 0;
            foreach (var gp in this.AffectedTiles)
            {
                var pos = Util.GetWorldPos(gp);
                var size = new Vector2(Util.TileSize.x, -Util.TileSize.y); // Negative y-value because y-Axis is reversed
                this.AffectedTileRects[i] = new Rect(pos, size);
                i++;
            }
        }

        protected abstract void SetAffectedTiles();

        protected override void UpdateTarget()
        {
            var enemyCount = 0;
            foreach (var enemy in this.GetAllEnemies())
            {
                for (int i = 0; i < this.AffectedTileRects.Length; i++)
                {
                    if (this.AffectedTileRects[i].Contains(enemy.transform.position, true))
                    {
                        if (++enemyCount >= this.MinimumEnemiesOnTiles)
                        {
                            this.ShouldShoot = true;
                            return;
                        }
                    }
                }
            }
            this.ShouldShoot = false;
        }

        private void Update()
        {
            this.RemainingReloadDuration -= Time.deltaTime;
            if (this.ShouldShoot && this.RemainingReloadDuration <= 0f)
            {
                Shoot();
                this.RemainingReloadDuration = this.ReloadDuration;
            }
        }

        protected abstract void Shoot();

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            foreach (var r in this.AffectedTileRects)
            {
                Gizmos.DrawWireCube(r.center, r.size);
            }
        }
    }
}
