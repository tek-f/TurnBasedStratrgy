using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AztecArmy.gameManager;
using AztecArmy.gridManager;

namespace AztecArmy.Units
{
    public class Unit : MonoBehaviour
    {
        #region Variables
        [Header("Game Management")]
        public GameManager gm;
        public GameObject unitWorldCanvas, moveButton, attackButton;
        [SerializeField]
        [Header("Unit Metrics")]
        public int health, basicDamage, moveSpeed, attackRange;
        public bool active, moved, attacked;
        public int teamID;
        #endregion
        public void OnTurnStart()
        {
            active = true;
            moved = false;
            attacked = false;
            moveButton.SetActive(true);
            if (attackButton != null)
            {
                attackButton.SetActive(true);
            }
        }
        public void OnSelection()
        {
            if (moved)
            {
                moveButton.SetActive(false);
            }
            if (attacked && attackButton != null)
            {
                attackButton.SetActive(false);
            }
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
        public void MoveToGridSpace(GridManager gridManager,int x, int z)
        {
            var tile = gridManager.GetTile(x, z);
            if (tile != null)
            {
                // Move Position to Tile
                transform.position = tile.PivotPoint;
                // Parent Unit to Tile
                transform.SetParent(tile.transform);
            }
        }
        public void CheckMovement()
        {

        }
        public void MoveToPosition(Transform targetTile)
        {
            Vector3 movePosition = targetTile.position;
            movePosition.y += 0.55f;
            gameObject.transform.position = movePosition;
            moved = true;

            if (moved && attacked)
            {
                active = false;
            }
            if (gm != null)
            {
                gm.selectionState = 0;
            }
        }
        public void BasicAttack(Unit target)
        {
            target.health -= basicDamage;
            attacked = true;
            if (moved && attacked)
            {
                active = false;
            }
            gm.selectionState = 0;
        }
        protected void Start()
        {
            unitWorldCanvas = transform.GetChild(0).gameObject;
            gm = GameObject.FindWithTag("Game Manager").GetComponent<GameManager>();
            gm.AddUnitToList(gameObject.GetComponent<Unit>(), teamID);

            //Testing
            health = 6;
            basicDamage = 2;
            moveSpeed = 3;
            attackRange = 1;
        }
    }
}