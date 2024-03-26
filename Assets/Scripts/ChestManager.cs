using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    public static ChestManager instance;

    void Awake()
    {
        instance = this;
    }

    public void Open()
    {

    }
}
