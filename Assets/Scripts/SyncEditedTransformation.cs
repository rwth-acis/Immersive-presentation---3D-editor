﻿using ImmersivePresentation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncEditedTransformation : MonoBehaviour
{
    /// <summary>
    /// The element in te presentation object that this gameobject represents. The transform of the related element will be updated once the transform of the gameobject changes.
    /// </summary>
    public Element3D relatedElement;


    // Start is called before the first frame update
    void Start()
    {
        print("Start called");
        transform.hasChanged = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.hasChanged)
        {
            print("Change detected");
            transform.hasChanged = false;
        }
    }
}