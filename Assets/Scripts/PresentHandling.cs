using i5.Toolkit.Core.ModelImporters;
using i5.Toolkit.Core.ServiceCore;
using ImmersivePresentation;
using Microsoft.MixedReality.Toolkit;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

public class PresentHandling : MonoBehaviour
{

    public  Presentation openPresentation;
    private  int _stageIndex;
    public  int stageIndex
    {
        get
        {
            return _stageIndex;
        }
        set
        {
            loadSceneFromStage(value);
            _stageIndex = value;
        }
    }

    private  List<GameObject> generadedGameObjects;

    public GameObject anchor;
    public GameObject menueOwner;
    public GameObject menueGuest;

    private JsonSerializerSettings jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

    public const string presentationJsonFilename = "presentation.json";

    private bool isOwner = false;

    /// <summary>
    /// Returns the stage that is the actual one at the moment
    /// </summary>
    /// <returns></returns>
    public  Stage getStage()
    {
        if (openPresentation == null) return null;
        if (stageIndex < 0 || stageIndex >= openPresentation.stages.Count) return null;
        return openPresentation.stages[stageIndex];
    }

    /// <summary>
    /// Loads the 3D Elements that are in the scene of the given stage that is defined by the stageIndex.
    /// </summary>
    /// <param name="pStageIndex">The index of the Stage from where the scene should be loaded.</param>
    public async void loadSceneFromStage(int pStageIndex)
    {
        //clean old scene
        if (generadedGameObjects != null && generadedGameObjects.Count != 0)
        {
            foreach (GameObject obj in generadedGameObjects)
            {
                //actualSceneGameObjList.Remove(obj);
                Destroy(obj);
            }
        }

        //load obj from the new scene
        generadedGameObjects = new List<GameObject>();

        //load new scene
        for (int i = 0; i < openPresentation.stages[pStageIndex].scene.elements.Count; i++)
        {
            Element3D curElement = openPresentation.stages[pStageIndex].scene.elements[i];
            GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportFromFileAsync(StaticInformation.tempPresDir + curElement.relativePath);
            obj.transform.parent = anchor.transform;
            obj.transform.localPosition = new Vector3((float)curElement.xPosition, (float)curElement.yPosition, (float)curElement.zPosition);
            obj.transform.localScale = new Vector3((float)curElement.xScale, (float)curElement.yScale, (float)curElement.zScale);
            obj.transform.Rotate((float)curElement.xRotation, (float)curElement.yRotation, (float)curElement.zRotation, Space.Self);

            generadedGameObjects.Add(obj);
        }
    }

    private void createWorkingDir()
    {
        StaticInformation.tempDirBase = Path.GetTempPath().ToString();
        createCleanDirectory(StaticInformation.tempSaveDir);
        createCleanDirectory(StaticInformation.tempDownloadDir);
        createCleanDirectory(StaticInformation.tempPresDir);
        createCleanDirectory(StaticInformation.tempPresDir + StaticInformation.tempSub2D);
        createCleanDirectory(StaticInformation.tempPresDir + StaticInformation.tempSub3D);
        createCleanDirectory(StaticInformation.tempPresDir + StaticInformation.tempSub3D + StaticInformation.tempSubSubScene);
        createCleanDirectory(StaticInformation.tempPresDir + StaticInformation.tempSub3D + StaticInformation.tempSubSubHandout);
    }
    public void createCleanDirectory(string pDirectoryPath)
    {
        try
        {
            // Determine whether the directory exists.
            if (Directory.Exists(pDirectoryPath))
            {
                //delete all the content of the directory
                DirectoryInfo dirInf = new DirectoryInfo(pDirectoryPath);

                foreach (FileInfo file in dirInf.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in dirInf.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            else
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(pDirectoryPath);
            }
        }
        catch
        {

        }
        finally { }
    }

    public void openMoreMenue()
    {
        print("NotImplemented");
    }

    public void nextStage()
    {
        print("NotImplemented");
    }

    public void previousStage()
    {
        print("NotImplemented");
    }

    public void leavePresentation()
    {
        print("NotImplemented");
    }

    public void leaveAndStopPresentation()
    {
        print("NotImplemented");
    }

    /// <summary>
    /// Uploads the new relativ anchor transformation to photon
    /// </summary>
    /// <param name="newTransform"></param>
    public void anchorRepositioned(Transform newTransform)
    {
        print("NotImplemented");
    }

    void Start()
    {
        //DEBUG:
        StaticInformation.shortCode = "1-14";

        //check for shortCode in StaticInformation Join 
        if(StaticInformation.shortCode == null)
        {
            print("Error: no shortCode is set");
            return;
        }

        //download the presentation
        createWorkingDir();

        string filename = "downloadPres.pres";
        string downloadFilePath = StaticInformation.tempDownloadDir + filename;
        if (File.Exists(downloadFilePath))
        {
            File.Delete(downloadFilePath);
        }


        //Initialise the presentation download
        if (BackendConnection.BC == null) print("1");
        BackendConnection.BC.DownloadPresentationShortCode(StaticInformation.shortCode, downloadFilePath, DownloadSucceed, DownloadFailed);

        //setup continues in secondPartOfSetup()
    }

    /// <summary>
    /// This method is called when the presentation is donloaded. This method takes care of the setup part with the photon room and azure
    /// </summary>
    public void secondPartOfSetup()
    {

        stageIndex = 0;
        //check ownership
        print(openPresentation.ownerId);
        double presUserId;
        bool isNumeric = Double.TryParse(openPresentation.ownerId, out presUserId);
        if(isNumeric && StaticInformation.userId == presUserId)
        {
            isOwner = true;
            menueOwner.SetActive(true);
            menueGuest.SetActive(false);
        }
        else
        {
            isOwner = false;
            menueOwner.SetActive(false);
            menueGuest.SetActive(true);
        }
        //connect to photon room

        //check for spatial anchor existens

        //Load spatial anchor

        //Else
        //Create and upload spatial anchor

        //check for actualStage

        //remove loading indicator
        //load actualStage

        //Else
        //Set actualStage to 0
        //load actualStage
    }

    void Update()
    {
        
    }

    public void DownloadSucceed(string path)
    {
        //Load zip extracted in temp
        string filePath = path;
        StaticInformation.tempDirBase = Path.GetTempPath().ToString();
        createCleanDirectory(StaticInformation.tempPresDir);
        ZipFile.ExtractToDirectory(filePath, StaticInformation.tempPresDir);

        //Deserialize json
        //*StaticInformation.openPresentation = dataSerializer.DeserializerJson(typeof(Presentation), tempPresDir + presentationJsonFilename) as Presentation;
        openPresentation = JsonConvert.DeserializeObject<Presentation>(File.ReadAllText(StaticInformation.tempPresDir + presentationJsonFilename), jsonSettings);

        secondPartOfSetup();
    }

    public void DownloadFailed(string msg)
    {
        print(msg);
    }

}
