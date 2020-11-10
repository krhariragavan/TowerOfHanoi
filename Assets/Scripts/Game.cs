using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    // Runtime Ref -->
    [Header ("Runtim Ref")]
    public Tower SelectedTower;
    // Runtime Ref -->

    // Scene Ref -->
    [Header ("Scene Ref")]
    public Tower [] AllTowers;
    public int DiskCount;
    const float DiskThickness = 0.12f;
    // Scene Ref -->

    // Project Ref --> 
    [Header ("Project Ref")]
    public GameObject DiskPrefabObj;
    // Project Ref --> 

    public bool HasStarted;

    void Start ()
    {
        //AddDisks ();
        //AddDisksReverse ();
        AddDisks ();
    }

    void AddDisks ()
    {
        Tower originTower = AllTowers [0];

        for (int i = DiskCount; i > 0; i--)
        {
            // Instantiate disks based on count decided by the user
            GameObject Inst = Instantiate (DiskPrefabObj);
            int Size = (DiskCount - i + 1); // Setting up disk size
            Inst.name = Size.ToString ();

            // Setting Disk initial Position 
            Vector3 pos = originTower.TowerOriginTransform.position;
            float yPos = pos.y + (i * (DiskThickness + 0.005f)); // - 0.1875f; // 0.12 is the value of disk thickness // 0.1875 is the value for the first disk 
            Inst.transform.position = new Vector3 (pos.x, yPos, pos.z);

            // Setting initial scale
            Vector3 localScale = Inst.transform.localScale;
            float scaleValue = localScale.x + (Size * 0.1f);
            Inst.transform.localScale = new Vector3 (scaleValue, localScale.y, scaleValue);

            // Setting disk value
            Disk disk = Inst.GetComponent<Disk> ();
            originTower.AddDisk (disk);
            disk.Size = Size;
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
        if (Input.GetMouseButtonDown (0))
        {
            if (HasStarted)
            {
                Tower tower = OnClickSelect (false);

                if (tower != null)
                {
                    if (SelectedTower != tower)
                    {
                        Disk MoveableDisk = SelectedTower.GetMoveableDiskBySize ();

                        if (tower.AllDisks.Count > 0)
                        {
                            if (MoveableDisk.Size < tower.GetMoveableDiskBySize ().Size)
                            {
                                MoveDisk (MoveableDisk, tower);
                                SelectedTower.RemoveDisk (MoveableDisk);

                                //Vector3 ToPos = tower.TowerOriginTransform.position;

                                //Sequence seq = DOTween.Sequence ();
                                //Vector3 TopPos = new Vector3 (ToPos.x, ToPos.y + 1.2f, ToPos.z);

                                //seq.Append (MoveableDisk.transform.DOMove (TopPos, 0.4f));
                                //seq.Append (MoveableDisk.transform.DOMove (ToPos, 0.4f));

                                ////MoveableDisk.transform.DOMove (ToPos, 1);

                                //tower.AddDisk (MoveableDisk); // Play animation 
                                //Debug.Log ("Moving disk --> " + MoveableDisk.name + "to Tower --> " + tower.name);
                            }
                            else
                                Debug.Log ("Invalid MOVE");
                        }
                        else
                        {
                            MoveDisk (MoveableDisk, tower);
                            SelectedTower.RemoveDisk (MoveableDisk);
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
                HasStarted = SelectedTower != null;
            }
        }
    }

    void MoveDisk (Disk moveabledisk, Tower ToTower)
    {
        // Setting disk position
        int diskcount = ToTower.AllDisks.Count;
        float ypos = diskcount * DiskThickness * 1.1f; // * 2f;
        Vector3 pos = ToTower.TowerOriginTransform.position;
        Vector3 ToPos = new Vector3 (pos.x, ypos, pos.z);

        // Disk move animation
        Sequence seq = DOTween.Sequence ();
        Vector3 TopPos = new Vector3 (ToPos.x, ToPos.y + 1.2f, ToPos.z);
        seq.Append (moveabledisk.transform.DOMove (TopPos, 0.4f));
        seq.Append (moveabledisk.transform.DOMove (ToPos, 0.4f));
        //MoveableDisk.transform.DOMove (ToPos, 1);
        ToTower.AddDisk (moveabledisk); // Play animation 

        // Debug Msg
        Debug.Log ("Moving disk --> " + moveabledisk.name + "to Tower --> " + ToTower.name);
    }

    Tower OnClickSelect (bool WithDisk)
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast (ray, out hit, 100f))
        {
            Tower tower = hit.collider.GetComponent<Tower> ();

            if (tower != null)
            {
                if (WithDisk)
                {
                    if (tower.AllDisks.Count > 0)
                    {
                        return tower;
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
}