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
        public GameManager gameManager;//reference var for game manager
        public GridManager gridManager;//reference var for grid  manager
        public GameObject unitWorldCanvas, moveButton, attackButton, specialButton;//reference var for units world space canvas and GUI
        public Tile unitCurrentTile;//the tile the unit is currently on, is set in SetGridPosition(), and 
        [SerializeField]
        [Header("Unit Metrics")]
        protected int health, basicDamage;
        public bool active, moved, attacked;
        public int teamID, moveRange, attackRange;
        #endregion
        public void OnUnitSpawn(int unitType)//set up function for units, with int unitType to determine which unit type is being spawned
        {
            unitWorldCanvas = transform.GetChild(0).gameObject;//sets reference to units World Space canvas
            gameManager = GameObject.FindWithTag("Game Manager").GetComponent<GameManager>();//sets reference to game manager object
            gridManager = GameObject.FindWithTag("Grid Manager").GetComponent<GridManager>();//sets reference to grid manager object
            switch (unitType)//Sets the units health, attack damage, movement range and attack range depending on what unit type is being spawned
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
            specialButton.SetActive(true);
            if(attackButton)
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
            if (attacked && attackButton)
            {
                attackButton.SetActive(false);
            }
            unitWorldCanvas.SetActive(true);
        }
        public void OnDeSelection()
        {                
            moveButton.SetActive(true);
            attackButton.SetActive(true);
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
        }
        public void SetGridPosition()
        {
            RaycastHit hit;
            if(Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, 5f))
            {
                if(hit.transform.GetComponent<Tile>() != null)
                {
                    gameObject.transform.SetParent(hit.transform);
                }
            }
            else
            {
                Debug.LogWarning("no parent found");
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
        }
    }
}