using SQLite4Unity3d;
using System;

public class Notes
{

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Creator { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public DateTime Time { get; set; }
    public string Object { get; set; }
    public string Building { get; set; }
    public string Level { get; set; }
    public string Room { get; set; }

    public override string ToString()
    {
        return string.Format("[Note: Id={0}, Creator={1}, Title={2},  Text={3}, Object={4}, Building={5}, Level={6}, Room={7}]", Id, Creator, Title, Text, Object, Building, Level, Room);
    }

    public string TitleToString()
    {
        return Title;
    }
}
