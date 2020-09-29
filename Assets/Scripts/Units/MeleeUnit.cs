using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AztecArmy.Units
{
    public class MeleeUnit : Unit
    {
        bool defending;
        
        public void Shield()
        {
            active = false;
            unitWorldCanvas.SetActive(false);
        }

        protected override void Start()
        {
            base.Start();

            health = 10;
            basicDamage = 2;
            moveRange = 2;
            attackRange = 1;
        }
    }
}