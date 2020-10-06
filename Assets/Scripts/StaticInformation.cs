using System;

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
}
