using SQLite4Unity3d;

public class Notes
{

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }
    public string Object { get; set; }
    public string Building { get; set; }
    public string Level { get; set; }
    public string Room { get; set; }

    public override string ToString()
    {
        return string.Format("[Note: Id={0}, Name={1},  Text={2}, Object={3}, Building={4}, Level={5}, Room={6}]", Id, Name, Text, Object, Building, Level, Room);
    }
}
