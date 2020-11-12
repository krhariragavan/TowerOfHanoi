﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header ("Scene Ref")]
    [SerializeField] GameObject PlayMenuObj;
    [SerializeField] GameObject InGameMenuObj;
    [SerializeField] Text MoveCountText;
    [SerializeField] Text BestMoveText;
    [SerializeField] Text TimerText;
    [SerializeField] Text InvalidMoveText;
    [SerializeField] Text DiskCountDisplayText;
    [SerializeField] GameObject WonTheGame;

    DateTime StartTime;
    TimeSpan TimeNow;

    [HideInInspector]
    public bool IsGameStarted;

    public static UIManager Instance;

    private void Awake ()
    {
        Instance = this;
    }

    void Start ()
    {
        InGameMenuObj.SetActive (false);
        
        //DisplayInvalidMove ();
        SetDiskCountDisplayText ();
        //Invoke ("RestartGame", 2f);
        WonTheGame.gameObject.SetActive (false); // Hiding the won the game text
    }

    void Update ()
    {
        if (IsGameStarted)
        {
            SetTimerText ();
            SetMoveCount ();
        }
        //SetBestMoveText ();
    }

    void SetTimerText ()
    {
        TimeNow = DateTime.Now - StartTime;
        TimerText.text = TimeNow.ToString ();
    }

    void SetMoveCount ()
    {
        MoveCountText.text = "Move Count - " + Game.Instance.MoveCount.ToString ();
    }

    public void SetBestMoveText ()
    {
        int bestMove = Game.Instance.GetBestMove ();
        if (bestMove > 0)
            BestMoveText.text = "Best Move - " + bestMove.ToString ();
        else
            BestMoveText.text = "Set your best move";
    }

    // Buttons
    public void PlayButton ()
    {
        IsGameStarted = true;
        StartTime = DateTime.Now;
        // Disable the player move
        PlayerMovement.Instance.DisablePlayerMove ();
        // Set Camera angle
        PlayerMovement.Instance.SetCameraAngle (true);
        // Setting invalid move text to false
        InvalidMoveText.gameObject.SetActive (false);
        // Start game
        Game.Instance.StartGame ();
        // Hide play button
        PlayMenuObj.SetActive (false);
        // Enable In game UI
        InGameMenuObj.SetActive (true);
        // Setting display count text
        SetDiskCountDisplayText ();

        SetBestMoveText (); // Setting best move text
        WonTheGame.gameObject.SetActive (false); // Hiding the won the game text
    }

    public void ExitGameButton ()
    {
        // Enabling back the player movement
        PlayerMovement.Instance.EnablePlayerMove ();

        // Setting camera angle back from where it started while pressing play button
        PlayerMovement.Instance.SetCameraAngle (false);

        // Setting ingame menu off
        InGameMenuObj.SetActive (false);

        // Show Play button
        PlayMenuObj.SetActive (true);
    }

    public void RestartGame ()
    {
        Debug.Log ("Restarting Game");
        Game.Instance.StartGame ();
        StartTime = DateTime.Now;
        SetBestMoveText (); // Setting best move text
        WonTheGame.gameObject.SetActive (false); // Hiding the won the game text
    }

    public void Undo ()
    {
        if (Game.Instance.CurrentMoveIndex >= 0)
        {
            Game.Instance.AllMoves [Game.Instance.CurrentMoveIndex].Undo ();
            Game.Instance.CurrentMoveIndex--;

            // Condition needed only if there is Redo
            //if (Game.Instance.CurrentMoveIndex != 0)
            //{
            //}

            Debug.Log (Game.Instance.CurrentMoveIndex);
        }
        else
        {
            Debug.Log ("NO undo MOVES");

            // No More Undo Moves
        }
    }

    public void DisplayInvalidMove ()
    {
        InvalidMoveText.gameObject.SetActive (true);
        Vector3 CurrentPos = InvalidMoveText.transform.position;
        InvalidMoveText.transform.position = new Vector3 (CurrentPos.x, CurrentPos.y, 10f);
        InvalidMoveText.transform.DOMove (new Vector3 (CurrentPos.x, CurrentPos.y, 0), 1f);

        StartCoroutine (HideInvalidMoveText ());
    }

    IEnumerator HideInvalidMoveText ()
    {
        yield return new WaitForSeconds (2f);
        InvalidMoveText.gameObject.SetActive (false);
    }

    void SetDiskCountDisplayText () // Used while starting of the game
    {
        DiskCountDisplayText.text = Game.Instance.DiskCount.ToString ();
    }

    public void SetDiskCount (bool IsPlusButton) // Set in inspector for plus and minus button
    {
        if (IsPlusButton)
        {
            if (Game.Instance.DiskCount < 10) // Max disk count
            {
                Game.Instance.DiskCount++;
            }
            SetDiskCountDisplayText ();
        }
        else
        {
            if (Game.Instance.DiskCount > 2) // Minimum disk count
            {
                Game.Instance.DiskCount--;
            }
            SetDiskCountDisplayText ();
        }
    }

    public void DisplayWinText ()
    {
        WonTheGame.gameObject.SetActive (true);
    }

    //public void Redo ()
    //{
    //    if (Game.Instance.CurrentMoveIndex < Game.Instance.AllMoves.Count)
    //    {
    //        Game.Instance.AllMoves [Game.Instance.CurrentMoveIndex].Execute ();

    //        if (Game.Instance.CurrentMoveIndex < Game.Instance.AllMoves.Count - 1)
    //        {
    //            Game.Instance.CurrentMoveIndex++;
    //        }
    //        Debug.Log (Game.Instance.CurrentMoveIndex);
    //    }
    //    else
    //    {
    //        Debug.Log ("NO REDO MOVES");
    //        // No More Redo moves
    //    }
    //}
}
