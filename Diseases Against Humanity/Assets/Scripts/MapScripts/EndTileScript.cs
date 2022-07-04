using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapScripts
{
    public class EndTileScript : TileScript
    {
        public GameObject[] Prefabs { get; set; }

        private List<GameObject> InstantiatedEndTiles;

        public override void Init(GridPoint gridPosition, Vector3 worldPosition, Transform parent, bool canBuild, bool isBorder, bool isWay)
        {
            base.Init(gridPosition, worldPosition, parent, canBuild, isBorder, isWay);

            this.InstantiatedEndTiles = new List<GameObject>();
            foreach (var go in this.Prefabs)
            {
                var instance = Instantiate(go, this.transform.parent);
                instance.transform.position = Util.GetWorldPos(gridPosition);
                instance.GetComponent<SpriteRenderer>().sortingOrder = Constants.CrowdTileSortingOrder;
                instance.GetComponent<SpriteRenderer>().enabled = false;
                this.InstantiatedEndTiles.Add(instance);
            }
        }

        public void ShowSuitableCrowd(int health)
        {
            var maxHealth = GameManager.GetInstance().MaxHealth;
            health = Mathf.Clamp(health, 0, maxHealth);
            int idx = (int)((float)health / maxHealth * (this.InstantiatedEndTiles.Count - 1) + 0.99f);
            for (int i = 0; i < this.InstantiatedEndTiles.Count; i++)
            {
                this.InstantiatedEndTiles[i].GetComponent<SpriteRenderer>().enabled = (i == idx);
            }
        }
    }
}
