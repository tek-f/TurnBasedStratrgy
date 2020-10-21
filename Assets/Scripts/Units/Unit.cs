using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
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
        public GameObject unitWorldCanvas;//reference variables for the units world space canvas and GUI
        public GameObject moveButton, attackButton, specialButton;
        public Tile unitCurrentTile;//the tile the unit is currently on, is set in MoveToGridSpace()
        public LineRenderer gmLineRenderer;
        [Header("Unit Metrics")]
        [SerializeField]
        protected int health, basicDamage, unitType, attackManaCost, specialManaCost;
        public int BasicDamage
        {
            get { return basicDamage; }
        }
        public int UnitType
        {
            get { return unitType; }
        }
        public int AttackManaCost
        {
            get { return attackManaCost; }
        }
        public int SpecialManaCost
        {
            get { return specialManaCost; }
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
        #region Status Effects
        public bool poisoned;
        protected int poisonedDuration = 2, currentPoisonedDuration, poisonedDamage = 2;
        public bool Poisoned
        {
            get { return poisoned; }
            set { poisoned = value; }
        }
        #endregion
        public void OnUnitSpawn(int UnitType)//set up function for units, with int unitType to determine which unit type is being spawned
        {
            unitWorldCanvas = transform.GetChild(0).gameObject;//sets reference to units World Space canvas
            gameManager = GameObject.FindWithTag("Game Manager").GetComponent<GameManager>();//sets reference to game manager object
            gridManager = GameObject.FindWithTag("Grid Manager").GetComponent<GridManager>();//sets reference to grid manager object            
            gmLineRenderer = gameManager.GetComponent<LineRenderer>();
            switch (UnitType)//Sets the units health, attack damage, movement range and attack range depending on what unit type is being spawned
            {
                case 0://Base Unit
                    health = 15;
                    basicDamage = 0;
                    moveRange = 5;
                    attackRange = 3;
                    //attackManaCost = 1;
                    specialManaCost = 3;
                    break;
                case 1://Melee Unit
                    health = 10;
                    basicDamage = 10;
                    moveRange = 4;
                    attackRange = 3;
                    attackManaCost = 1;
                    specialManaCost = 2;
                    break;
                case 2://Ranged Unit
                    health = 5;
                    basicDamage = 4;
                    moveRange = 3;
                    attackRange = 6;
                    attackManaCost = 1;
                    specialManaCost = 4;
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
            if (attackButton)
            {
                attackButton.SetActive(true);
            }
            if(poisoned)
            {
                currentPoisonedDuration++;
                health -= poisonedDamage;
                if (health <= 0)
                {
                    UnitDeath();
                }
                if(currentPoisonedDuration >= poisonedDuration)
                {
                    poisoned = false;
                    currentPoisonedDuration = 0;
                }
            }
        }
        public void UnitDeath()
        {
            gameManager.RemoveUnitFromList(this, teamID);//remove unit from unit team list on game manager
            if (unitType == 0)//if unit is base unit
            {
                gameManager.EndGame(teamID);//game ends
            }
            Destroy(gameObject);//destroy the unit
        }
        public void OnSelection(int currentMana)
        {
            if (moved)
            {
                moveButton.SetActive(false);
            }
            if (attackButton != null && (attacked || currentMana < attackManaCost))
            {
                attackButton.SetActive(false);
            }
            if(attacked || moved || currentMana < specialManaCost)
            {
                specialButton.SetActive(false);
            }
            unitWorldCanvas.SetActive(true);
            gameManager.selectionState = 6;
        }
        public void OnDeSelection()
        {                
            moveButton.SetActive(true);
            if (attackButton != null)
            {
                attackButton.SetActive(true);
            }
            unitWorldCanvas.SetActive(false);
        }
        public void SelectMovement()
        {
            gameManager.selectionState = 1;
            gmLineRenderer.enabled = true;
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
                UnitDeath();
            }
        }
        #region Old Code
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
        #endregion
    }
}