using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using UnityEngine;

public class Presentation
{
    public string presentationId { get; set; }
    public string ownerId { get; set; }
    public string name { get; set; }
    //the path where the resulting presentation file will be saved
    public string filepath { get; set; }
    //in tempFilePath the presentation will create a folder that will be exported as a ziped file to the filepath when it is saved
    //ToDo: maybe not needed because stored in the editor
    private string tempFilePath { get; set; }
    public DateTime timeOfCreation { get; set; }
    public ObservableCollection<Stage> stages { get; set; }

    public Presentation()
    {

    }
    public Presentation(string pPresentationId, string pPresentationName)
    {
        presentationId = pPresentationId;
        ownerId = "DemoOwner1";

        //Initialize all parameters
        name = pPresentationName;
        timeOfCreation = DateTime.Now;

        //Create a new Stage
        stages = new ObservableCollection<Stage>();
        stages.Add(new Stage(presentationId + "-1"));

        //Initialize the folder structure in temp
    }
}

public class Scene
{
    public string sceneId { get; set; }
    public DateTime timeOfCreation { get; set; }
    public ObservableCollection<Element3D> elements { get; set; }

    public Scene()
    {

    }
    public Scene(string pSceneId)
    {
        sceneId = pSceneId;
        timeOfCreation = DateTime.Now;
        elements = new ObservableCollection<Element3D>();
    }
}

public class Stage
{
    public string stageId { get; set; }
    public DateTime timeOfCreation { get; set; }
    public Canvas canvas { get; set; }
    public Scene scene { get; set; }
    public Handout handout { get; set; }

    public Stage()
    {

    }
    public Stage(string pStageId)
    {
        stageId = pStageId;
        timeOfCreation = DateTime.Now;
        canvas = new Canvas(stageId + "-c-1");
        scene = new Scene(stageId + "-s-1");
        handout = new Handout(stageId + "h-1");
    }
}

public class Canvas
{
    public string canvasId { get; set; }
    public DateTime timeOfCreation { get; set; }
    public ObservableCollection<Element2D> elements { get; set; }

    public Canvas() { }
    public Canvas(string pCanvasId)
    {
        canvasId = pCanvasId;
        timeOfCreation = DateTime.Now;
        elements = new ObservableCollection<Element2D>();
    }
}

public class Handout
{
    public string handoutId { get; set; }
    public DateTime timeOfCreation { get; set; }
    public ObservableCollection<Element3D> elements { get; set; }

    public Handout() { }
    public Handout(string pHandoutId)
    {
        handoutId = pHandoutId;
        timeOfCreation = DateTime.Now;
        elements = new ObservableCollection<Element3D>();
    }
}

public class Element : INotifyPropertyChanged
{
    public string elementId { get; set; }

    public Element()
    {
        elementId = "";
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnProperyChanged(string name)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}

public class Element2D : Element
{
    //The Position describes where the left top corner of the element will be positioned. The value is in percentage (50 is in the middle).
    private double _xPosition;
    public double xPosition
    {
        get
        {
            return _xPosition;
        }
        set
        {
            _xPosition = value;
            OnProperyChanged("xPosition");
        }
    }
    public double _yPosition;
    public double yPosition
    {
        get
        {
            return _yPosition;
        }
        set
        {
            _yPosition = value;
            OnProperyChanged("yPosition");
        }
    }
    private bool _highlighted;
    public bool highlighted
    {
        get
        {
            return _highlighted;
        }
        set
        {
            _highlighted = value;
            OnProperyChanged("highlighted");
        }
    }

    public Element2D() : base()
    {
        highlighted = false;
        xPosition = 50;
        yPosition = 50;
    }

    public Element2D(double pXPosition, double pYPosition) : base()
    {
        highlighted = false;
        xPosition = pXPosition;
        yPosition = pYPosition;
    }
}

public class Element3D
{
    //The Position describes where the center of the element will be positioned. The value is in percentage (0 is in the middle)
    public double xPosition { get; set; }
    public double yPosition { get; set; }
    public double zPosition { get; set; }

    //The Scale describes how much of the Axis this element will cover in percentage (100 is the complete axis)
    public double xScale { get; set; }
    public double yScale { get; set; }
    public double zScale { get; set; }

    public string relativePath { get; set; }
    public string filename
    {
        get
        {
            try
            {
                if (relativePath != "")
                {
                    return Path.GetFileNameWithoutExtension(relativePath);
                }
                else
                {
                    return "3D Element";
                }
            }
            catch
            {
                return "3D Element";
            }
        }
        set
        {

        }
    }

    public Element3D() : base()
    {
        //Default Position
        xPosition = 0;
        yPosition = 0;
        zPosition = 20;
        //Default Scale
        xScale = 20;
        yScale = 20;
        zScale = 20;
    }

    public Element3D(string pRelativePath) : base()
    {
        relativePath = pRelativePath;
        //Default Position
        xPosition = 0;
        yPosition = 0;
        zPosition = 20;
        //Default Scale
        xScale = 20;
        yScale = 20;
        zScale = 20;
    }

    public Element3D(string pRelativePath, double pXPosition, double pYPosition, double pZPosition) : base()
    {
        relativePath = pRelativePath;

        xPosition = pXPosition;
        yPosition = pYPosition;
        zPosition = pZPosition;
        //Default Scale
        xScale = 20;
        yScale = 20;
        zScale = 20;
    }

    public Element3D(string pRelativePath, double pXPosition, double pYPosition, double pZPosition, double pXScale, double pYScale, double pZScale) : base()
    {
        relativePath = pRelativePath;

        xPosition = pXPosition;
        yPosition = pYPosition;
        zPosition = pZPosition;

        xScale = pXScale;
        yScale = pYScale;
        zScale = pZScale;
    }
}

public class Image2D : Element2D
{
    private string _relativeImageSource;
    public string relativeImageSource
    {
        get
        {
            return _relativeImageSource;
        }
        set
        {
            _relativeImageSource = value;
            OnProperyChanged("relativeImageSource");
        }
    }
    private double _xScale;
    public double xScale
    {
        get
        {
            return _xScale;
        }
        set
        {
            _xScale = value;
            OnProperyChanged("xScale");
        }
    }
    private double _yScale;
    public double yScale
    {
        get
        {
            return _yScale;
        }
        set
        {
            _yScale = value;
            OnProperyChanged("yScale");
        }
    }

    public Image2D() : base()
    {
        //Default Scale
        xPosition = 50;
        yPosition = 50;
        xScale = 20;
        yScale = 20;
    }

    public Image2D(string pRelativeImageSource) : base()
    {
        //Default Scale
        xPosition = 50;
        yPosition = 50;
        xScale = 20;
        yScale = 20;
        relativeImageSource = pRelativeImageSource;
    }

    public Image2D(double pXPosition, double pYPosition, double pXScale, double pYScale) : base(pXPosition, pYPosition)
    {
        xScale = pXScale;
        yScale = pYScale;
    }

}

public class Text2D : Element2D
{
    private string _content;
    public string content
    {
        get
        {
            return _content;
        }
        set
        {
            _content = value;
            OnProperyChanged("content");
        }
    }
    private int _fontsize;
    public int fontsize
    {
        get
        {
            return _fontsize;
        }
        set
        {
            _fontsize = value;
            OnProperyChanged("fontsize");
        }
    }

    public Text2D()
    {
        xPosition = 20;
        yPosition = 20;
        content = "New Text";
        fontsize = 40;
    }
}