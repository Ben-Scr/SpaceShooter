using UnityEngine;

namespace SpaceShooter
{
    [CreateAssetMenu(fileName = "Asteriod", menuName = "Scriptable Objects/Asteriod")]
    public class Asteriod : ScriptableObject
    {
        public GameObject GameObject;
        public int Health = 1;
        public float MovementSpeed = 5f;
        public float RotationSpeed = 1;
        public float ScaleMultiplier = 1f;
        public int ScoreValue = 10;
    }
}
