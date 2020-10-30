using i5.Toolkit.Core.ModelImporters;
using i5.Toolkit.Core.ServiceCore;
using ImmersivePresentation;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControlTypes;
using Microsoft.MixedReality.Toolkit.UI;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PresentHandling : MonoBehaviour
{

    public  Presentation openPresentation;
    private int _stageIndex;
    public  int stageIndex
    {
        get
        {
            return _stageIndex;
        }
        set
        {
            _stageIndex = value;
        }
    }

    private  List<GameObject> generadedGameObjects;

    public GameObject anchor;
    public GameObject anchorAppBar;
    public GameObject loadingIndicator;
    public GameObject menueOwner;
    public GameObject menueMore;
    public GameObject menueGuest;
    public GameObject canvas;
    public GameObject appBarPrefab;
    public PhotonConnectionScript photonConnectionScript;

    private JsonSerializerSettings jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

    public const string presentationJsonFilename = "presentation.json";

    public bool isOwner = false;

    public Renderer canvasRenderer;
    public Material backupMaterial;

    private bool showHandoutInsteadOfScene = false;

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
    /// Depending on wheather the scene or the handout is selected, it will be loaded.
    /// </summary>
    /// <param name="pStageIndex">Index of the stage to load the scene or the handout from.</param>
    public async Task loadStage(int pStageIndex)
    {
        if (showHandoutInsteadOfScene)
        {
            await loadHandoutFromStage(pStageIndex);
        }
        else
        {
            await loadSceneFromStage(pStageIndex);
        }
    }

    /// <summary>
    /// Loads the 3D Elements that are in the scene of the given stage that is defined by the stageIndex.
    /// </summary>
    /// <param name="pStageIndex">The index of the Stage from where the scene should be loaded.</param>
    public async Task loadSceneFromStage(int pStageIndex)
    {
        stageIndex = pStageIndex;
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
#if UNITY_ANDROID
            GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportFromFileAsync(StaticInformation.tempPresDir + curElement.relativePath.Replace('\\', '/'));
            //GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportFromFileAsync(StaticInformation.tempPresDir + "/ImPres3D/presentation/3DMedia/Scene/sheep.obj");
#else
            GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportFromFileAsync(StaticInformation.tempPresDir + curElement.relativePath);
            //GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportFromFileAsync(StaticInformation.tempPresDir + "/ImPres3D/presentation/3DMedia/Scene/sheep.obj");
#endif
            obj.transform.parent = anchor.transform;
            obj.transform.localPosition = new Vector3((float)curElement.xPosition, (float)curElement.yPosition, (float)curElement.zPosition);
            obj.transform.localScale = new Vector3((float)curElement.xScale, (float)curElement.yScale, (float)curElement.zScale);
            obj.transform.Rotate((float)curElement.xRotation, (float)curElement.yRotation, (float)curElement.zRotation, Space.Self);

            generadedGameObjects.Add(obj);
        }

        //update the canvas
        loadCanvas(pStageIndex);
    }

    /// <summary>
    /// Loads the 3D Elements that are in the handout of the given stage that is defined by the stageIndex.
    /// </summary>
    /// <param name="pStageIndex">The index of the stage from where the handout should be loaded.</param>
    /// <returns></returns>
    public async Task loadHandoutFromStage(int pStageIndex)
    {
        stageIndex = pStageIndex;
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
        for (int i = 0; i < openPresentation.stages[pStageIndex].handout.elements.Count; i++)
        {
            Element3D curElement = openPresentation.stages[pStageIndex].handout.elements[i];
#if UNITY_ANDROID
            GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportFromFileAsync(StaticInformation.tempPresDir + curElement.relativePath.Replace('\\', '/'));
            //GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportFromFileAsync(StaticInformation.tempPresDir + "/ImPres3D/presentation/3DMedia/Scene/sheep.obj");
#else
            GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportFromFileAsync(StaticInformation.tempPresDir + curElement.relativePath);
            //GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportFromFileAsync(StaticInformation.tempPresDir + "/ImPres3D/presentation/3DMedia/Scene/sheep.obj");
#endif
            obj.transform.parent = anchor.transform;
            obj.transform.localPosition = new Vector3((float)curElement.xPosition, (float)curElement.yPosition, (float)curElement.zPosition);
            obj.transform.localScale = new Vector3((float)curElement.xScale, (float)curElement.yScale, (float)curElement.zScale);
            obj.transform.Rotate((float)curElement.xRotation, (float)curElement.yRotation, (float)curElement.zRotation, Space.Self);

            //Add bounds control
            BoundsControl boundsControl;
            boundsControl = obj.AddComponent<BoundsControl>();
            boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateManually;

            GameObject appBar = Instantiate(appBarPrefab);
            AppBar appBarScript = appBar.GetComponent<AppBar>();
            appBarScript.Target = obj.GetComponent<BoundsControl>();
            generadedGameObjects.Add(appBar);

            ObjectManipulator objMan;
            objMan = obj.AddComponent<ObjectManipulator>();

            generadedGameObjects.Add(obj);
        }

        //update the canvas
        loadCanvas(pStageIndex);
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

    private void createWorkingDir()
    {
        StaticInformation.tempDirBase = Application.persistentDataPath;
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

    public void openMoreMenu()
    {
        menueMore.SetActive(true);
    }

    public void nextStage()
    {
        if(stageIndex + 1 < openPresentation.stages.Count)
        {
            photonConnectionScript.sendStageIndex(stageIndex + 1);
        }
    }

    public void previousStage()
    {
        if(stageIndex - 1 >= 0)
        {
            photonConnectionScript.sendStageIndex(stageIndex - 1);
        }
    }

    public void leavePresentation()
    {
        photonConnectionScript.Disconnect();
        StaticInformation.openPresentation = null;
        StaticInformation.selectedPresElem = null;
        SceneManager.LoadScene("Scenes/WelcomeScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("PresentationScene");
    }

    public void leaveAndStopPresentation()
    {
        //stop the presentation for coordinator

        //leave the presentation
        photonConnectionScript.Disconnect();
        StaticInformation.openPresentation = null;
        StaticInformation.selectedPresElem = null;
        SceneManager.LoadScene("Scenes/WelcomeScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("PresentationScene");
    }

    public void closeMenuMore()
    {
        menueMore.SetActive(false);
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
        if (BackendConnection.BC == null) 
        {
            print("Error: No Backend connection established.");
            StaticInformation.openPresentation = null;
            StaticInformation.selectedPresElem = null;
            SceneManager.LoadScene("Scenes/WelcomeScene", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("PresentationScene");
        }
        BackendConnection.BC.DownloadPresentationShortCode(StaticInformation.shortCode, downloadFilePath, DownloadSucceed, DownloadFailed);

        //setup continues in secondPartOfSetup()
    }

    /// <summary>
    /// This method is called when the presentation is donloaded. This method takes care of the setup part with the photon room and azure
    /// </summary>
    public void secondPartOfSetup()
    {
        
        //connect to photon room
        BackendConnection.BC.GetConnectionInformation(StaticInformation.shortCode, thirdPartOfStart, ConnectionInfoFailed);
        
    }

    void thirdPartOfStart(ConnectionInformation pConnInf)
    {
        //Save pConnInfo in StaticInfo
        StaticInformation.connInf = pConnInf;
        //check ownership
        if (BackendConnection.BC.loggedIn && BackendConnection.BC.userId == StaticInformation.connInf.iduser)
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
        photonConnectionScript.connectToRoom(StaticInformation.connInf.photonroomname);
        
    }

    public async Task<string> generateAnchorAsync()
    {
        anchor.SetActive(true);
        anchorAppBar.SetActive(true);
        print("anchor generated async");
        return "AnchorIdFake";
    }

    public async Task loadAnchorAsync(string anchorID)
    {
        anchor.SetActive(true);
        anchorAppBar.SetActive(true);
        print("Anchor loaded async");
    }

    void Update()
    {
        
    }

    public void DownloadSucceed(string path)
    {
        //Load zip extracted in temp
        string filePath = path;
        StaticInformation.tempDirBase = Application.persistentDataPath;
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

    public void ConnectionInfoFailed(string msg)
    {
        print(msg);
    }

    public void showCanvas()
    {
        canvas.SetActive(true);
    }

    public void setshowHandoutInsteadOfScene(bool pValue)
    {
        if(showHandoutInsteadOfScene != pValue)
        {
            showHandoutInsteadOfScene = pValue;
            if (showHandoutInsteadOfScene)
            {
                loadHandoutFromStage(stageIndex);
            }
            else
            {
                loadSceneFromStage(stageIndex);
            }
        }
    }

    public void toogleshowHandoutInsteadOfScene()
    {
        showHandoutInsteadOfScene = !showHandoutInsteadOfScene;
        if (showHandoutInsteadOfScene)
        {
            loadHandoutFromStage(stageIndex);
        }
        else
        {
            loadSceneFromStage(stageIndex);
        }
    }
}
