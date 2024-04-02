using Newtonsoft.Json;
using SQLite4Unity3d;
using System;

[Serializable]
public class Notes
{

    //[PrimaryKey, AutoIncrement]


     int ID=0; //{ get; set; }

    public string user_id; //{ get; set; }
    
    public string title;// { get; set; }
    
    public string text; //{ get; set; }
    
    public string gobject;// { get; set; }
    
    public string project_id;// { get; set; }
    
    public string position; //{ get; set; }
    
    public override string ToString()
    {
        return string.Format(" user_id:{0}, title:{1}, text:{2}, gobject:{3}, project_id:{4}, position:{5}", user_id, title, text, gobject, project_id, position);
    }

    public string TitleToString()
    {
        return title;
    }
}
