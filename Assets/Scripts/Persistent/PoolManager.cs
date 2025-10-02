using UnityEngine;

namespace SpaceShooter
{
    public class PoolManager : MonoBehaviour
    {
        public static DynamicObjectPool AsteriodsPool;
        public static DynamicObjectPool BulletsPool;
        public static DynamicObjectPool UIPool;
        public static DynamicObjectPool EffectsPool;

        private void Awake()
        {
            AsteriodsPool = new DynamicObjectPool();
            BulletsPool = new DynamicObjectPool();
            UIPool = new DynamicObjectPool();
            EffectsPool = new DynamicObjectPool();
        }
    }
}
