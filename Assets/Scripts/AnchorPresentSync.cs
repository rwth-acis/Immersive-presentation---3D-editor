using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPresentSync : MonoBehaviour
{
    /// <summary>
    /// The PresentHandling Scripts function anchorRepositioned is called when the transform of the object changed
    /// </summary>
    public PresentHandling presentHandling;
    // Start is called before the first frame update
    void Start()
    {
        transform.hasChanged = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void manipulationEnded()
    {
        if (transform.hasChanged)
        {
            presentHandling.anchorRepositioned(transform);
            transform.hasChanged = false;
        }
    }
}
