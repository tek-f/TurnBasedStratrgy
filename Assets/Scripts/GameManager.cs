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

            /*if(currentTeam == 1)
            {

            }
            else
            {
                foreach (Unit unit in team2Units)
                {
                    unit.OnTurnStart();
                }
            }*/
        }
        public void AddUnitToList(Unit unit, int team)
        {
            //reference to team to be added to
            //reference to the unit to be added
            //Add referenced unit to referenced team
            //Put on unit instead?


            if (team == 1)
            {
                team1Units.Add(unit);
            }
            else if(team == 2)
            {
                team2Units.Add(unit);
            }
        }
        void Start()
        {
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

            gridManager = GameObject.FindWithTag("Grid Manager").GetComponent<GridManager>();
            gridManager.GenerateTiles();

            Debug.Log(gridManager.m_tiles.Length);

            //get number of teams
            //create


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
            Debug.Log(teamList[0][0].gameObject.name);
            Debug.Log(teamList[1][0].gameObject.name);

            /*foreach (List<Unit> team in teamList)
            {
                //Instantiate prefab of base unit
                GameObject baseUnit = Instantiate(baseUnitPrefab);
                //Add base unit to team list
                team.Add(baseUnit.GetComponent<Unit>());
                team[0].OnUnitSpawn();
            }*/

            //TEMP Team base setup, will need to be changed to allow for more than two teams

            currentTeam = Random.Range(1, numberOfTeams + 1);

            /*foreach (Unit unit in team1Units)
            {
                unit.OnTurnStart();
                unit.SetGridPosition();
            }
            foreach (Unit unit in team2Units)
            {
                unit.OnTurnStart();
                unit.SetGridPosition();
            }*/

        }
        void Update()
        {
            switch (selectionState)
            {
                case 0://selecting units
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            Unit tempUnit = hit.transform.gameObject.GetComponent<Unit>();
                            if (tempUnit != null && tempUnit.teamID == currentTeam && tempUnit.active)
                            {
                                if (selectedUnit != null)
                                    selectedUnit.OnDeSelection();

                                selectedUnit = hit.transform.gameObject.GetComponent<Unit>();
                                selectedUnit.OnSelection();
                            }
                        }
                    }
                    if (Input.GetButtonDown("Cancel"))
                    {
                        if (selectedUnit != null)
                            selectedUnit.OnDeSelection();

                        selectedUnit = null;
                    }
                    break;
                case 1://selecting movement
                    Ray ray1 = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit1;
                    Tile currentTile = null;
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
                        if (path.Count > 0 /*&& path.Count <= selectedUnit.moveSpeed */&& currentTile != null)
                        {
                            Debug.Log("move attempted");
                            selectedUnit.MoveToGridSpace(gridManager, currentTile.x, currentTile.z);
                        }
                    }
                    /*
                   if (Input.GetMouseButtonDown(0))
                   {
                       Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                       RaycastHit hit;
                       if (Physics.Raycast(ray, out hit))
                       {



                           //Old
                           if (hit.transform.gameObject.CompareTag("Tile") && selectedUnit != null)
                               selectedUnit.MoveToTile(hit.transform);
                       }
                   }
                   */
                    if (Input.GetButtonDown("Cancel"))
                        selectionState = 0;
                    break;

                case 2://selecting attack target
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.gameObject.GetComponent<Unit>() != null)
                                selectedUnit.BasicAttack(hit.transform.gameObject.GetComponent<Unit>());
                        }
                    }
                    if (Input.GetButtonDown("Cancel"))
                        selectionState = 0;
                    break;

                case 3://selecting target for spawn ability, specific to the base unit
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.CompareTag("Tile"))
                                selectedUnit.gameObject.GetComponent<BaseUnit>().SpawnUnit(hit.transform);
                        }
                    }
                    if (Input.GetButtonDown("Cancel"))
                        selectionState = 0;

                    break;

                default:
                    if (Input.GetMouseButtonDown(0))
                        Debug.LogError("Selection State Out of Range");

                    break;
            }
        }
    }
}