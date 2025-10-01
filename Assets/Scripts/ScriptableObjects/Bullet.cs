using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Scriptable Objects/Bullet")]
public class Bullet : Weapon
{
    public float Speed = 10f;
    
    public bool IsPiercing = false;
    public bool IsHauning = false;
    public float RotationSpeed = 10f;
    public GameObject HitEffect;
}
