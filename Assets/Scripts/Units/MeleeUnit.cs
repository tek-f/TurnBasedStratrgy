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
    }
}