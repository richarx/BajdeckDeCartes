using UnityEngine;
using System.Collections.Generic;

public class Slot : MonoBehaviour
{

    private int _slotIndex = -1;

    
    private int SlotIndex
    {
        get { return (_slotIndex); }
        set { _slotIndex = value; }
    }
    
}
