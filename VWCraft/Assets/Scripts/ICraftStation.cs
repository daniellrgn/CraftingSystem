using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICraftStation {
    Inventory Input { get; set; }
    void Craft();
}