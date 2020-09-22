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
    }
}