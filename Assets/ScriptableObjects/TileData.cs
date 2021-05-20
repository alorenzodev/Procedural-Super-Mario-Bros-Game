using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu]
public class TileData: ScriptableObject
{
    public AnimatedTile[] tiles;

    private Dictionary<int, string> contenido = new Dictionary<int, string>();

    public float foo, bar;


}
