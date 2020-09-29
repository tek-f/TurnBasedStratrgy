﻿using System.Collections;
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
        public Tile unitCurrentTile;
        [SerializeField]
        [Header("Unit Metrics")]
        protected int health, basicDamage, unitType;
        public bool active, moved, attacked;
        public int teamID, moveRange, attackRange;
        #endregion
        public void OnUnitSpawn()
        {
            unitWorldCanvas = transform.GetChild(0).gameObject;
            gameManager = GameObject.FindWithTag("Game Manager").GetComponent<GameManager>();
            //gameManager.AddUnitToList(gameObject.GetComponent<Unit>(), teamID);
            gridManager = GameObject.FindWithTag("Grid Manager").GetComponent<GridManager>();
            SetGridPosition();
            switch (unitType)
            {
                case 0://Base Unit
                    health = 15;
                    basicDamage = 0;
                    moveRange = 5;
                    attackRange = 2;
                    break;
                case 1://Melee Unit
                    health = 10;
                    basicDamage = 2;
                    moveRange = 2;
                    attackRange = 1;
                    break;
                case 2://Ranged Unit
                    health = 5;
                    basicDamage = 4;
                    moveRange = 1;
                    attackRange = 4;
                    break;
            }
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
                unitCurrentTile = tile;
            }
            if (moved && attacked)
            {
                active = false;
            }
            if (gameManager != null)
            {
                gameManager.selectionState = 0;
            }
            Debug.Log(x + " and " + z);
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
        protected virtual void Start()
        {
            OnUnitSpawn();

            ////Testing
            //health = 6;
            //basicDamage = 2;
            //moveSpeed = 3;
            //attackRange = 3;
        }
    }
}