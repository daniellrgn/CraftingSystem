using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICraftStation<T> {
    Inventory Input { get; set; }
    List<T> recipeList { get; set; }
    void Craft();
}