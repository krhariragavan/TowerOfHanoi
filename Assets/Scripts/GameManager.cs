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
    public GameObject OriginTower;
    public GameObject VictoryTower1;
    public GameObject VictoryTower2;
    // Scene Ref -------->

    [Header ("Set Value")]
    // Variables ---------->
    public int TotalCount;
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
        SetClickedObj ();
    }

    /// <summary>
    /// Setting up on click values
    /// </summary>
    void SetClickedObj ()
    {
        if (Input.GetMouseButton (0))
        {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit raycastHit;

            if (Physics.Raycast (ray, out raycastHit, 100f))
            {
                Interactable interact = raycastHit.collider.GetComponent<Interactable> ();
                if (interact != null)
                {
                    if (interact.CanMove) // Returns true only if it is a top piece
                    {
                        ClickedObject = interact.gameObject;
                    }
                }
            }
        }
    }


}
