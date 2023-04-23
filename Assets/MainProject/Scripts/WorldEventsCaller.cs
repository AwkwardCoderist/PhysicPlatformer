using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventsCaller : MonoBehaviour
{
    public void GoToScene(string name)
    {
        SceneMenage.instance.GoToScene(name);
    }
}
