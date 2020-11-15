using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AztecArmy.Units;
using AztecArmy.gridManager;
using System.Linq;
using TMPro;

namespace AztecArmy.gameManager
{
    public class GameManager : MonoBehaviour
    {
        #region Variables
        #region Game Setup
        public int selectionState = 0;//used to determine what kind of selection state the player is in, i.e. selecting units, selecting where to move the selectedUnit
        public int currentTeam;//used to track which team is active, i.e. which team's turn it is
        Camera mainCamera;
        LayerMask unitLayer = 8, tileLayer = 9;
        [SerializeField] GameObject HUDPanel, gameOverPanel;
        #endregion
        #region Units
        public Unit selectedUnit;//the unit that is currently selected by the game manager
        public List<List<Unit>> teamList = new List<List<Unit>>();//a list of a list of units, functions as both the list of teams in the game, and as the individual team lists
        public int numberOfTeams;
        public GameObject baseUnitPrefab;
        #endregion
        #region PathFinding
        GridManager gridManager;
        LineRenderer lineRenderer;
        #endregion
        #region Mana
        [SerializeField] List<int> mana = new List<int>();//list of ints that represent the mana each team has
        int initialManaValue = 25;
        #endregion
        #region End Game Display
        [SerializeField] TMP_Text endGameTextDisplay;
        #endregion
        #endregion
        public void EndTurn()
        {
            if (selectedUnit != null)
            {
                selectedUnit.OnDeSelection();
                selectedUnit = null;
            }

            selectionState = 0;

            if (currentTeam == numberOfTeams)
            {
                currentTeam = 1;
            }
            else
            {
                currentTeam++;
            }
            var tempTeamList = teamList[currentTeam - 1].ToList();
            foreach (Unit unit in tempTeamList)
            {
                unit.OnTurnStart();
            }
        }
        public void AddUnitToList(Unit unit, int team)
        {
            teamList[team - 1].Add(unit);
        }
        public void RemoveUnitFromList(Unit unit, int team)
        {
            teamList[team - 1].Remove(unit);
        }
        public void EndGame(int losingTeamID)
        {
            int winningTeamID = 1;
            if(losingTeamID == 1)
            {
                winningTeamID = 2;
            }
            HUDPanel.SetActive(false);//close the game heads up display UI
            endGameTextDisplay.text = "Team " + winningTeamID + " is victorious!";
            gameOverPanel.SetActive(true);//open end game UI
        }
        public void AddMana(int teamID, int value)
        {
            if(teamID == 1)
            {
                mana[0] += value;
            }
            else if(teamID == 2)
            {
                mana[1] += value;
            }
            else
            {
                Debug.LogWarning("Mana index out of range");
            }
        }
        void Start()
        {
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            gridManager = GameObject.FindWithTag("Grid Manager").GetComponent<GridManager>();            
            lineRenderer = gameObject.GetComponent<LineRenderer>();

            gridManager.GenerateTiles();//Generate the game map

            #region Team SetUp
            for (int i = 0; i < numberOfTeams; i++)//Loops through the list of teams
            {
                teamList.Add(new List<Unit>());
                //Instantiate prefab of base unit
                Unit baseUnit = Instantiate(baseUnitPrefab).GetComponent<Unit>();
                //Set team ID
                baseUnit.TeamID = i + 1;
                //Add base unit to team list
                teamList[i].Add(baseUnit);
                //Set up base Unit
                teamList[i][0].OnUnitSpawn(0);
                //Set up mana
                mana.Add(initialManaValue);
            }

            //TEMP Team base setup, will need to be changed to allow for more than two teams
            teamList[0][0].MoveToGridSpace(gridManager, 0, 0);
            teamList[1][0].MoveToGridSpace(gridManager, 9, 9);
            teamList[0][0].OnTurnStart();
            teamList[1][0].OnTurnStart();
            //TEMP

            currentTeam = Random.Range(1, numberOfTeams + 1);
            #endregion
        }
        void Update()
        {
            Tile currentTile = null;
            switch (selectionState)
            {
                case 0://selecting units
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray0 = mainCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit0;
                        if (Physics.Raycast(ray0, out hit0))
                        {
                            Unit tempUnit0 = hit0.transform.gameObject.GetComponent<Unit>();
                            if (tempUnit0 != null && tempUnit0.TeamID == currentTeam && tempUnit0.Active == true)
                            {
                                Debug.Log(tempUnit0 + " unit clicked on");
                                if (selectedUnit != null)
                                {
                                    selectedUnit.OnDeSelection();
                                }
                                selectedUnit = tempUnit0;
                                selectedUnit.OnSelection(mana[selectedUnit.TeamID - 1]);
                            }
                        }
                    }
                    break;

                case 1://selecting movement
                    Ray ray1 = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit1;
                    List<Tile> path = new List<Tile>();
                    if (Physics.Raycast(ray1, out hit1, 100f))
                    {
                        Transform test1 = hit1.transform;
                        if (hit1.transform.GetComponent<Tile>() != null && hit1.transform.childCount == 0)
                        {
                            //Path finding
                            currentTile = hit1.transform.GetComponent<Tile>();
                            path = gridManager.FindPath(selectedUnit.GetComponentInParent<Tile>(), currentTile);

                            //Path Line Rendering
                            lineRenderer.positionCount = path.Count();
                            List<Vector3> pathLine = new List<Vector3>();
                            for(int i = 0; i < path.Count; i++)
                            {
                                Vector3 lineVector = path[i].transform.position;
                                lineVector.y = 0.25f;
                                pathLine.Add(lineVector);
                            }
                            lineRenderer.SetPositions(pathLine.ToArray());
                        }
                        else
                        {
                            path.Clear();
                        }
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (path.Count > 0 && path.Count <= selectedUnit.MoveRange && currentTile != null)
                        {
                            selectedUnit.MoveToGridSpace(gridManager, currentTile.x, currentTile.z);
                            selectionState = 0;
                            lineRenderer.enabled = false;
                            selectedUnit = null;
                        }
                    }
                    break;

                case 2://selecting attack target
                    Ray ray2 = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit2;
                    Unit tempUnit2 = null;
                    float attackDistance = 0;
                    if (Physics.Raycast(ray2, out hit2))
                    {
                        tempUnit2 = hit2.transform.gameObject.GetComponent<Unit>();
                        if (tempUnit2 != null)
                        {
                            attackDistance = Vector3.Distance(selectedUnit.unitCurrentTile.PivotPoint, tempUnit2.unitCurrentTile.PivotPoint);
                        }
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        Debug.Log(attackDistance);
                        if (attackDistance > 0 && attackDistance <= selectedUnit.AttackRange)
                        {
                            tempUnit2.TakeDamage(selectedUnit.BasicDamage);
                            selectedUnit.Attacked = true;
                            mana[selectedUnit.TeamID - 1] -= selectedUnit.AttackManaCost;
                            selectionState = 0;
                            selectedUnit = null;
                        }
                    }
                    break;

                case 3://selecting target for spawn ability, specific to the base unit
                    Ray ray3 = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit3;
                    float spwanDistance = 0;
                    if (Physics.Raycast(ray3, out hit3))
                    {
                        currentTile = hit3.transform.GetComponent<Tile>();
                        if (currentTile != null)
                        {
                            spwanDistance = Vector3.Distance(currentTile.PivotPoint, selectedUnit.unitCurrentTile.PivotPoint);
                            Debug.Log(spwanDistance);
                        }
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (spwanDistance > 0 && spwanDistance <= selectedUnit.AttackRange && currentTile != null)
                        {
                            selectedUnit.gameObject.GetComponent<BaseUnit>().SpawnUnit(hit3.transform);
                            mana[selectedUnit.TeamID - 1] -= selectedUnit.SpecialManaCost;
                            selectionState = 0;
                            selectedUnit = null;
                        }
                    }
                    break;
                case 4://selecting the target for the shield ability, specific to the melee unit
                    Ray ray4 = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit4;
                    Unit tempUnit4 = null;
                    float shieldTargetDistance = 0;
                    if (Physics.Raycast(ray4, out hit4))
                    {
                        tempUnit4 = hit4.transform.gameObject.GetComponent<Unit>();
                        if (tempUnit4 != null)
                        {
                            shieldTargetDistance = Vector3.Distance(tempUnit4.unitCurrentTile.PivotPoint, selectedUnit.unitCurrentTile.PivotPoint);
                        }
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (tempUnit4 != null && tempUnit4.TeamID == selectedUnit.TeamID && shieldTargetDistance <= 1.5)
                        {
                            selectedUnit.transform.gameObject.GetComponent<MeleeUnit>().ShieldUnit(tempUnit4);
                            mana[selectedUnit.TeamID - 1] -= selectedUnit.SpecialManaCost;
                            selectedUnit.Active = false;
                            selectedUnit = null;
                            selectionState = 0;
                        }
                    }
                    break;
                case 5://selecting the target for poision unit ability, specific to ranged unit ability
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray5 = mainCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit5;
                        if (Physics.Raycast(ray5, out hit5))
                        {
                            Unit tempUnit5 = hit5.transform.gameObject.GetComponent<Unit>();
                            if (tempUnit5 != null && tempUnit5.TeamID != selectedUnit.TeamID)
                            {
                                selectedUnit.transform.gameObject.GetComponent<RangedUnit>().PoisonAttack(tempUnit5);
                                mana[selectedUnit.TeamID - 1] -= selectedUnit.SpecialManaCost;
                                selectedUnit.Active = false;
                                selectedUnit = null;
                                selectionState = 0;
                            }
                        }
                    }
                    break;
                case 6://for interactng with world space UI

                    break;
                default:
                    if (Input.GetMouseButtonDown(0))
                    {
                        Debug.LogWarning("Game Manager Selection State Out of Range");
                    }
                    break;
            }
            if (Input.GetButtonDown("Cancel"))
            {
                selectionState = 0;
                if (selectedUnit != null)
                {
                    selectedUnit.OnDeSelection();
                }
                selectedUnit = null;
            }
        }
    }
}