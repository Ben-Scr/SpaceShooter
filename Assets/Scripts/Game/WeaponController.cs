using SpaceShooter;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    [SerializeField] float spreadAngle = 60f;
    [SerializeField] private List<Weapon> initWeapons;
    [SerializeField] private Slider[] weaponSliders;
    [SerializeField] private Color weaponAvailableColor, weaponUnavailableColor;

    private List<WeaponInstance> persistentWeapons = new List<WeaponInstance>();

    private void Awake()
    {
        int i = 0;
        foreach (var weapon in initWeapons)
        {
            Slider weaponSlider = weaponSliders[i];
            weaponSlider.maxValue = weapon.CallInterval;
            weaponSlider.value = weaponSlider.maxValue;
            persistentWeapons.Add(new WeaponInstance(weapon));
        }
    }

    private void Update()
    {
        UpdateWeapons();
    }

    private void UpdateWeapons()
    {
        int j = 0;
        foreach (WeaponInstance persistentWeapon in persistentWeapons)
        {
            UpdateWeaponSlider(weaponSliders[j++], persistentWeapon.LastCallTimeCounter);
            Weapon weapon = persistentWeapon.Weapon;
            Transform playerTr = PlayerController.Instance.transform;

            if (persistentWeapon.CanCall() && (Input.GetKeyDown(weapon.CallKey) || (weapon.Auto)))
            {
                persistentWeapon.LastCallTimeCounter = 0;

                if (weapon is Bullet bullet)
                {
                    if (bullet.EmitCount == 1)
                    {
                        BulletHandler.SpawnBullet(bullet, (Vector2)playerTr.position, playerTr.rotation);
                    }
                    else
                    {
                        if (bullet.SpreadType == SpreadType.Frustum)
                        {
                            float startAngle = -spreadAngle / 2f;
                            float angleStep = spreadAngle / (bullet.EmitCount - 1);

                            for (int i = 0; i < bullet.EmitCount; i++)
                            {
                                float currentAngle = startAngle + angleStep * i;
                                Quaternion rot = playerTr.rotation * Quaternion.Euler(0, 0, currentAngle);
                                BulletHandler.SpawnBullet(bullet, (Vector2)playerTr.position, rot);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < bullet.EmitCount; i++)
                            {
                                float angle = i * (360f / bullet.EmitCount);
                                Quaternion rot = Quaternion.Euler(0, 0, angle);
                                BulletHandler.SpawnBullet(bullet, (Vector2)playerTr.position, rot);
                            }
                        }
                    }
                }
            }
        }
    }


    private void UpdateWeaponSlider(Slider slider, float value)
    {
        slider.value = value;
        bool isMax = slider.value >= slider.maxValue;
        slider.fillRect.GetComponent<Image>().color = isMax ? weaponAvailableColor : weaponUnavailableColor;
    }
}
