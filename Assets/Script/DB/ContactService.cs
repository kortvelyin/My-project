using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactService 
{
    DB dB;

    public ContactService() 
    {
        dB = new DB();
    }

    public void CreateNotesTable()
    {
       dB.GetConnection().DropTable<Notes>();
       dB.GetConnection().CreateTable<Notes>();
    }

    public int AddNote(Notes note)
    {

        return dB.GetConnection().Insert(note);
    }

    public IEnumerable<Notes> GetNotes()
    {
        return dB.GetConnection().Table<Notes>();
    }

    public IEnumerable<Notes> GetNotesFromProject(string name)
    {
        return dB.GetConnection().Table<Notes>().Where(x => x.Building == name);
    }

}
