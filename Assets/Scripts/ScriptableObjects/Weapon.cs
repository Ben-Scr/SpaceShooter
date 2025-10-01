using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public abstract class Weapon : ScriptableObject
{
    public GameObject GameObject;
    public int Damage = 1;
    public bool Auto; // If true, the weapon fires automatically without any user input required
    public float CallInterval;
    public KeyCode CallKey = KeyCode.Space;
}
