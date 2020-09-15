using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AztecArmy.Units
{
    public class Unit : MonoBehaviour
    {
        #region Variables
        [Header("Game Management")]
        bool selected;
        [Header("Unit Metrics")]
        int health;
        #endregion
        public void OnSelection()
        {
            //set units menu to active
            //selected = true
        }
        public void Movement()
        {

        }
        public void BasicAttack(int damage, int range, Unit target)
        {

        }
    }
}