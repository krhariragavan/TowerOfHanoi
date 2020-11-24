using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Derived from an interface for undo purpose. Implementing REDO also becomes very handy with this interface
// This class is based on Command Pattern
public class Move : IMove 
{
    Tower _FromTower;
    Disk _Disk;
    Tower _ToTower;

    // Constructor for Move Class
    public Move (Tower FromTower, Tower ToTower)
    {
        _FromTower = FromTower;
        _ToTower = ToTower;

        _Disk = _FromTower.GetMoveableDiskBySize ();
    }

    // Disk move from one tower to another
    void MakeMove (Tower fromTower, Tower toTower, bool IsUndo)
    {
        Disk moveabledisk = fromTower.GetMoveableDiskBySize (); // Get the actual moveable disk

        int diskcount = toTower.AllDisks.Count; // Get disk count in the 2nd tower
        float ypos = (diskcount * Game.DiskThickness * 1.1f) + 3.3f; // 3.3 in environment and in game only its 0.1f

        // Get positions for animation purpose
        Vector3 pos = toTower.TowerOriginTransform.position;
        Vector3 ToPos = new Vector3 (pos.x, ypos, pos.z);

        fromTower.RemoveDisk (moveabledisk);

        // Disk move animation
        Sequence seq = DOTween.Sequence ();
        Vector3 TopPos = new Vector3 (ToPos.x, ToPos.y + 1.2f, ToPos.z);
        seq.Append (moveabledisk.transform.DOMove (TopPos, 0.4f));
        seq.Append (moveabledisk.transform.DOMove (ToPos, 0.4f));
        //MoveableDisk.transform.DOMove (ToPos, 1);
        toTower.AddDisk (moveabledisk); // Play animation

        Game.Instance.MoveCount++;

        // Changing move count in undo
        //if (IsUndo)
        //    Game.Instance.MoveCount--;
        //else
        //    Game.Instance.MoveCount++;

    }

    public void Execute ()
    {
        MakeMove (_FromTower, _ToTower, false);
    }

    public void Undo ()
    {
        MakeMove (_ToTower, _FromTower, true);
    }
}