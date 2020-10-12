using ImmersivePresentation;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class StaticInformation
{
    public static string tempDirBase { get; set; } //Path to the start of the temporary folder of the actual windows user 
    public const string tempDownloadSuffix = "ImPres3D\\downloads\\";
    public static string tempDownloadDir
    {
        get
        {
            return tempDirBase + tempDownloadSuffix;
        }
    } //Path where the presentations are downloaded to.

    public const string tempSuffix = "ImPres3D\\presentation\\";
    public static string tempPresDir
    {
        get
        {
            return tempDirBase + tempSuffix;
        }
    } //Path where the presentation content is stored and the json of the presentation.
    public const string presentationJsonFilename = "presentation.json";
    public const string tempSub2D = "2DMedia\\";
    public const string tempSub3D = "3DMedia\\";
    public const string tempSubSubScene = "Scene\\";
    public const string tempSubSubHandout = "Handout\\";

    public const string tempSaveSuffix = "ImPres3D\\save\\";
    public static string tempSaveDir
    {
        get
        {
            return tempDirBase + tempSaveSuffix;
        }
    } //Path where the presentation is saved before uploading.

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

    public static bool removeDisabledObject;

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

    //Edit openPresentation

    public static void remove3DElementfromPresentation(Element3D elem)
    {
        //clean the presentaion temp folder
        removeRessources(elem);
        //remove from scene list
        getOpenedStage().scene.elements.Remove(elem);
    }

    /// <summary>
    /// Removes the resources of the element from the temp folder.
    /// </summary>
    /// <param name="elem">Element which resources should be removed.</param>
    private static void removeRessources(Element elem)
    {
        if (elem.GetType() == typeof(Image2D))
        {
            Image2D img = (Image2D)elem;
            if (File.Exists(tempPresDir + img.relativeImageSource))
            {
                File.Delete(tempPresDir + img.relativeImageSource);
            }
            return;
        }

        if (elem.GetType() == typeof(Element3D))
        {
            Element3D elem3d = (Element3D)elem;
            if (elem3d.relativePath != "" && File.Exists(tempPresDir + elem3d.relativePath))
            {
                File.Delete(tempPresDir + elem3d.relativePath);
            }
            if (elem3d.relativMaterialPath != "" && File.Exists(tempPresDir + elem3d.relativMaterialPath))
            {
                File.Delete(tempPresDir + elem3d.relativMaterialPath);
            }
        }
    }

    public static void saveTranslate(Element3D elem, Transform pTransform)
    {
        elem.xPosition = (double)pTransform.localPosition.x;
        elem.yPosition = (double)pTransform.localPosition.y;
        elem.zPosition = (double)pTransform.localPosition.z;
        elem.xScale = (double)pTransform.localScale.x;
        elem.yScale = (double)pTransform.localScale.y;
        elem.zScale = (double)pTransform.localScale.z;
        elem.xRotation = (double)pTransform.localRotation.eulerAngles.x;
        elem.yRotation = (double)pTransform.localRotation.eulerAngles.y;
        elem.zRotation = (double)pTransform.localRotation.eulerAngles.z;
    }

}
