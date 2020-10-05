using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentationElement
{
    public int iduser { get; set; }
    public int idpresentation { get; set; }
    public string filepath { get; set; }
    public object timeofcreation { get; set; }
    public string name { get; set; }
    public long? lastchange { get; set; }
}

public class ListResponse
{
    public List<PresentationElement> presentations { get; set; }
}
