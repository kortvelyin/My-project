using SQLite4Unity3d;
using System;

public class Notes
{

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Creator { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public string Object { get; set; }
    public string Building { get; set; }
    public string Position { get; set; }
    
    public override string ToString()
    {
        return string.Format("{Id:{0}, Creator:{1}, Title:{2},  Object:{3}, Building:{4}, Position:{5}", Id, Creator, Title, Text, Object, Building,Position);
    }

    public string TitleToString()
    {
        return Title;
    }
}
