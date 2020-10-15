using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using ImmersivePresentation;
using System;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.ModelImporters;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControlTypes;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.SceneManagement;

public class EditorSceneHandler : MonoBehaviour
{
    public const string tempDownloadSuffix = "ImPres3D\\downloads\\";
    public const string tempSaveSuffixAndFilename = "ImPres3D\\save\\presentation.pres";
    public const string tempSuffix = "ImPres3D\\presentation\\";
    public const string presentationJsonFilename = "presentation.json";
    public const string tempSub2D = "2DMedia\\";
    public const string tempSub3D = "3DMedia\\";
    public const string tempSubSubScene = "Scene\\";
    public const string tempSubSubHandout = "Handout\\";

    public const string tempSaveSuffix = "ImPres3D\\save\\";

    public string presentationSavingPath
    {
        get
        {
            return StaticInformation.tempDirBase + tempSaveSuffixAndFilename;
        }
    }

    /// <summary>
    /// The Gameobject that serves as the anchor for the scene
    /// </summary>
    public GameObject anchor;

    /// <summary>
    /// The loading indicator that will be removed 
    /// </summary>
    public GameObject loadingVisualizer;

    public GameObject appBarPrefab;

    //private DataSerializer dataSerializer = new DataSerializer();
    private JsonSerializerSettings jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

    List<GameObject> actualSceneGameObjList;

