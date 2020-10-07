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

public class EditorSceneHandler : MonoBehaviour
{
    public string tempDirBase { get; set; } //Path to the start of the temporary folder of the actual windows user 
    public const string tempDownloadSuffix = "ImPres3D\\downloads\\";
    public string tempDownloadDir
    {
        get
        {
            return tempDirBase + tempDownloadSuffix;
        }
    } //Path where the presentations are downloaded to.

    public const string tempSuffix = "ImPres3D\\presentation\\";
    public string tempPresDir
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
    public string tempSaveDir
    {
        get
        {
            return tempDirBase + tempSaveSuffix;
        }
    } //Path where the presentation is saved before uploading.

    

    //private DataSerializer dataSerializer = new DataSerializer();
    private JsonSerializerSettings jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

    Presentation openPresentation;
    List<GameObject> actualSceneGameObjList;

    // Start is called before the first frame update
    void Start()
    {
        print(StaticInformation.selectedPresName);

        createWorkingDir();

        string filename = Path.GetFileNameWithoutExtension(StaticInformation.selectedPresElem.filepath);
        if (filename == "") filename = "downloadPres";
        filename = filename + ".pres";
        string downloadFilePath = tempDownloadDir + filename;


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
        print(openPresentation.name);
        print(tempDirBase);
        //import all Gameobjects from the presentation
        actualSceneGameObjList = new List<GameObject>();
        create3DObjectsFromScene(openPresentation.stages[0].scene, actualSceneGameObjList);
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
        tempDirBase = Path.GetTempPath().ToString();
        createCleanDirectory(tempPresDir);
        ZipFile.ExtractToDirectory(filePath, tempPresDir);

        //Deserialize json
        //*openPresentation = dataSerializer.DeserializerJson(typeof(Presentation), tempPresDir + presentationJsonFilename) as Presentation;
        openPresentation = JsonConvert.DeserializeObject<Presentation>(File.ReadAllText(tempPresDir + presentationJsonFilename), jsonSettings);
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
        tempDirBase = Path.GetTempPath().ToString();
        createCleanDirectory(tempSaveDir);
        createCleanDirectory(tempDownloadDir);
        createCleanDirectory(tempPresDir);
        createCleanDirectory(tempPresDir + tempSub2D);
        createCleanDirectory(tempPresDir + tempSub3D);
        createCleanDirectory(tempPresDir + tempSub3D + tempSubSubScene);
        createCleanDirectory(tempPresDir + tempSub3D + tempSubSubHandout);
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
            GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportFromFileAsync(tempPresDir + pScene.elements[i].relativePath);
            pSceneGameObjList.Add(obj);
            print("imported obj: " + i);
        }
    }
}
