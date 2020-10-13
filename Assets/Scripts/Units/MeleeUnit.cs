using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AztecArmy.Units
{
    public class MeleeUnit : Unit
    {
        public void SelectShieldUnit()
        {
            gameManager.selectionState = 4;
            unitWorldCanvas.SetActive(false);
        }
        public void ShieldUnit(Unit shieldedUnit)
        {
            shieldedUnit.Shielded = true;
            Debug.Log(shieldedUnit + " has been shielded");
            active = false;
        }
        private void Start()
        {
            OnUnitSpawn(1);
        }
    }
}