    // Start is called before the first frame update
    void Start()
    {
        StaticInformation.removeDisabledObject = true;
        print(StaticInformation.selectedPresName);

        createWorkingDir();

        string filename = Path.GetFileNameWithoutExtension(StaticInformation.selectedPresElem.filepath);
        if (filename == "") filename = "downloadPres";
        filename = filename + ".pres";
        string downloadFilePath = StaticInformation.tempDownloadDir + filename;


        //Initialise the presentation download
        BackendConnection.BC.DownloadPresentation(StaticInformation.selectedPresElem.idpresentation, downloadFilePath, DownloadSucceed, DownloadFailed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// This handler gets called when the download is done
    /// </summary>
    /// <param name="path">The file path where the zipped presentation is stored</param>
    private void DownloadSucceed(string path)
    {
        print("Download Succeed");

        loadPresentation(path);
        //import all Gameobjects from the presentation
        actualSceneGameObjList = new List<GameObject>();
        if(StaticInformation.openPresentation.stages == null || StaticInformation.openPresentation.stages.Count == 0)
        {
            //Show error that no stage is given
            print("No stage in the opened presentation.");
            return;
        } 
        create3DObjectsFromScene(StaticInformation.getOpenedStage().scene, actualSceneGameObjList);
        loadingVisualizer.SetActive(false);
    }

    /// <summary>
    /// This handler gets called when the download failed
    /// </summary>
    /// <param name="msg">The message that describes the error</param>
    private void DownloadFailed(string msg)
    {
        print(msg);
        //Go back to the welcome Scene
    }

    private void loadPresentation(string pPath)
    {
        //Load zip extracted in temp
        string filePath = pPath;
        StaticInformation.tempDirBase = Path.GetTempPath().ToString();
        createCleanDirectory(StaticInformation.tempPresDir);
        ZipFile.ExtractToDirectory(filePath, StaticInformation.tempPresDir);

        //Deserialize json
        //*StaticInformation.openPresentation = dataSerializer.DeserializerJson(typeof(Presentation), tempPresDir + presentationJsonFilename) as Presentation;
        StaticInformation.openPresentation = JsonConvert.DeserializeObject<Presentation>(File.ReadAllText(StaticInformation.tempPresDir + presentationJsonFilename), jsonSettings);
    }

    /// <summary>
    /// Creates an empty directory at the specified path
    /// </summary>
    /// <param name="pDirectoryPath">The location of the directory. All new directories along the way will be created.</param>
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

    private void createWorkingDir()
    {
        StaticInformation.tempDirBase = Path.GetTempPath().ToString();
        createCleanDirectory(StaticInformation.tempSaveDir);
        createCleanDirectory(StaticInformation.tempDownloadDir);
        createCleanDirectory(StaticInformation.tempPresDir);
        createCleanDirectory(StaticInformation.tempPresDir + tempSub2D);
        createCleanDirectory(StaticInformation.tempPresDir + tempSub3D);
        createCleanDirectory(StaticInformation.tempPresDir + tempSub3D + tempSubSubScene);
        createCleanDirectory(StaticInformation.tempPresDir + tempSub3D + tempSubSubHandout);
    }

    /// <summary>
    /// Loads all the 3D elements that are in the scence into the unity scene
    /// </summary>
    /// <param name="pScene">The scene of the presentation</param>
    /// <param name="pSceneGameObjList">A List where all the created Gameobjects are stored</param>
    private async void create3DObjectsFromScene(ImmersivePresentation.Scene pScene, List<GameObject> pSceneGameObjList)
    {
        for (int i = 0; i < pScene.elements.Count; i++)
        {
            Element3D curElement = pScene.elements[i];
            GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportFromFileAsync(StaticInformation.tempPresDir + curElement.relativePath);
            obj.transform.parent = anchor.transform;
            obj.transform.localPosition = new Vector3( (float)curElement.xPosition, (float)curElement.yPosition, (float)curElement.zPosition);
            obj.transform.localScale = new Vector3((float)curElement.xScale, (float)curElement.yScale, (float)curElement.zScale);
            obj.transform.Rotate((float)curElement.xRotation, (float)curElement.yRotation, (float)curElement.zRotation, Space.Self);

            //Add transformation sync
            SyncEditedTransformation syncT = obj.AddComponent<SyncEditedTransformation>() as SyncEditedTransformation;
            syncT.relatedElement = curElement;

            //Add bounds control
            BoundsControl boundsControl;
            boundsControl = obj.AddComponent<BoundsControl>();
            boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateManually;

            GameObject appBar =  Instantiate(appBarPrefab);
            AppBar appBarScript = appBar.GetComponent<AppBar>();
            appBarScript.Target = obj.GetComponent<BoundsControl>();
            pSceneGameObjList.Add(appBar);

            ObjectManipulator objMan;
            objMan = obj.AddComponent<ObjectManipulator>();

            pSceneGameObjList.Add(obj);
            print("imported obj with id: " + i);
        }
    }

    public void nextStage()
    {
        StaticInformation.removeDisabledObject = false;
        if(StaticInformation.openStageIndex + 1 < StaticInformation.openPresentation.stages.Count && StaticInformation.openStageIndex + 1 >= 0)
        {
            StaticInformation.openStageIndex = StaticInformation.openStageIndex + 1;
            //delete obj from old scene
            foreach(GameObject obj in actualSceneGameObjList)
            {
                //actualSceneGameObjList.Remove(obj);
                Destroy(obj);
            }
            //load obj from the new scene
            actualSceneGameObjList = new List<GameObject>();
            create3DObjectsFromScene(StaticInformation.getOpenedStage().scene, actualSceneGameObjList);
        }
        StaticInformation.removeDisabledObject = true;
    }

    public void previousStage()
    {
        StaticInformation.removeDisabledObject = false;
        if (StaticInformation.openStageIndex - 1 < StaticInformation.openPresentation.stages.Count && StaticInformation.openStageIndex -1 >= 0)
        {
            StaticInformation.openStageIndex = StaticInformation.openStageIndex - 1;
            //delete obj from old scene
            foreach (GameObject obj in actualSceneGameObjList)
            {
                //actualSceneGameObjList.Remove(obj);
                Destroy(obj);
            }
            //load obj from the new scene
            actualSceneGameObjList = new List<GameObject>();
            create3DObjectsFromScene(StaticInformation.getOpenedStage().scene, actualSceneGameObjList);
        }
        StaticInformation.removeDisabledObject = true;
    }

    /// <summary>
    /// Creats a .pres file from the actual presentation. Then this .pres file is uploaded to the backend coordinator
    /// </summary>
    public void savePresentation()
    {
        File.WriteAllText(StaticInformation.tempPresDir + presentationJsonFilename, JsonConvert.SerializeObject(StaticInformation.openPresentation, jsonSettings));

        if (File.Exists(presentationSavingPath))
        {
            File.Delete(presentationSavingPath);
        }
        ZipFile.CreateFromDirectory(StaticInformation.tempPresDir, presentationSavingPath);

        BackendConnection.BC.UploadPresentation(StaticInformation.selectedPresId.ToString(), presentationSavingPath, UploadSucceed, UploadFailed);
    }

    public void UploadSucceed()
    {
        print("Upload Succeed");
    }

    public void UploadFailed(string msg)
    {
        print("Upload Failed");
    }

    /// <summary>
    /// Closes the editing of the 
    /// </summary>
    public void closePresentation()
    {
        StaticInformation.removeDisabledObject = false;
        //delete obj from old scene
        foreach (GameObject obj in actualSceneGameObjList)
        {
            //actualSceneGameObjList.Remove(obj);
            Destroy(obj);
        }
        actualSceneGameObjList = new List<GameObject>();

        StaticInformation.openPresentation = null;
        StaticInformation.selectedPresElem = null;
        SceneManager.LoadScene("Scenes/WelcomeScene", LoadSceneMode.Additive);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("WelcomeScene"));
        SceneManager.UnloadSceneAsync("EditorScene");
    }
}
