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
        public int currentTeam;
        Camera mainCamera;

        public List<Unit> team1Units = new List<Unit>();
        public List<Unit> team2Units = new List<Unit>();
        #endregion
        #region PathFinding
        GridManager gridManager;
        #endregion
        #endregion
        public void EndTurn()
        {
            if(currentTeam == 1)
            {
                currentTeam = 2;
            }
            else
            {
                currentTeam = 1;
            }

            if(currentTeam == 1)
            {
                foreach (Unit unit in team1Units)
                {
                    unit.OnTurnStart();
                }
            }
            else
            {
                foreach (Unit unit in team2Units)
                {
                    unit.OnTurnStart();
                }
            }
        }
        public void AddUnitToList(Unit unit, int team)
        {
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
            currentTeam = Random.Range(1, 3);
            gridManager = GameObject.FindWithTag("Grid Manager").GetComponent<GridManager>();
            foreach (Unit unit in team1Units)
            {
                unit.OnTurnStart();
                unit.SetInitialPosition();
            }
            foreach (Unit unit in team2Units)
            {
                unit.OnTurnStart();
                unit.SetInitialPosition();
            }
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
                    if(path.Count > 0 && currentTile != null)
                    {
                        if(Input.GetMouseButtonDown(0))
                        {
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