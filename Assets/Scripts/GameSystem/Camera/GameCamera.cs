using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var player = PlayerController.getTransform();
        var pos = transform.position;
        //Controlamos si Mario existe, para control de errores.
        if (player != null)
        {
            pos.x = player.position.x;
            pos.y = player.position.y;
        }
        transform.position = pos;
    }
}
