﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : IMove
{
    Tower _FromTower;
    Disk _Disk;
    Tower _ToTower;

    public Move (Tower FromTower, Tower ToTower)
    {
        _FromTower = FromTower;
        _ToTower = ToTower;

        _Disk = _FromTower.GetMoveableDiskBySize ();
    }

    void MakeMove (Tower fromTower, Tower toTower, bool IsUndo)
    {
        Disk moveabledisk = fromTower.GetMoveableDiskBySize ();

        int diskcount = toTower.AllDisks.Count;
        float ypos = (diskcount * Game.DiskThickness * 1.1f) + 0.1f; // * 2f;
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

        if (IsUndo)
            Game.Instance.MoveCount--;
        else
            Game.Instance.MoveCount++;

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