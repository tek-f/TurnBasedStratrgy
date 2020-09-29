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
        public GameManager gameManager;
        public GridManager gridManager;
        public GameObject unitWorldCanvas, moveButton, attackButton;
        [SerializeField]
        [Header("Unit Metrics")]
        public int health, basicDamage, moveSpeed, attackRange;
        public bool active, moved, attacked;
        public int teamID;
        #endregion
        public void OnUnitSpawn()
        {
            unitWorldCanvas = transform.GetChild(0).gameObject;
            gameManager = GameObject.FindWithTag("Game Manager").GetComponent<GameManager>();
            //gameManager.AddUnitToList(gameObject.GetComponent<Unit>(), teamID);
            gridManager = GameObject.FindWithTag("Grid Manager").GetComponent<GridManager>();
            //SetGridPosition();
            Debug.Log("Set up complete");
        }
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
            gameManager.selectionState = 1;
            unitWorldCanvas.SetActive(false);
        }
        public void SelectAttack()
        {
            gameManager.selectionState = 2;
            unitWorldCanvas.SetActive(false);
        }
        public void MoveToGridSpace(GridManager gridManager,int x, int z)
        {
            Tile tile = gridManager.GetTile(x, z);
            if (tile != null)
            {
                transform.position = tile.PivotPoint;
                transform.SetParent(tile.transform);
                moved = true;
            }
            if (moved && attacked)
            {
                active = false;
            }
            if (gameManager != null)
            {
                gameManager.selectionState = 0;
            }
        }
        public void SetGridPosition()
        {
            Debug.Log("called");
            RaycastHit hit;
            if(Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, 5f))
            {
                Debug.Log("Raycast has been cast");
                if(hit.transform.GetComponent<Tile>() != null)
                {
                    gameObject.transform.SetParent(hit.transform);
                    Debug.Log("Parent set: " + gameObject.transform.parent.name);
                }
            }
            else
            {
                Debug.Log("no parent found");
            }
        }
        /*public void MoveToPosition(Transform targetTile)
        {
            Vector3 movePosition = targetTile.position;
            movePosition.y += 0.55f;
            gameObject.transform.position = movePosition;
            moved = true;

            if (moved && attacked)
            {
                active = false;
            }
            if (gameManager != null)
            {
                gameManager.selectionState = 0;
            }
        }*/
        public void BasicAttack(Unit target)
        {
            target.health -= basicDamage;
            attacked = true;
            if (moved && attacked)
            {
                active = false;
            }
            gameManager.selectionState = 0;
        }
        protected void Start()
        {
            OnUnitSpawn();
            //Testing
            health = 6;
            basicDamage = 2;
            moveSpeed = 3;
            attackRange = 1;
        }
        protected void Update()
        {
            if(Input.GetKeyDown(KeyCode.S))
            {
                SetGridPosition();
            }
        }
    }
}