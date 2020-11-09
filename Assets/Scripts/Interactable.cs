using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool CanMove;
    public int Size; // proportional to number - 1 smallest and 10 is large

    void Start ()
    {

    }

    void Update ()
    {
        SetFirstPieceMoveable ();
    }

    /// <summary>
    /// Identify if this is a top piece and enable move. If player tries to make a wrong move --> Alert
    /// </summary>
    void SetFirstPieceMoveable ()
    {
        Ray ray = new Ray (transform.position, Vector3.up);
        RaycastHit hit;

        if (Physics.Raycast (ray, out hit, 100f))
        {
            Interactable interact = hit.collider.GetComponent<Interactable> ();

            if (interact != null)
            {
                CanMove = false;
                if (Size < interact.Size)
                {
                    GameManager.Instance.IsWrongMove = true;
                    // Change Color
                    // Reset Position
                    Debug.Log ("Wrong Move");
                }
                else
                {
                    GameManager.Instance.IsWrongMove = false;
                }
            }
            else
            {
                CanMove = true;
                GameManager.Instance.IsWrongMove = false;
            }
        }
        else
        {
            CanMove = true;
            GameManager.Instance.IsWrongMove = false;
        }
    }

    private void OnCollisionEnter (Collision collision)
    {
        Debug.Log ("Collision Enter");
        CanMove = false;
    }
}
