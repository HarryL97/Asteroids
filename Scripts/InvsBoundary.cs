using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvsBoundary : MonoBehaviour
{
    [SerializeField]
    private int oppositeBoundaryNumber;

    public int OppositeBoundaryNumber   {
        get {
            return oppositeBoundaryNumber;
        }
    }

}

