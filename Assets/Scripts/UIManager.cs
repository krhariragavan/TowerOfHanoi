using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header ("Scene Ref")]
    public GameObject PlayButtonObj;
    public GameObject InGameMenuObj;
    public Text MoveCountText;
    public Text BestMoveText;
    public Text TimerText;
    public Text InvalidMoveText;

    DateTime StartTime;
    TimeSpan TimeNow;


    public static UIManager Instance;

    private void Awake ()
    {
        Instance = this;
    }
    void Start ()
    {
        StartTime = DateTime.Now;
        //Invoke ("RestartGame", 2f);
    }

    void Update ()
    {
        SetTimerText ();
        SetMoveCount ();
        //SetBestMoveText ();
    }

    void SetTimerText ()
    {
        TimeNow = DateTime.Now - StartTime;
        TimerText.text = TimeNow.ToString ();
    }

    void SetMoveCount ()
    {
        MoveCountText.text = Game.Instance.MoveCount.ToString ();
    }

    public void SetBestMoveText ()
    {
        int bestMove = Game.Instance.GetBestMove ();
        if (bestMove > 0)
            BestMoveText.text = bestMove.ToString ();
        else
            BestMoveText.text = "Set your best move";
    }

    // Buttons
    public void PlayButton ()
    {
        // Disable the player move
        PlayerMovement.Instance.DisablePlayerMove ();
        // Set Camera angle
        PlayerMovement.Instance.SetCameraAngle (true);

        // Start game

        // Hide play button
        PlayButtonObj.SetActive (false);

        // Enable In game UI
    }

    public void ExitGameButton ()
    {
        // Enabling back the player movement
        PlayerMovement.Instance.EnablePlayerMove ();

        // Setting camera angle back from where it started while pressing play button
        PlayerMovement.Instance.SetCameraAngle (false);

        // Show Play button
        PlayButtonObj.SetActive (true);
    }

    public void RestartGame ()
    {
        Debug.Log ("Restarting Game");
        Game.Instance.StartGame ();
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
