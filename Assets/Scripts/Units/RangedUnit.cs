using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AztecArmy.Units
{
    public class RangedUnit : Unit
    {
        public void PoisonAttack()
        {

            active = false;
            unitWorldCanvas.SetActive(false);
        }
        protected override void Start()
        {
            base.Start();

            health = 5;
            basicDamage = 4;
            moveRange = 1;
            attackRange = 4;
        }
    }
}