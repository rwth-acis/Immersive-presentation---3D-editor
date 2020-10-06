using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PresListButtonScript : MonoBehaviour
{
    public TextMeshPro buttonText;

    private static PresentationElement _pres;
    /// <summary>
    /// The presentation this button represents
    /// </summary>
    public PresentationElement pres
    {
        get
        {
            return _pres;
        }

        set
        {
            _pres = value;
            buttonText.text = value.name;
        }
    }

    /// <summary>
    /// The text element where the name of presentation should appear when the presentation is selected
    /// </summary>
    public TextMeshProUGUI showNameOfPres;

    public void Click()
    {
        if (pres == null) {
            showNameOfPres.text = "Please select another presentation";
            return;
        }
        showNameOfPres.text = pres.name;
        StaticInformation.selectedPresElem = pres;
    }
}
