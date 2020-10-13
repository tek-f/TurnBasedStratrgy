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
        public GameManager gameManager;//reference variable for the game manager
        public GridManager gridManager;//reference variable for gthe rid manager
        public GameObject unitWorldCanvas, moveButton, attackButton, specialButton;//reference variables for the units world space canvas and GUI
        public Tile unitCurrentTile;//the tile the unit is currently on, is set in MoveToGridSpace() 
        [Header("Unit Metrics")]
        [SerializeField]
        protected int health, basicDamage, unitType;
        public int BasicDamage
        {
            get { return basicDamage; }
        }
        [SerializeField]
        protected bool active, moved, attacked, shielded;
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }
        public bool Moved
        {
            get { return moved; }
            set { moved = value; }
        }
        public bool Attacked
        {
            get { return attacked; }
            set { attacked = value; }
        }
        public bool Shielded
        {
            get { return shielded; }
            set { shielded = value; }
        }
        [SerializeField]
        protected int teamID, moveRange, attackRange;
        public int TeamID
        {
            get { return teamID; }
            set { teamID = value; }
        }
        public int AttackRange
        {
            get { return attackRange; }
        }
        public int MoveRange
        {
            get { return moveRange; }
        }
        #endregion
        public void OnUnitSpawn(int UnitType)//set up function for units, with int unitType to determine which unit type is being spawned
        {
            unitWorldCanvas = transform.GetChild(0).gameObject;//sets reference to units World Space canvas
            gameManager = GameObject.FindWithTag("Game Manager").GetComponent<GameManager>();//sets reference to game manager object
            gridManager = GameObject.FindWithTag("Grid Manager").GetComponent<GridManager>();//sets reference to grid manager object
            switch (UnitType)//Sets the units health, attack damage, movement range and attack range depending on what unit type is being spawned
            {
                case 0://Base Unit
                    health = 15;
                    basicDamage = 0;
                    moveRange = 5;
                    attackRange = 3;
                    break;
                case 1://Melee Unit
                    health = 10;
                    basicDamage = 10;
                    moveRange = 4;
                    attackRange = 3;
                    break;
                case 2://Ranged Unit
                    health = 5;
                    basicDamage = 4;
                    moveRange = 3;
                    attackRange = 6;
                    break;
            }
            unitType = UnitType;
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
        public void GUISetUp()
        {
            
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
            if(attacked || moved)
            {
                specialButton.SetActive(false);
            }
            unitWorldCanvas.SetActive(true);
            gameManager.selectionState = 6;
        }
        public void OnDeSelection()
        {                
            moveButton.SetActive(true);
            if (attackButton)
            {
                attackButton.SetActive(true);
            }
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
        public void TakeDamage(int damage)
        {
            if (shielded)
            {
                shielded = false;
                Debug.Log("attack shielded");
                return;
            }
            health -= damage;
            if(health <= 0)
            {
                gameManager.RemoveUnitFromList(this, teamID);//remove unit from unit team list on game manager
                if (unitType == 0)//if unit is base unit
                {
                    gameManager.EndGame();//game ends
                }
                Destroy(gameObject);//destroy the unit
            }
        }
    }
}