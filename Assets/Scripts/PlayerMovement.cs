using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this script in Main Camera
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float runSpeed = 8.0F;
    public float gravity = 20.0F;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;

    [SerializeField]
    private float Speed = 5.0f;

    public float lookSpeed = 3;
    Vector2 rotation = Vector2.zero;

    bool IsEnabled; // Enable and disable player move

    // Camera position for the game
    Vector3 CurrentPos; // Camera position before entering the game
    Vector3 CurrentRot; // Current rotation in euler angles

    public Vector3 TowerOfHanoiGamePos;

    public static PlayerMovement Instance;

    private void Awake ()
    {
        Instance = this;
    }

    private void Start ()
    {
        controller = GetComponent<CharacterController> ();
        EnablePlayerMove ();
    }

    void Update ()
    {
        if (!IsEnabled) return;
        PlayerMove ();
        MouseLook ();
    }

    void PlayerMove ()
    {
        //var horizontal = Input.GetAxis ("Horizontal");
        //var vertical = Input.GetAxis ("Vertical");
        //transform.Translate (new Vector3 (horizontal, 0, vertical) * (Speed * Time.deltaTime));

        if (controller.isGrounded)
        {
            moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
            moveDirection = transform.TransformDirection (moveDirection);
            moveDirection *= walkSpeed;
            if (Input.GetButton ("Jump"))
                moveDirection.y = jumpSpeed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move (moveDirection * Time.deltaTime);
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
            CurrentRot = transform.rotation.eulerAngles;

            transform.DOMove (TowerOfHanoiGamePos, 1f); // Animation time is 1
            transform.DORotate (Vector3.zero, 1);
        }
        else
        {
            transform.DOMove (CurrentPos, 1f);
            transform.DORotate (CurrentRot, 1f);
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