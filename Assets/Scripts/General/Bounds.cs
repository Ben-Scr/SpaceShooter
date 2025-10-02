using System;
using UnityEngine;

namespace SpaceShooter
{
    [Serializable]
    public struct Bounds
    {
        public float MinX, MaxX, MinY, MaxY;

        public Bounds(float minX, float minY, float maxX, float maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        public bool Contains(Vector2 position)
        {
            return position.x >= MinX && position.x <= MaxX && position.y >= MinY && position.y <= MaxY;
        }
    }
}
