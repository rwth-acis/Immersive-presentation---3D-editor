using ImmersivePresentation;
using System;
using UnityEditor;

public static class StaticInformation
{
    /// <summary>
    /// The presentation that is selected at the moment
    /// </summary>
    public static PresentationElement selectedPresElem { get; set; }

    /// <summary>
    /// The name of the selected presentation
    /// </summary>
    public static string selectedPresName
    {
        get
        {
            if (selectedPresElem == null) return "";
            return selectedPresElem.name;
        }
    }

    /// <summary>
    /// The id of the selected presentation
    /// </summary>
    public static double selectedPresId
    {
        get
        {
            if (selectedPresElem == null) return -1;
            return Convert.ToDouble(selectedPresElem.idpresentation);
        }
    }

    /// <summary>
    /// The id of the owner of the selected presentation
    /// </summary>
    public static double userId
    {
        get
        {
            if (selectedPresElem == null) return -1;
            return Convert.ToDouble(selectedPresElem.iduser);
        }
    }

    private static Presentation _openPresentation;
    /// <summary>
    /// The presentation that is opened at the moment
    /// </summary>
    public static Presentation openPresentation
    {
        get
        {
            return _openPresentation;
        }
        set
        {
            _openPresentation = value;
            openStageIndex = 0;
        }
    }

    /// <summary>
    /// The index of the stage that is opened at the moment
    /// </summary>
    public static int openStageIndex { get; set; }

    public static Stage getOpenedStage()
    {
        if(openPresentation.stages.Count > openStageIndex && openStageIndex >= 0)
        {
            return openPresentation.stages[openStageIndex];
        }
        else
        {
            return null;
        }
    }
}
