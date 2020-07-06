﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBG: MonoBehaviour
{
    public Vector2 speed;
    Material material;
    Vector2 movement;
    
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        movement += speed * Time.deltaTime;
        material.mainTextureOffset = movement;
    }
}
