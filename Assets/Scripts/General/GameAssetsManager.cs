using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class GameAssetsManager : MonoBehaviour
{
    [SerializeField] private List<Weapon> weaponsContainer;
    public static List<Weapon> WeaponsContainer => Instance.weaponsContainer;
    public static GameAssetsManager Instance { get; private set; }
    public Assets.Scripts.General.Bounds MapBounds;

    private void Awake()
    {
        Instance = this;
    }
}
