﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public bool IsVictoryTower; // Except orgin tower other two towers are marked as vicotry tower in inspector
    public List<GameObject> AllDisks = new List<GameObject> (); // List of all objects that is placed in the tower
    public Transform TowerOriginTransform; // Setting up disk posiions

    [SerializeField] MeshRenderer mesh;
    Color TowerDefaultMaterialColor;

    Disk IncomingDisk;

    void Start ()
    {
        TowerDefaultMaterialColor = mesh.material.color;
    }

    void Update ()
    {
        if (IsVictoryTower)
        {
            //if (!GameManager.Instance.IsWrongMove)
            //{
            if (AllDisks.Count == Game.Instance.DiskCount)
            {
                //GameManager.Instance.VictoryAchieved ();
                Game.Instance.SaveBestMove ();
                UIManager.Instance.DisplayWinText (); // Displaying win text
                UIManager.Instance.IsGameStarted = false;
                //Debug.Log ("WONNNN");
            }
            //}
        }

        //SetCurrentMoveCount ();
    }

    void SetCurrentMoveCount ()
    {
        if (IncomingDisk != null)
        {
            if (GameManager.Instance.CanPlaceDisk) // Becomes true on Mouse Up.
            {
                GameManager.Instance.CanPlaceDisk = false;

                if (IncomingDisk.CurrentTowerObj != null)
                {
                    if (IncomingDisk.CurrentTowerObj != this.gameObject)
                    {
                        IncomingDisk.CurrentTowerObj = this.gameObject;
                        GameManager.Instance.CurrentMoveCount++;
                    }
                }
            }
            IncomingDisk = null;
        }
    }

    public void SetDiskPosition ()
    {
        List<int> AllDiskSizes = new List<int> ();

        foreach (GameObject DiskObj in AllDisks)
        {
            Disk disk = DiskObj.GetComponent<Disk> ();
            AllDiskSizes.Add (disk.Size);

        }

        //Array.Sort (AllDiskSizes.ToArray ());
    }

    public void AddDisk (Disk disk)
    {
        // Play animation to tower location
        AllDisks.Add (disk.gameObject);
    }

    public void RemoveDisk (Disk disk)
    {
        AllDisks.Remove (disk.gameObject);
    }

    public void RemoveAllDisk ()
    {
        AllDisks.RemoveAll (delegate (GameObject d) { return d == null; });

        AllDisks.Clear ();
        AllDisks = new List<GameObject> ();
    }

    public Disk GetMoveableDiskBySize ()
    {
        int [] AllDiskSize = new int [AllDisks.Count];

        for (int i = 0; i < AllDisks.Count; i++)
        {
            Disk disk = AllDisks [i].GetComponent<Disk> ();
            AllDiskSize [i] = disk.Size;
        }

        int MinSize = Mathf.Min (AllDiskSize);

        foreach (GameObject diskObj in AllDisks)
        {
            Disk disk = diskObj.GetComponent<Disk> ();
            if (disk.Size == MinSize)
            {
                return disk;
            }
        }
        return null;
    }

    public void SetTowerColor (bool IsDefault)
    {
        if (IsDefault)
        {
            foreach (Material mat in mesh.materials)
                mat.color = TowerDefaultMaterialColor;
        }
        else
        {
            foreach (Material mat in mesh.materials)
                mat.color = new Color (0, 1, 0, 1);
        }
    }

    //Disk GetMoveablePiece ()
    //{
    //    foreach (GameObject disk in AllDisks)
    //    {
    //        Disk interact = disk.GetComponent<Disk> ();
    //        if (interact != null)
    //        {
    //            if (interact.CanMove)
    //            {
    //                return interact;
    //            }
    //        }
    //    }

    //    return null;
    //}

    //public int GetMoveablePieceSize () // One being the smallest and 10 being the largest. Value above max disk count denotes there are no disks in the tower
    //{
    //    Disk interact = GetMoveablePiece ();

    //    if (interact != null)
    //    {
    //        return interact.Size;
    //    }
    //    else
    //    {
    //        return GameManager.Instance.MaxDiskCount + 1; // Value above max disk count denotes that there are no disks in the tower
    //    }
    //}

    /// <summary>
    /// On object enters tower ---> This step is required to check victory condition
    /// </summary>
    /// <param name="other"></param>
    //private void OnTriggerEnter (Collider other)
    //{
    //    Disk disk = other.GetComponent<Disk> ();
    //    if (disk != null)
    //    {
    //        AllDisks.Add (other.gameObject);
    //        IncomingDisk = disk;
    //        Debug.Log (other.gameObject.name + "Enter ---> " + this.name);
    //    }
    //}

    //private void OnTriggerStay (Collider other)
    //{
    //    Disk disk = other.GetComponent<Disk> ();

    //    if (disk != null)
    //    {
    //        if (disk.CurrentTowerObj == null)
    //        {
    //            //Debug.Log ("Current Tower for Disk --> " + disk.name + " is NULL");
    //            disk.CurrentTowerObj = this.gameObject;
    //        }

    //        //Debug.Log (disk.CurrentTower.name);
    //    }
    //}

    /// <summary>
    /// On object exits tower ---> This step is required to check on all the objects
    /// </summary>
    /// <param name="other"></param>
    //private void OnTriggerExit (Collider other)
    //{
    //    Disk interact = other.GetComponent<Disk> ();
    //    if (interact != null)
    //    {
    //        AllDisks.Remove (other.gameObject);
    //        Debug.Log ("Exit --->" + other.gameObject.name);
    //    }
    //}
}
