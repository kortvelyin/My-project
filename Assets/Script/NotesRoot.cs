using Newtonsoft.Json;
using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class NotesInRoot
{
    public int ID; 
    public string user_id;
    public string title;
    public string text;
    public string gobject;
    public string project_id;
    public string position;

public override string ToString()
{
    return string.Format(" user_id:{0}, title:{1}, text:{2}, gobject:{3}, position:{4}", user_id, title, text, gobject, position);
}
}


public class NotesRoot
{
    public int status;
    public List<Notes> data { get; set; }
    public bool success;
}
