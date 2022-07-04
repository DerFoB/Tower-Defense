using UnityEngine;

namespace Assets.Scripts.DefenseScripts
{
    public abstract class DirectionDependentTower : TowerBase
    {
        public Direction Direction { get; set; }
        public abstract bool IgnoreDirection { get; }

        protected override void Init()
        {
            base.Init();
            if (!IgnoreDirection)
            {
                var rot = -(((int)this.Direction)) * 90f; // Rotation is counterclockwise
                this.transform.rotation = Quaternion.Euler(0, 0, rot);
            }
        }
    }

}
