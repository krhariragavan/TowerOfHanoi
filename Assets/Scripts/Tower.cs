using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public bool IsVictoryTower; // Except orgin tower other two towers are marked as vicotry tower in inspector
    public List<GameObject> TowerContains = new List<GameObject> (); // List of all objects that is placed in the tower

    void Start ()
    {

    }

    void Update ()
    {
        if (IsVictoryTower)
        {
            if (!GameManager.Instance.IsWrongMove)
            {
                if (TowerContains.Count == GameManager.Instance.TotalCount)
                {
                    GameManager.Instance.VictoryAchieved ();
                }
            }
        }
    }

    private void OnTriggerEnter (Collider other)
    {
        Interactable interact = other.GetComponent<Interactable> ();
        if (interact != null)
        {
            TowerContains.Add (other.gameObject);
            Debug.Log ("Enter --->" + other.gameObject.name);
        }

    }

    private void OnTriggerExit (Collider other)
    {
        Interactable interact = other.GetComponent<Interactable> ();
        if (interact != null)
        {
            TowerContains.Remove (other.gameObject);
            Debug.Log ("Exit --->" + other.gameObject.name);
        }
    }

    private void OnTriggerStay (Collider other)
    {
        //Debug.Log ("Stay --->" + other.gameObject.name);
    }
}
