using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    string PlayerPrefsKey
    {
        get
        {
            return "Best" + DiskCount;
        }
    }

    // Has player made a first click
    public bool HasStarted; // becomes true on player clicks on tower

    [HideInInspector] public int CurrentMoveIndex;

    // Singleton Instance of the script
    public static Game Instance;

    private void Awake ()
    {
        Instance = this;
    }

    void Start ()
    {
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
    }

    void AddDisks ()// Add Disks
    {
        UIManager.Instance.SetBestMoveText ();

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

        for (int i = 0; i < AllDisks.Count; i++)
        {
            //Disk DiskInst = AllDisks [i];
            //AllDisks.Remove (DiskInst);

            GameObject obj = AllDisks [i];
            Destroy (obj);
        }

        AllDisks.RemoveAll (delegate (GameObject d) { return d == null; }); // Removing all values from list

        AllDisks.Clear (); // Clearing again to make sure list is 100% clear
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

    void Update ()
    {
        if (Input.GetMouseButtonDown (0)) // On left click the tower
        {
            if (HasStarted)
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
            else
            {
                //HasStarted = true;
                SelectedTower = OnClickSelect (true);
                HasStarted = SelectedTower != null; // Setting up if it has started if selectedtower not null game has started
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
    public void SaveBestMove ()
    {
        PlayerPrefs.SetInt (PlayerPrefsKey, MoveCount);
    }

    // Getting exsisting best moves
    public int GetBestMove ()
    {
        return PlayerPrefs.GetInt (PlayerPrefsKey);
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