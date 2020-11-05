using ImmersivePresentation;
using Microsoft.MixedReality.Toolkit.Experimental.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncEditedTransformation : MonoBehaviour
{
    /// <summary>
    /// The element in te presentation object that this gameobject represents. The transform of the related element will be updated once the transform of the gameobject changes.
    /// </summary>
    public Element3D relatedElement;
    public GameObject relatedAppBar;
    public GameObject removeDialog;

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
            //print("Change detected");
            StaticInformation.saveTranslate(relatedElement, transform);
        }
    }

    private void OnDisable()
    {
        if (StaticInformation.removeDisabledObject)
        {
            //print("Remove Object from presentation");

            Dialog myDialog = Dialog.Open(removeDialog, DialogButtonType.Yes | DialogButtonType.No, "Are you sure to delete the object?", "This action is permanent and can not be reverted.", true);
            if (myDialog != null)
            {
                myDialog.OnClosed += OnClosedRemoveDialogEvent;
            }
        }
    }

    private void OnClosedRemoveDialogEvent(DialogResult obj)
    {
        if (obj.Result == DialogButtonType.Yes)
        {
            StaticInformation.remove3DElementfromPresentation(relatedElement);
        }else if(obj.Result == DialogButtonType.No)
        {
            gameObject.SetActive(true);
            relatedAppBar.SetActive(true);
        }
    }
}
