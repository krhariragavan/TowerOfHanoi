using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    // Runtime Ref -->
    [Header ("Runtim Ref")]
    public Tower SelectedTower; // On click this 
    public int MoveCount;
    public List<GameObject> AllDisks = new List<GameObject> ();
    public List<IMove> AllMoves = new List<IMove> ();
    // Runtime Ref -->

    // Scene Ref -->
    [Header ("Scene Ref")]
    public Tower [] AllTowers;
    public int DiskCount;
    public const float DiskThickness = 0.15f;
    public Color [] DiskColors;
    // Scene Ref -->

    // Project Ref --> 
    [Header ("Project Ref")]
    public GameObject DiskPrefabObj;
    // Project Ref --> 

    // Key for saving playerprefs based on diskcount
    string PlayerPrefsKey_BestMove
    {
        get
        {
            return "BestMove" + DiskCount;
        }
    }

    string PlayerPrefsKey_BestTimer
    {
        get
        {
            return "BestTimer" + DiskCount;
        }
    }



    // Has player made a first click
    public bool HasStarted; // becomes true on player clicks on tower

    // Click and drag the peg
    Tower FromTower, ToTower;
    Vector3 DiskDefaultPos;
    Disk MoveableDisk;
    // Click and drag the peg

    bool IsGameOver  // Check if player can make any more moves
    {
        get { return !UIManager.Instance.IsGameStarted; }
    }

    // Best Move and Timer -->
    //[HideInInspector] public int CurrentTimerInSeconds; // TimeNow.Seconds returns the same value
    [HideInInspector] public int BestMove;
    [HideInInspector] public int BestTimerInSeconds;
    [HideInInspector]
    public int MinBestMoveCount
    {
        get
        {
            return (int) Mathf.Pow (2, DiskCount) - 1;
        }
    }
    // Best Move and Timer -->

    // Move timer enables click and drag function only when timer crosses 0.4 secs. 
    // DOMove animation timing is 0.4f. This step is done only coz if user tries to move same peg twice within 0.4 sec
    float MoveTimer;

    // Timer
    [HideInInspector] public DateTime StartTime;
    [HideInInspector] public TimeSpan TimeNow;

    // Saving move index for undo purpose
    [HideInInspector] public int CurrentMoveIndex;

    // Singleton Instance of the script
    public static Game Instance;

    private void Awake ()
    {
        Instance = this;
    }

    void Start ()
    {
        //PlayerPrefs.DeleteAll (); // USED FOR TESTING
        //StartGame (); // USED FOR TESTING
    }

    // Start the game 
    public void StartGame ()
    {
        if (AllDisks.Count > 0)
        {
            RemoveDisks ();
            AddDisks ();
        }
        else
        {
            AddDisks ();
        }

        GetBestMoveAndTimer ();
    }

    void AddDisks ()// Add Disks
    {
        UIManager.Instance.SetBestMoveOrTimerText ();

        MoveCount = 0; // Setting up move count to 0 - default start value
        Tower originTower = AllTowers [0];

        for (int i = DiskCount; i > 0; i--)
        {
            // Instantiate disks based on count decided by the user
            GameObject Inst = Instantiate (DiskPrefabObj);
            int Size = (DiskCount - i + 1); // Setting up disk size
            Inst.name = Size.ToString ();

            // Setting Disk initial Position 
            Vector3 pos = originTower.TowerOriginTransform.position;
            float yPos = pos.y + (i * (DiskThickness + 0.005f)) - 0.218f; // - 0.1875f; // 0.12 is the value of disk thickness // 0.1875 is the value for the first disk 
            Inst.transform.position = new Vector3 (pos.x, yPos, pos.z);

            // Setting initial scale
            Vector3 localScale = Inst.transform.localScale;
            float scaleValue = localScale.x + (Size * 4f);
            Inst.transform.localScale = new Vector3 (scaleValue, localScale.y, scaleValue);

            // Setting disk value
            Disk disk = Inst.GetComponent<Disk> ();

            AllDisks.Add (Inst);

            // Setting Random Colors for disks
            int RandomColor = Random.Range (0, DiskColors.Length);
            Color color = DiskColors [RandomColor];
            disk.SetColor (color);

            originTower.AddDisk (disk);
            disk.Size = Size;
        }
    }

    void RemoveDisks () // Destroy all exsisting disks
    {
        Debug.Log ("Removing");

        //AllDisks.RemoveAll (item => item == null);

        //AllDisks.RemoveAll (delegate (GameObject o) { return o == null; });

        if (AllDisks.Count > 0)
        {
            for (int i = 0; i < AllDisks.Count; i++)
            {
                //Disk DiskInst = AllDisks [i];
                //AllDisks.Remove (DiskInst);

                GameObject obj = AllDisks [i];
                Destroy (obj);
            }

            AllDisks.RemoveAll (delegate (GameObject d) { return d == null; }); // Removing all values from list

            AllDisks.Clear (); // Clearing again to make sure list is 100% clear
        }

        AllDisks = new List<GameObject> (); // Creating a new instance of the list

        foreach (Tower tower in AllTowers)
        {
            tower.RemoveAllDisk (); // Removing all disks from tower
        }
    }

    //void AddDisks ()
    //{
    //    Tower originTower = AllTowers [0];

    //    for (int i = 1; i <= DiskCount; i++)
    //    {
    //        GameObject Inst = Instantiate (DiskPrefabObj);


    //        // Setting instance position
    //        Vector3 OriginPos = originTower.TowerOriginTransform.position;
    //        //float YPos = i * (Inst.transform.localScale.y + 0.1f) - (DiskCount - i) * Inst.transform.localScale.y; //new Vector3 (OriginPos.x, OriginPos.y + ((i - 1) + Inst.transform.localScale.y + 0.1f), OriginPos.z);
    //        float YPos = (DiskCount - i) * Inst.transform.localScale.y + (i * Inst.transform.localScale.y * 3);
    //        Inst.transform.position = new Vector3 (OriginPos.x, OriginPos.y - YPos, OriginPos.z);

    //        // Setting up disk size
    //        float size = i * 0.2f;
    //        Vector3 scale = new Vector3 (size, 0, size);
    //        Inst.transform.localScale = Inst.transform.localScale + scale;

    //        // Setting up disk value
    //        Disk diskInst = Inst.GetComponent<Disk> ();
    //        diskInst.Size = i;

    //        // Adding disk to tower
    //        originTower.AddDisk (diskInst);
    //    }
    //    //originTower.AddDisk ()
    //}

    //void AddDisksReverse ()
    //{
    //    Tower originTower = AllTowers [0];

    //    for (int i = DiskCount; i > 0; i--)
    //    {
    //        GameObject Inst = Instantiate (DiskPrefabObj);

    //        // Setting instance position
    //        Vector3 OriginPos = originTower.TowerOriginTransform.position;
    //        float yPos = i * (Inst.transform.localScale.y + 0.1f); // + DiskCount;
    //        Vector3 Pos = new Vector3 (OriginPos.x, OriginPos.y - yPos, OriginPos.z);
    //        Inst.transform.position = Pos;

    //        // Setting up disk size
    //        float size = i * 0.2f;
    //        Vector3 scale = new Vector3 (size, 0, size);
    //        Inst.transform.localScale = Inst.transform.localScale - scale;

    //        // Setting up disk value
    //        Disk diskInst = Inst.GetComponent<Disk> ();
    //        diskInst.Size = i;

    //        // Adding disk to tower
    //        originTower.AddDisk (diskInst);
    //    }
    //    //originTower.AddDisk ()
    //}

    void SetTimer ()
    {
        TimeNow = DateTime.Now - StartTime;
        //TimerText.text = TimeNow.ToString ();
    }

    void Update ()
    {
        // Click on two towers to make the move
        //ClickOnTwoTowers (); // Commented to change its functionality/experience

        if (!IsGameOver)
        {
            SetTimer ();
            MoveTimer += Time.deltaTime;

            if (MoveTimer > 0.4f)
            {
                OnClickAndDrag (); // click and drag peg
            }
        }

        // Key inputs for starting the game
        if (Input.GetKeyDown (KeyCode.Return))
        {
            UIManager.Instance.PlayButton ();
        }
        // Key inputs for decreasing the disk/peg count
        if (Input.GetKeyDown (KeyCode.Alpha0))
        {
            UIManager.Instance.SetDiskCount (false);
        }
        // Key inputs for increasing the disk/peg count
        if (Input.GetKeyDown (KeyCode.Alpha1))
        {
            UIManager.Instance.SetDiskCount (true);
        }
    }

    /// <summary>
    /// Click on tower one and tower two to make a move
    /// </summary>
    void ClickOnTwoTowers ()
    {
        if (Input.GetMouseButtonDown (0)) // On left click the tower
        {
            if (HasStarted) // 2nd tower
            {
                Tower tower = OnClickSelect (false); // Getting which tower has been clicked

                if (tower != null)
                {
                    if (SelectedTower != tower) // Check if user selected same towers for moving the disk
                    {
                        Disk MoveableDisk = SelectedTower.GetMoveableDiskBySize ();

                        if (tower.AllDisks.Count > 0)
                        {
                            if (MoveableDisk.Size < tower.GetMoveableDiskBySize ().Size)
                            {
                                MoveDiskWithUndo (SelectedTower, tower); // Moving the disk to next tower
                                //MoveDisk (MoveableDisk, tower);
                                //SelectedTower.RemoveDisk (MoveableDisk); // Migrated to Move.cs
                            }
                            else // This block is for INVALID MOVE
                            {
                                UIManager.Instance.DisplayInvalidMove ();
                                // Setting default color to towers
                                SelectedTower.SetTowerColor (true);
                                tower.SetTowerColor (true);
                                // Debug msg
                                Debug.Log ("Invalid MOVE");
                            }
                        }
                        else
                        {
                            MoveDiskWithUndo (SelectedTower, tower); // Moving the disk to next tower
                            //MoveDisk (MoveableDisk, tower);
                            //SelectedTower.RemoveDisk (MoveableDisk); // Migrated to Move.cs
                        }
                    }
                    else
                    {
                        Debug.Log ("Selected Same Tower");
                    }

                    SelectedTower = null;
                    HasStarted = false;
                }
            }
            else // 1st tower
            {
                //HasStarted = true;
                SelectedTower = OnClickSelect (true);
                HasStarted = SelectedTower != null; // Setting up if it has started if selectedtower not null game has started
            }
        }
    }

    /// <summary>
    /// Click and drag the peg to make a move
    /// </summary>
    void OnClickAndDrag ()
    {
        if (Input.GetMouseButtonDown (0))
        {
            FromTower = OnClickSelect (true);

            if (FromTower != null)
            {
                MoveableDisk = FromTower.GetMoveableDiskBySize ();
                if (MoveableDisk != null)
                {
                    Debug.Log ("Mouse Down, --> " + FromTower.gameObject.name);
                    DiskDefaultPos = MoveableDisk.transform.position;
                }
            }
        }
        else if (Input.GetMouseButton (0))
        {
            //float Zpos = FromTower.GetMoveableDiskBySize ().transform.position.z;

            if (FromTower != null)
            {
                if (MoveableDisk != null)
                {
                    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast (ray, out hit, Mathf.Infinity))
                    {
                        Vector3 pos = hit.point; //ray.origin;
                        MoveableDisk.transform.position = new Vector3 (pos.x, pos.y, 0);// + CurrPos;
                    }
                }
                //Vector3 MovePos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0));
                //FromTower.GetMoveableDiskBySize ().transform.position = MovePos;
                //Vector3 CurrPos = FromTower.GetMoveableDiskBySize ().transform.position;
            }
        }
        else if (Input.GetMouseButtonUp (0))
        {
            ToTower = OnClickSelect (false);
            if (ToTower != null)
            {
                Debug.Log ("Mouse Up, --> " + ToTower.gameObject.name);

                if (MoveableDisk != null)
                {
                    if (ToTower.AllDisks.Count > 0)
                    {
                        if (MoveableDisk.Size < ToTower.GetMoveableDiskBySize ().Size)
                        {
                            MoveDiskWithUndo (FromTower, ToTower); // Moving the disk to next tower
                            //return;
                            //MoveDisk (MoveableDisk, tower);
                            //SelectedTower.RemoveDisk (MoveableDisk); // Migrated to Move.cs
                        }
                        else // This block is for INVALID MOVE
                        {
                            UIManager.Instance.DisplayInvalidMove ();
                            // Setting default color to towers
                            FromTower.SetTowerColor (true);
                            ToTower.SetTowerColor (true);
                            // Debug msg
                            MoveTimer = 0;
                            MoveableDisk.transform.DOMove (DiskDefaultPos, 0.4f);
                            Debug.Log ("Invalid MOVE");
                            //return;
                        }
                    }
                    else
                    {
                        MoveDiskWithUndo (FromTower, ToTower); // Moving the disk to next tower
                        //MoveDisk (MoveableDisk, tower);
                        //SelectedTower.RemoveDisk (MoveableDisk); // Migrated to Move.cs
                    }
                }

                // Moving disk with undo
                //MoveDiskWithUndo (FromTower, ToTower);
            }
            else
            {
                if (FromTower != null)
                {
                    if (MoveableDisk != null)
                    {
                        MoveTimer = 0;
                        MoveableDisk.transform.DOMove (DiskDefaultPos, 0.4f);
                        Debug.Log ("GO back to Previous Tower -->" + FromTower.gameObject.name);
                    }
                }
            }
            FromTower = null;
            ToTower = null;
            MoveableDisk = null;
        }
    }

    void MoveDiskWithUndo (Tower fromtower, Tower totower)
    {
        IMove move = new Move (fromtower, totower); // All move functionalities implemented in Move.cs
        move.Execute (); // Move values is recorded for undo purpose

        // Setting default tower color
        fromtower.SetTowerColor (true);
        // Setting default tower color
        totower.SetTowerColor (true);

        // CurrentMoveIndex is used for UNDO
        CurrentMoveIndex = AllMoves.Count;
        // Adding all moves in a list of iMove for UNDO
        AllMoves.Add (move);
    }

    // On drag move the peg from tower 1 to tower 2
    //Tower OnDragMove ()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
    //    RaycastHit hit;

    //    if (Physics.Raycast (ray, out hit, 100f))
    //    {
    //        Tower tower = hit.collider.GetComponent<Tower> ();

    //        if (tower != null)
    //        {
    //            tower.SetTowerColor (false);

    //            return tower;
    //            //if (WithDisk)
    //            //{
    //            //    if (tower.AllDisks.Count > 0)
    //            //    {
    //            //        return tower; // Returns clicked tower object
    //            //    }
    //            //}
    //            //else
    //            //{
    //            //    return tower;
    //            //}
    //        }
    //    }
    //    return null;
    //}

    // On click Select the Tower

    Tower OnClickSelect (bool WithDisk)
    {
        // Raycasting to find, click on the tower
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast (ray, out hit, 100f))
        {
            Tower tower = hit.collider.GetComponent<Tower> ();

            if (tower != null)
            {
                tower.SetTowerColor (false);

                if (WithDisk)
                {
                    if (tower.AllDisks.Count > 0)
                    {
                        return tower; // Returns clicked tower object
                    }
                }
                else
                {
                    return tower;
                }
            }
        }
        return null;
    }

    // Saving the best move locally
    public string SaveBestMoveAndTimer ()
    {
        string UIMsg = "";
        // ------> Best Move
        if (PlayerPrefs.HasKey (PlayerPrefsKey_BestMove))
        {
            //Debug.Log ("Min Count = " + MinBestMoveCount);
            //Debug.Log ("Move Count = " + MoveCount);

            if (MoveCount == MinBestMoveCount)
            {
                // Show UI
                string Msg = "Congrats, You have completed the game in minimum possible moves";
                UIMsg += Msg;

                PlayerPrefs.SetInt (PlayerPrefsKey_BestMove, MoveCount);
                Debug.Log (Msg);
            }
            else if (MoveCount < BestMove)
            {
                string Msg = "Congrats, This is your personal Best move, But you can complete the game in "
                    + MinBestMoveCount + "moves!";
                UIMsg += Msg;

                PlayerPrefs.SetInt (PlayerPrefsKey_BestMove, MoveCount);
                Debug.Log (Msg);
                // Show UI
            }
            else
            {
                string Msg = "You can complete this in " + MinBestMoveCount + " moves";
                UIMsg += Msg;

                Debug.Log (Msg);
                // Show UI
            }
        }
        else
        {
            if (MoveCount == MinBestMoveCount)
            {
                // Show UI
                string Msg = "Congrats, You have completed the game in minimum possible moves";
                UIMsg += Msg;

                PlayerPrefs.SetInt (PlayerPrefsKey_BestMove, MoveCount);
                Debug.Log (Msg);
            }
            else
            {
                string Msg = "This is your Best Move " + MoveCount;
                UIMsg += Msg;

                PlayerPrefs.SetInt (PlayerPrefsKey_BestMove, MoveCount);
                Debug.Log (Msg);
            }

        }
        // --------->

        // ---> Best Timer
        if (PlayerPrefs.HasKey (PlayerPrefsKey_BestTimer))
        {
            if (BestTimerInSeconds > 0)
            {
                string Msg = "";
                if (TimeNow.Seconds == BestTimerInSeconds)
                {
                    Msg = "You have completed in same time";
                    Debug.Log (Msg);
                }
                else if (TimeNow.Seconds < BestTimerInSeconds)
                {
                    PlayerPrefs.SetInt (PlayerPrefsKey_BestTimer, TimeNow.Seconds);
                    Msg = "Congrats, You have completed Quicker than the last time";
                    Debug.Log (Msg);
                }
                else
                {
                    Msg = "Previously you had completed the same puzzle in " + BestTimerInSeconds + " seconds";
                    Debug.Log (Msg);
                }
                UIMsg += "\n\n" + Msg;
            }
        }
        else
        {
            string Msg = "Congrats, You have set the best time";
            UIMsg += "\n\n" + Msg;
            PlayerPrefs.SetInt (PlayerPrefsKey_BestTimer, TimeNow.Seconds);
        }

        PlayerPrefs.Save ();
        return UIMsg;
        // ---> Best Timer
    }

    // Getting exsisting best moves
    void GetBestMoveAndTimer ()
    {
        if (PlayerPrefs.HasKey (PlayerPrefsKey_BestMove))
            BestMove = PlayerPrefs.GetInt (PlayerPrefsKey_BestMove);
        else
            BestMove = 0;

        if (PlayerPrefs.HasKey (PlayerPrefsKey_BestTimer))
            BestTimerInSeconds = PlayerPrefs.GetInt (PlayerPrefsKey_BestTimer);
        else
            BestTimerInSeconds = 0;

        Debug.Log ("Best move = " + BestMove);
    }

    #region Temp
    // Old Method for moving disk/peg without undo
    //void MoveDisk (Disk moveabledisk, Tower ToTower)
    //{
    //    // Setting disk position
    //    int diskcount = ToTower.AllDisks.Count;
    //    float ypos = (diskcount * DiskThickness * 1.1f) + 0.1f; // * 2f;
    //    Vector3 pos = ToTower.TowerOriginTransform.position;
    //    Vector3 ToPos = new Vector3 (pos.x, ypos, pos.z);

    //    // Disk move animation
    //    Sequence seq = DOTween.Sequence ();
    //    Vector3 TopPos = new Vector3 (ToPos.x, ToPos.y + 1.2f, ToPos.z);
    //    seq.Append (moveabledisk.transform.DOMove (TopPos, 0.4f));
    //    seq.Append (moveabledisk.transform.DOMove (ToPos, 0.4f));
    //    //MoveableDisk.transform.DOMove (ToPos, 1);
    //    ToTower.AddDisk (moveabledisk); // Play animation 

    //    MoveCount++;

    //    // Debug Msg
    //    Debug.Log ("Moving disk --> " + moveabledisk.name + " to Tower --> " + ToTower.name);
    //}
    #endregion
}