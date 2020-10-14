using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AztecArmy.Units
{
    public class RangedUnit : Unit
    {
        [SerializeField] int poisonAttackDamage = 2;
        public void SelectPoisonAttack()
        {
            gameManager.selectionState = 5;
            unitWorldCanvas.SetActive(false);
        }
        public void PoisonAttack(Unit target)
        {
            target.TakeDamage(poisonAttackDamage);

            int a = Random.Range(0, 2);
            if(a == 1)
            {
                target.Poisoned = true;
                Debug.Log("poison suceeded");
                active = false;
            }

            unitWorldCanvas.SetActive(false);
        }
        private void Start()
        {
            OnUnitSpawn(2);
        }
    }
}