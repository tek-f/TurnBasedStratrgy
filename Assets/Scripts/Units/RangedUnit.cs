using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AztecArmy.Units
{
    public class RangedUnit : Unit
    {

        public void SelectPoisonAttack()
        {
            gameManager.selectionState = 5;
            unitWorldCanvas.SetActive(false);
        }
        public void PoisonAttack(Unit target)
        {
            target.Poisoned = true;
            active = false;
            unitWorldCanvas.SetActive(false);
        }
        private void Start()
        {
            OnUnitSpawn(2);
        }
    }
}