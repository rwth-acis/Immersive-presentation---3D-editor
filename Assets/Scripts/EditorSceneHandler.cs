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
    public string presentationSavingPath
    {
        get
        {
            return StaticInformation.tempDirBase + StaticInformation.tempSaveSuffixAndFilename;
        }
    }

    public GameObject canvas;
    public Renderer canvasRenderer;
    public Material backupMaterial;

    /// <summary>
    /// The Gameobject that serves as the anchor for the scene
    /// </summary>
    public GameObject anchor;

    /// <summary>
    /// The loading indicator that will be removed 
    /// </summary>
    public GameObject loadingVisualizer;

    public GameObject appBarPrefab;

    [SerializeField]
    [Tooltip("Type of dialog that should be displayed when the user clicked he remove Button on a 3D Element.")]
    public GameObject removeDialog;

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
        loadCanvas(StaticInformation.openStageIndex);
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
        StaticInformation.openPresentation = JsonConvert.DeserializeObject<Presentation>(File.ReadAllText(StaticInformation.tempPresDir + StaticInformation.presentationJsonFilename), jsonSettings);
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
        createCleanDirectory(StaticInformation.tempPresDir + StaticInformation.tempSub2D);
        createCleanDirectory(StaticInformation.tempPresDir + StaticInformation.tempSub3D);
        createCleanDirectory(StaticInformation.tempPresDir + StaticInformation.tempSub3D + StaticInformation.tempSubSubScene);
        createCleanDirectory(StaticInformation.tempPresDir + StaticInformation.tempSub3D + StaticInformation.tempSubSubHandout);
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
#if UNITY_ANDROID
            // LLUPD GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportFromFileAsync(StaticInformation.tempPresDir + curElement.relativePath.Replace('\\', '/'));
            GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportAsync("file://" + StaticInformation.tempPresDir + curElement.relativePath.Replace('\\', '/'));
            //string debug_help = @"C:\Users\lukas\AppData\LocalLow\LukasLiss\ImPres\ImPres3D\presentation\3DMedia\Scene\Mond.obj";
            //print((StaticInformation.tempPresDir + curElement.relativePath.Replace('\\', '/')).Replace('/', '\\'));
            //GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportAsync((StaticInformation.tempPresDir + curElement.relativePath.Replace('\\', '/')).Replace('/', '\\'));
            //GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportAsync("https://people.sc.fsu.edu/~jburkardt/data/obj/airboat.obj"); //works
            //if (System.IO.File.Exists("C:\\Users\\lukas\\OneDrive\\Dokumente\\Arbeit_i5_HiWi\\Mond.obj"))
            //{
            //    print("File Exist");
            //}
            //else
            //{
            //    print("File NOT Exist");
            //}
            
            //GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportAsync("C:\\Users\\lukas\\OneDrive\\Dokumente\\Arbeit_i5_HiWi\\Mond.obj");
        

#else
            GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportFromFileAsync(StaticInformation.tempPresDir + curElement.relativePath);
#endif
            obj.transform.parent = anchor.transform;
            obj.transform.localPosition = new Vector3( (float)curElement.xPosition, (float)curElement.yPosition, (float)curElement.zPosition);
            obj.transform.localScale = new Vector3((float)curElement.xScale, (float)curElement.yScale, (float)curElement.zScale);
            obj.transform.Rotate((float)curElement.xRotation, (float)curElement.yRotation, (float)curElement.zRotation, Space.Self);

            //Add transformation sync
            SyncEditedTransformation syncT = obj.AddComponent<SyncEditedTransformation>() as SyncEditedTransformation;
            syncT.relatedElement = curElement;
            syncT.removeDialog = removeDialog;

            //Add bounds control
            BoundsControl boundsControl;
            boundsControl = obj.AddComponent<BoundsControl>();
            boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateManually;

            GameObject appBar =  Instantiate(appBarPrefab);
            AppBar appBarScript = appBar.GetComponent<AppBar>();
            appBarScript.Target = obj.GetComponent<BoundsControl>();
            pSceneGameObjList.Add(appBar);

            syncT.relatedAppBar = appBar;

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
            loadCanvas(StaticInformation.openStageIndex);
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
            loadCanvas(StaticInformation.openStageIndex);
            create3DObjectsFromScene(StaticInformation.getOpenedStage().scene, actualSceneGameObjList);
        }
        StaticInformation.removeDisabledObject = true;
    }

    /// <summary>
    /// Creats a .pres file from the actual presentation. Then this .pres file is uploaded to the backend coordinator
    /// </summary>
    public void savePresentation()
    {
        loadingVisualizer.SetActive(true);
        File.WriteAllText(StaticInformation.tempPresDir + StaticInformation.presentationJsonFilename, JsonConvert.SerializeObject(StaticInformation.openPresentation, jsonSettings));

        if (File.Exists(presentationSavingPath))
        {
            File.Delete(presentationSavingPath);
        }
        ZipFile.CreateFromDirectory(StaticInformation.tempPresDir, presentationSavingPath);

        BackendConnection.BC.UploadPresentation(StaticInformation.selectedPresId.ToString(), presentationSavingPath, UploadSucceed, UploadFailed);
    }

    public void UploadSucceed()
    {
        loadingVisualizer.SetActive(false);
        print("Upload Succeed");
    }

    public void UploadFailed(string msg)
    {
        loadingVisualizer.SetActive(false);
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

    private void loadCanvas(int pStageIndex)
    {
        //the images as in the subfolder CanvasImg/[pStageIndex].png
        string potentialCanvasImgPath = StaticInformation.tempPresDir + StaticInformation.tempSubCanvasImg + pStageIndex + ".png";

        if (File.Exists(potentialCanvasImgPath))
        {
            var bytes = System.IO.File.ReadAllBytes(potentialCanvasImgPath);
            var tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            canvasRenderer.material.mainTexture = tex;
        }
        else
        {
            canvasRenderer.material = backupMaterial;
        }
    }
}
