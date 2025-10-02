using UnityEngine;

namespace SpaceShooter
{
    public class GameAssetsManager : MonoBehaviour
    {
        public static GameAssetsManager Instance { get; private set; }
        public Bounds MapBounds;

        private void Awake()
        {
            Instance = this;
        }
    }
}