using UnityEngine;

namespace SpaceShooter
{
    [CreateAssetMenu(fileName = "Bullet", menuName = "Scriptable Objects/Bullet")]
    public class Bullet : Weapon
    {
        public float Speed = 10f;

        public SpreadType SpreadType;
        public int EmitCount;
        public bool IsPiercing = false;
        public bool IsHauning = false;
        public bool Bounceable = false;
        public int MaxBounces = 3;
        public float HitDistanceThreshold;
        public float RotationSpeed = 10f;
        public GameObject HitEffect;
    }

    public enum SpreadType
    {
        Circular,
        Frustum
    }
}