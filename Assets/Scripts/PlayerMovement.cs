using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this script in Main Camera
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float Speed = 5.0f;

    public float lookSpeed = 3;
    Vector2 rotation = Vector2.zero;

    bool IsEnabled; // Enable and disable player move

    // Camera position for the game
    Vector3 CurrentPos; // Camera position before entering the game
    public Vector3 TowerOfHanoiGamePos;

    public static PlayerMovement Instance;

    private void Awake ()
    {
        Instance = this;
    }

    void Update ()
    {
        if (!IsEnabled) return;
        PlayerMove ();
        MouseLook ();
    }

    void PlayerMove ()
    {
        var horizontal = Input.GetAxis ("Horizontal");
        var vertical = Input.GetAxis ("Vertical");
        transform.Translate (new Vector3 (horizontal, 0, vertical) * (Speed * Time.deltaTime));
    }

    void MouseLook () // Look rotation (UP down is Camera) (Left right is Transform rotation)
    {
        rotation.y += Input.GetAxis ("Mouse X");
        rotation.x += -Input.GetAxis ("Mouse Y");
        Camera.main.transform.localRotation = Quaternion.Euler (rotation.x * lookSpeed, rotation.y * lookSpeed, 0);
    }

    // On Start Tower of Hanoi game
    public void SetCameraAngle (bool IsEnterTheGame) // While enter the game --> go to game position // while exit the game go to previous position
    {
        if (IsEnterTheGame)
        {
            CurrentPos = transform.position;
            transform.DOMove (TowerOfHanoiGamePos, 1f); // Animation time is 1
        }
        else
        {
            transform.DOMove (CurrentPos, 1f);
        }
    }

    public void EnablePlayerMove ()
    {
        IsEnabled = true;
    }

    public void DisablePlayerMove ()
    {
        IsEnabled = false;
    }
}