using UnityEngine;

namespace SpaceShooter
{
    public static class UnityUtility
    {
        public static void LookAt2D(Transform from, Vector3 lookAt, float rotationSpeed = 14, float rotationOffset = 0, bool negative = false)
        {
            Vector3 dir = lookAt - from.position;

            if (negative)
                dir = from.position - lookAt;


            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            from.transform.rotation = Quaternion.Lerp(from.rotation, Quaternion.AngleAxis(angle + rotationOffset, Vector3.forward), Time.deltaTime * rotationSpeed);
        }

        public static Quaternion LookAt2DQuaternion(Transform from, Vector3 lookAt, float rotationSpeed = 14, float rotationOffset = 0, bool negative = false)
        {
            Vector3 dir = lookAt - from.position;

            if (negative)
                dir = from.position - lookAt;


            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            return Quaternion.Lerp(from.rotation, Quaternion.AngleAxis(angle + rotationOffset, Vector3.forward), Time.deltaTime * rotationSpeed);
        }

        public static Vector3 GetMouseWorldPosition(Camera cam = null)
        {
            if (cam == null)
                cam = Camera.main;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -cam.transform.position.z; // Distance from the camera to the world origin
            return cam.ScreenToWorldPoint(mousePos);
        }
    }
}