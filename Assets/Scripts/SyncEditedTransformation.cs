using ImmersivePresentation;
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
        transform.hasChanged = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.hasChanged)
        {
            print("Change detected");
            StaticInformation.saveTranslate(relatedElement, transform);
        }
    }

    private void OnDisable()
    {
        if (StaticInformation.removeDisabledObject)
        {
            print("Remove Object from presentation");
            StaticInformation.remove3DElementfromPresentation(relatedElement);
        }
    }
}
