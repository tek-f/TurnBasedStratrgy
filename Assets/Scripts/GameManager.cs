using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AztecArmy.Units;
using AztecArmy.gridManager;

namespace AztecArmy.gameManager
{
    public class GameManager : MonoBehaviour
    {
        #region Variables
        #region Units
        public Unit selectedUnit;
        public int selectionState = 0;//used to determine what kind of selection state the player is in, i.e. selecting units, selecting where to move the selectedUnit
        public int currentTeam;//used to track which team is active, i.e. which team's turn it is
        Camera mainCamera;
        [SerializeField]
        public List<List<Unit>> teamList = new List<List<Unit>>();
        public int numberOfTeams;
        public List<Unit> team1Units = new List<Unit>();
        public List<Unit> team2Units = new List<Unit>();
        public GameObject baseUnitPrefab;
        #endregion
        #region PathFinding
        GridManager gridManager;
        #endregion
        #endregion
        public void EndTurn()
        {
            if(currentTeam == numberOfTeams)
            {
                currentTeam = 1;
            }
            else
            {
                currentTeam++;
            }

            foreach (Unit unit in teamList[currentTeam - 1])
            {
                unit.OnTurnStart();
            }
        }
        public void AddUnitToList(Unit unit, int team)
        {
            teamList[team - 1].Add(unit);
        }
        void Start()
        {
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            gridManager = GameObject.FindWithTag("Grid Manager").GetComponent<GridManager>();

            gridManager.GenerateTiles();

            #region Team SetUp
            for (int i = 0; i < numberOfTeams; i++)
            {
                teamList.Add(new List<Unit>());
                //Instantiate prefab of base unit
                Unit baseUnit = Instantiate(baseUnitPrefab).GetComponent<Unit>();
                //Set team ID
                baseUnit.teamID = i + 1;
                //Add base unit to team list
                teamList[i].Add(baseUnit);
                //Set up base Unit
                teamList[i][0].OnUnitSpawn();
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
                            Unit tempUnit = hit0.transform.gameObject.GetComponent<Unit>();
                            if (tempUnit != null && tempUnit.teamID == currentTeam && tempUnit.active)
                            {
                                if (selectedUnit != null)
                                    selectedUnit.OnDeSelection();

                                selectedUnit = hit0.transform.gameObject.GetComponent<Unit>();
                                selectedUnit.OnSelection();
                            }
                        }
                    }
                    if (Input.GetButtonDown("Cancel"))
                    {
                        if (selectedUnit != null)
                        {
                            selectedUnit.OnDeSelection();
                        }
                        selectedUnit = null;
                    }
                    break;

                case 1://selecting movement
                    Ray ray1 = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit1;
                    List<Tile> path = new List<Tile>();
                    if (Physics.Raycast(ray1, out hit1))
                    {
                        if(hit1.transform.GetComponent<Tile>() != null)
                        {
                            currentTile = hit1.transform.GetComponent<Tile>();
                            path = gridManager.FindPath(selectedUnit.GetComponentInParent<Tile>(), currentTile);
                        }
                    }
                    if(Input.GetMouseButtonDown(0))
                    {
                        if (path.Count > 0 && path.Count <= selectedUnit.moveRange && currentTile != null)
                        {
                            Debug.Log("move attempted");
                            selectedUnit.MoveToGridSpace(gridManager, currentTile.x, currentTile.z);
                        }
                    }
                    if (Input.GetButtonDown("Cancel"))
                    {
                        selectionState = 0;
                    }
                    break;

                case 2://selecting attack target
                    Ray ray2 = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit2;
                    float attackDistance = 0;
                    if (Physics.Raycast(ray2, out hit2))
                    {
                        Unit tempUnit = hit2.transform.gameObject.GetComponent<Unit>();
                        if (tempUnit != null)
                        {
                            attackDistance = Vector3.Distance(selectedUnit.unitCurrentTile.PivotPoint, tempUnit.unitCurrentTile.PivotPoint);
                        }
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        if(attackDistance > 0 && attackDistance <= selectedUnit.attackRange)
                        {
                            selectedUnit.BasicAttack(hit2.transform.gameObject.GetComponent<Unit>());
                        }
                    }
                    if (Input.GetButtonDown("Cancel"))
                    {
                        selectionState = 0;
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
                        if(spwanDistance > 0 && spwanDistance <= selectedUnit.attackRange && currentTile != null)
                        {
                            selectedUnit.gameObject.GetComponent<BaseUnit>().SpawnUnit(hit3.transform);
                        }
                    }
                    if (Input.GetButtonDown("Cancel"))
                    {
                        selectionState = 0;
                    }
                    break;

                default:
                    if (Input.GetMouseButtonDown(0))
                    {
                        Debug.LogWarning("Selection State Out of Range");
                    }
                    break;
            }
        }
    }
}