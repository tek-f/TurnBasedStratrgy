using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
        public void SelectAttack()
        {
            gm.selectionState = 2;
        }
        public void MoveToTile(Transform targetTile)
        {
            Vector3 movePosition = targetTile.position;
            movePosition.y += 0.55f;
            gameObject.transform.position = movePosition;
            gm.selectionState = 0;
            unitWorldCanvas.SetActive(false);
        }
        public void BasicAttack(Unit target)
        {
            target.health -= basicDamage;
            gm.selectionState = 0;
            unitWorldCanvas.SetActive(false);
        }
        protected void Start()
        {
            unitWorldCanvas = transform.GetChild(0).gameObject;
            gm = GameObject.FindWithTag("Game Manager").GetComponent<GameManager>();
            //Testing
            health = 6;
            basicDamage = 2;
        }
        protected void Update()
        {
            if(Input.GetKeyDown(KeyCode.M))
            {
                SelectMovement();
            }
        }
    }
}