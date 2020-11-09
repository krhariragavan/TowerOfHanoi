using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public bool IsVictoryTower; // Except orgin tower other two towers are marked as vicotry tower in inspector
    public List<GameObject> AllDisks = new List<GameObject> (); // List of all objects that is placed in the tower

    void Start ()
    {

    }

    void Update ()
    {
        if (IsVictoryTower)
        {
            if (!GameManager.Instance.IsWrongMove)
            {
                if (AllDisks.Count == GameManager.Instance.MaxDiskCount)
                {
                    GameManager.Instance.VictoryAchieved ();
                }
            }
        }
    }

    Interactable GetMoveablePiece ()
    {
        foreach (GameObject disk in AllDisks)
        {
            Interactable interact = disk.GetComponent<Interactable> ();
            if (interact != null)
            {
                if (interact.CanMove)
                {
                    return interact;
                }
            }
        }

        return null;
    }

    public int GetMoveablePieceSize () // One being the smallest and 10 being the largest. Value above max disk count denotes there are no disks in the tower
    {
        Interactable interact = GetMoveablePiece ();

        if (interact != null)
        {
            return interact.Size;
        }
        else
        {
            return GameManager.Instance.MaxDiskCount + 1; // Value above max disk count denotes that there are no disks in the tower
        }
    }

    /// <summary>
    /// On object enters tower ---> This step is required to check victory condition
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter (Collider other)
    {
        Interactable interact = other.GetComponent<Interactable> ();
        if (interact != null)
        {
            AllDisks.Add (other.gameObject);
            Debug.Log ("Enter --->" + other.gameObject.name);
        }

    }

    /// <summary>
    /// On object exits tower ---> This step is required to check on all the objects
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit (Collider other)
    {
        Interactable interact = other.GetComponent<Interactable> ();
        if (interact != null)
        {
            AllDisks.Remove (other.gameObject);
            Debug.Log ("Exit --->" + other.gameObject.name);
        }
    }

    private void OnTriggerStay (Collider other)
    {
        //Debug.Log ("Stay --->" + other.gameObject.name);
    }
}
