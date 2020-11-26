using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header ("Scene Ref")]
    [SerializeField] GameObject PlayMenuObj; // Play menu before the game starts
    [SerializeField] GameObject InGameMenuObj; // In Game menu - after the game starts
    [SerializeField] Text MoveCountText; // Current move count text
    [SerializeField] Text BestMoveText; // Best move count text
    [SerializeField] Text TimerText; // Timer text
    [SerializeField] Text BestTimerText; // Best Timer Text
    [SerializeField] GameObject InvalidMoveBG; // Invalid move text
    [SerializeField] Text DiskCountDisplayText; // disk count display before the game starts
    [SerializeField] GameObject WonTheGame; // Won the game text
    [SerializeField] GameObject UndoMsgObj;

    //DateTime StartTime;
    //TimeSpan TimeNow;

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
        if (IsGameStarted) // Set timer and move count only when the game is started
        {
            SetTimerText ();
            SetMoveCount ();
        }
        //SetBestMoveText ();
    }

    // Set timer text value
    void SetTimerText ()
    {
        //Game.Instance.TimeNow = DateTime.Now - Game.Instance.StartTime;
        TimerText.text = Game.Instance.TimeNow.ToString ();
    }

    // Setting move count value in the UI Text
    void SetMoveCount ()
    {
        MoveCountText.text = "Move Count - " + Game.Instance.MoveCount.ToString ();
    }

    // Setting best move count value in the UI Text
    public void SetBestMoveOrTimerText ()
    {
        int bestMove = Game.Instance.BestMove;
        int BestTimer = Game.Instance.BestTimerInSeconds;

        if (bestMove > 0)
            BestMoveText.text = "Best Move - " + bestMove.ToString ();
        else
            BestMoveText.text = "Set your best move";

        if (BestTimer > 0)
            BestTimerText.text = "Record Time - " + BestTimer.ToString ();
        else
            BestTimerText.text = "Set your best time";

    }

    // Buttons
    #region Buttons
    public void PlayButton ()
    {
        IsGameStarted = true;
        Game.Instance.StartTime = DateTime.Now;
        // Disable the player move
        PlayerMovement.Instance.DisablePlayerMove ();
        // Set Camera angle
        PlayerMovement.Instance.SetCameraAngle (true);
        // Setting invalid move text to false
        InvalidMoveBG.gameObject.SetActive (false);
        // Undo Msg UI
        UndoMsgObj.SetActive (false);
        // Start game
        Game.Instance.StartGame ();
        // Hide play button
        PlayMenuObj.SetActive (false);
        // Enable In game UI
        InGameMenuObj.SetActive (true);
        // Setting display count text
        SetDiskCountDisplayText ();

        SetBestMoveOrTimerText (); // Setting best move text
        WonTheGame.gameObject.SetActive (false); // Hiding the won the game text
    }

    public void ExitGameButton ()
    {
        IsGameStarted = false;

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
        IsGameStarted = true;
        Debug.Log ("Restarting Game");
        Game.Instance.StartGame ();
        Game.Instance.StartTime = DateTime.Now;
        SetBestMoveOrTimerText (); // Setting best move text
        WonTheGame.gameObject.SetActive (false); // Hiding the won the game text
    }

    public void Undo ()
    {
        if (!IsGameStarted) return;

        if (Game.Instance.MoveCount < 1)
        {
            // No Undo moves
            //DisplayMsg (UndoMsgObj);
            UndoMsgObj.SetActive (true);
            StartCoroutine (HideInvalidMoveText (UndoMsgObj));
            return;
        }

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
            // No More Undo Moves
            Debug.Log ("NO undo MOVES");
            //DisplayMsg (UndoMsgObj);
            UndoMsgObj.SetActive (true);
            StartCoroutine (HideInvalidMoveText (UndoMsgObj));
            return;
        }
    }
    #endregion

    void DisplayMsg (GameObject Obj)
    {
        Obj.gameObject.SetActive (true);
        Vector3 CurrentPos = Obj.transform.position;
        Obj.transform.position = new Vector3 (CurrentPos.x, CurrentPos.y, 10f);
        Obj.transform.DOMove (new Vector3 (CurrentPos.x, CurrentPos.y, 0), .4f);
        StartCoroutine (HideInvalidMoveText (Obj));
    }

    public void DisplayInvalidMove ()
    {
        DisplayMsg (InvalidMoveBG);

        //InvalidMoveBG.gameObject.SetActive (true);
        //Vector3 CurrentPos = InvalidMoveBG.transform.position;
        //InvalidMoveBG.transform.position = new Vector3 (CurrentPos.x, CurrentPos.y, 10f);
        //InvalidMoveBG.transform.DOMove (new Vector3 (CurrentPos.x, CurrentPos.y, 0), 1f);

        //StartCoroutine (HideInvalidMoveText (InvalidMoveBG.gameObject));
    }

    IEnumerator HideInvalidMoveText (GameObject Obj)
    {
        yield return new WaitForSeconds (2f);
        Obj.SetActive (false);
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

    public void DisplayWinText (string winmsg)
    {
        Text text = WonTheGame.GetComponentInChildren<Text> ();
        text.text = "YOU WON THE GAME.\n\n" + winmsg;

        WonTheGame.gameObject.SetActive (true);
        Vector3 CurrentPos = WonTheGame.transform.position;
        WonTheGame.transform.position = new Vector3 (CurrentPos.x, CurrentPos.y, 10f);
        WonTheGame.transform.DOMove (new Vector3 (CurrentPos.x, CurrentPos.y, -1), .4f);
    }

    // Future expansion if needed...
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
