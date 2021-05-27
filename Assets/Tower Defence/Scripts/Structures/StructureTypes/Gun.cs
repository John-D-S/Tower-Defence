using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structure
{
    public class Gun : Tower
    {
        public override void ShootProjectile()
        {
            
        }

        private void Update()
        {
            UpdateStructure();
            if (!Preview)
            {
                UpdateTower();
            }
        }
    }
}