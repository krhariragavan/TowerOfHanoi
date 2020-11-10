using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Run-Time Ref --------->
    // Clicked Object
    public GameObject ClickedObject;
    // Run-Time Ref --------->

    // Scene Ref -------->
    public Tower [] AllTowers;

    public Tower OriginTower;
    public Tower VictoryTower1;
    public Tower VictoryTower2;
    // Scene Ref -------->

    [Header ("Set Value")]
    // Variables ---------->
    public int MaxDiskCount;
    // Variables ---------->

    //[HideInInspector]
    public bool IsWrongMove;

    // Events -------->
    public delegate void VictoryAchieved_Delegate (); // Event triggered when player successfully achieves the goal
    public event VictoryAchieved_Delegate OnVictoryAchieved;
    public void VictoryAchieved () // This function is called in Tower.cs 
    {
        OnVictoryAchieved?.Invoke ();
    }

    public delegate void LostGame_Delegate (); // Losses game when there are no more moves or if there is a time constrain in future
    public event LostGame_Delegate OnLostGame;
    // Events -------->

    // Move Count
    public int CurrentMoveCount;
    public bool CanPlaceDisk; // This is called from Tower.cs. OnTriggerEnter and value is being set in both Tower and This.

    public static GameManager Instance;

    private void Awake ()
    {
        Instance = this;
    }

    void Start ()
    {

    }

    void Update ()
    {
        //SetClickedObj ();
        //TowerInput ();
    }

    bool HasStarted;
    public Tower SelectedTower;
    //GameObject SelectedDiskObj;

    void TowerInput ()
    {
        // 
        if (Input.GetMouseButtonDown (0))
        {
            if (HasStarted)
            {
                // Make possible Move
                if (SelectedTower != null)
                {
                    Tower ClickedTower = OnClickSelect (false);
                    if (ClickedTower != null)
                    {
                        foreach (Tower tower in AllTowers)
                        {
                            if (SelectedTower.name != ClickedTower.name)
                            {
                                Debug.Log (tower.name);
                            }
                            else
                            {
                                Debug.Log ("Selected same tower --> " + SelectedTower);
                            }
                        }
                        SelectedTower = null;
                    }
                }
                else
                {
                    HasStarted = true;
                    SelectedTower = OnClickSelect (true);
                }
            }
            else
            {
                HasStarted = true;
                SelectedTower = OnClickSelect (true);
            }

            //HasStarted = !HasStarted;
            // Get the starting tower
            // 
        }
    }

    Tower OnClickSelect (bool WithDisk)
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast (ray, out hit, 100f))
        {
            Tower tower = hit.collider.GetComponent<Tower> ();
            if (tower != null)
            {
                if (WithDisk)
                {
                    if (tower.AllDisks.Count > 0)
                    {
                        return tower;
                    }
                    else
                    {
                        Debug.Log ("NO DISKS");

                        // Display msg
                    }
                }
                else
                {
                    return tower;
                }
            }
            //Disk disk = hit.collider.GetComponent<Disk> ();
            //if (disk != null)
            //{
            //    if (disk.CanMove) // Returns true only if it is a top piece
            //    {
            //        if (ClickedObject == null)
            //            ClickedObject = disk.gameObject;
            //    }
            //}
        }
        //else
        //    ClickedObject = null;

        return null;
    }

    /// <summary>
    /// Setting up on click object and enable move
    /// </summary>
    void SetClickedObj ()
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast (ray, out hit, 100f))
        {
            Disk interact = hit.collider.GetComponent<Disk> ();
            if (interact != null)
            {
                if (interact.CanMove) // Returns true only if it is a top piece
                {
                    if (ClickedObject == null)
                        ClickedObject = interact.gameObject;
                }
            }
        }
        else
            ClickedObject = null;

        if (Input.GetMouseButton (0)) // On Mouse Button press and hold
        {
            if (ClickedObject != null)
                ClickedObject.transform.position = new Vector3 (hit.point.x, hit.point.y, ClickedObject.transform.position.z);
        }

        if (Input.GetMouseButtonUp (0)) // On Mouse button Release
        {
            //Disk disk = ClickedObject.GetComponent<Disk> ();
            if (ClickedObject != null)
            {
                CanPlaceDisk = true;
                ClickedObject = null;
            }
            //if (disk.CurrentTower ) 
            // If disk is in the same tower no move count
            // If disk goes to different tower count Moves
        }
    }

    void IdentifyNextMove ()
    {
        // Run this only after player making first move if not, Run the victory algorithm

        int OriginMaxSize = OriginTower.GetMoveablePieceSize ();
        int Tower1MaxSize = VictoryTower1.GetMoveablePieceSize ();
        int Tower2MaxSize = VictoryTower2.GetMoveablePieceSize ();


        //int Smallest = Mathf.Min (OriginMaxSize, Tower1MaxSize, Tower2MaxSize);
        //int LargestSmallest = Mathf.Max (OriginMaxSize, Tower1MaxSize, Tower2MaxSize);

        if (OriginMaxSize <= Tower1MaxSize && OriginMaxSize <= Tower2MaxSize)
        {
            // origin has smallest
            DrawPathForNextMove (OriginTower, VictoryTower1);
            DrawPathForNextMove (OriginTower, VictoryTower2);
            if (Tower1MaxSize == Tower2MaxSize) // Max size is equal only if both doesn't have any disk
            {
                // Both doesn't has any disks
            }
            else if (Tower1MaxSize < Tower2MaxSize)
            {
                // Tower1 second smallest
                DrawPathForNextMove (VictoryTower1, VictoryTower2);
            }
            else
            {
                // Tower2 second smallest
                DrawPathForNextMove (VictoryTower2, VictoryTower1);
            }
        }
        else if (Tower1MaxSize <= OriginMaxSize && Tower1MaxSize <= Tower2MaxSize)
        {
            // Tower 1 has smallest
            DrawPathForNextMove (VictoryTower1, OriginTower);
            DrawPathForNextMove (VictoryTower1, VictoryTower2);
            if (OriginMaxSize == Tower2MaxSize) // Max size is equal only if both doesn't have any disk
            {
                // Both doesn't has any disks
            }
            else if (OriginMaxSize < Tower2MaxSize)
            {
                // Orgin second smallest
                //DrawPathForNextMove (VictoryTower1, OriginTower);
                //DrawPathForNextMove (VictoryTower1, VictoryTower2);

                DrawPathForNextMove (OriginTower, VictoryTower2);
            }
            else
            {
                // Tower2 second smallest
                DrawPathForNextMove (VictoryTower2, OriginTower);
            }
        }
        else
        {
            // Tower 2 has smallest
            DrawPathForNextMove (VictoryTower2, OriginTower);
            DrawPathForNextMove (VictoryTower2, VictoryTower2);

            if (OriginMaxSize == Tower1MaxSize) // Max size is equal only if both doesn't have any disk
            {
                // Both doesn't has any disks
            }
            else if (OriginMaxSize < Tower1MaxSize)
            {
                // Origin second smallest
                DrawPathForNextMove (OriginTower, VictoryTower1);
            }
            else
            {
                // Tower1 second smallest
                DrawPathForNextMove (VictoryTower1, OriginTower);
            }
        }
    }

    void DrawPathForNextMove (Tower FromObj, Tower ToObj)
    {
        Debug.Log ("Move this obj to --> " + FromObj.name);
        Debug.Log ("This obj --> " + FromObj.name);
    }


    void Undo ()
    {

    }

    void Redo ()
    {

    }
}
