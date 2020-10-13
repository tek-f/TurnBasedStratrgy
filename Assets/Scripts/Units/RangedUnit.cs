using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AztecArmy.Units
{
    public class RangedUnit : Unit
    {
        List<Unit> poisonedUnits = new List<Unit>();
        public void PoisonAttack(Unit target)
        {
            target.Poisoned = true;
            poisonedUnits.Add(target);
            active = false;
            unitWorldCanvas.SetActive(false);
        }
        private void Start()
        {
            OnUnitSpawn(2);
        }
    }
}