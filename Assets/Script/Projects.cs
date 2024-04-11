using Newtonsoft.Json;
using SQLite4Unity3d;
using System;
using System.Collections.Generic;

[Serializable]
public class Project
{
    public int ID { get; set; }
    public string name { get; set; } //projects name
    public string start { get; set; } //date
    public string finish { get; set; } //date
    public string layername { get; set; } //pictures, design, interior design, BMI...
    public string model { get; set; }// gameobject list or url for AssetBundle
}

public class ProjectRoot
{
    public int status { get; set; }
    public List<Project> data { get; set; }
    public bool success { get; set; }
}