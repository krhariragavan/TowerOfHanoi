using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove
{
    void Execute ();
    void Undo ();
}
