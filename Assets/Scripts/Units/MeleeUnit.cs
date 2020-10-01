using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AztecArmy.Units
{
    public class MeleeUnit : Unit
    {
        
        public void Shield()
        {
            active = false;
            unitWorldCanvas.SetActive(false);
        }

        private void Start()
        {
            OnUnitSpawn(1);
        }
    }
}