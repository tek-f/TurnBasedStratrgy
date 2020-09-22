using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AztecArmy.gameManager;
namespace AztecArmy.Units
{
    public class Unit : MonoBehaviour
    {
        #region Variables
        [Header("Game Management")]
        protected bool selected;
        public GameManager gm;
        public GameObject unitWorldCanvas;
        [SerializeField]
        [Header("Unit Metrics")]
        protected int health, basicDamage, moveSpeed, attackRange;
        #endregion
        public void OnSelection()
        {
            unitWorldCanvas.SetActive(true);
        }
        public void OnDeSelection()
        {
            unitWorldCanvas.SetActive(false);
        }
        public void SelectMovement()
        {
            gm.selectionState = 1;
            unitWorldCanvas.SetActive(false);
        }
        public void SelectAttack()
        {
            gm.selectionState = 2;
            unitWorldCanvas.SetActive(false);
        }
        public void MoveToTile(Transform targetTile)
        {
            Vector3 movePosition = targetTile.position;
            movePosition.y += 0.55f;
            gameObject.transform.position = movePosition;
            gm.selectionState = 0;
        }
        public void BasicAttack(Unit target)
        {
            target.health -= basicDamage;
            gm.selectionState = 0;
        }
        protected void Start()
        {
            unitWorldCanvas = transform.GetChild(0).gameObject;
            gm = GameObject.FindWithTag("Game Manager").GetComponent<GameManager>();
            //Testing
            health = 6;
            basicDamage = 2;
        }
    }
}