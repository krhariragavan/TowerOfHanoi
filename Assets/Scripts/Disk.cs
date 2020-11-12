using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disk : MonoBehaviour
{
    public bool CanMove;
    public int Size; // proportional to number 1 being the smallest and 10 being the large
    public GameObject CurrentTowerObj;
    //public Tower NewTower;
    //public GameObject DiskObj;
    public MeshRenderer DiskMat;

    void Start ()
    {
        //SetColor ();
    }

    void Update ()
    {
        //SetFirstPieceMoveable ();
    }

    public void SetColor (Color color)
    {
        //int RandomColor = Random.Range (0, Game.Instance.DiskColors.Length);
        //Color color = Game.Instance.DiskColors [RandomColor];
        DiskMat.material.color = color;
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
            Disk interact = hit.collider.GetComponent<Disk> ();

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
