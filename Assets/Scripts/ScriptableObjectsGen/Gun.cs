using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.ParthJain.FPSShooter{
[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class Gun : ScriptableObject
{
    public string name;
    public float firerate;
    public float aimSpeed;
    public GameObject prefab;
}
}
