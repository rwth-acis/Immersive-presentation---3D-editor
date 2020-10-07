using i5.Toolkit.Core.ModelImporters;
using i5.Toolkit.Core.ServiceCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorObjImporterBootstrapper : BaseServiceBootstrapper
{
    /// <summary>
    /// This class initializes the object importer service
    /// </summary>
    protected override void RegisterServices()
    {
        ObjImporter importer = new ObjImporter();
        ServiceManager.RegisterService(importer);
    }

    protected override void UnRegisterServices()
    {
        ServiceManager.RemoveService<ObjImporter>();
    }
}
