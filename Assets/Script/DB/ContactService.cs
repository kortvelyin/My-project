using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ContactService 
{
    DB dB;
    string jnote;
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


    public IEnumerator PostData_Coroutine(string json, string uri)
    {
        //Debug.Log("In post: "+note.ToString());
        //jnote= JsonUtility.ToJson(note);
        using (UnityWebRequest www = UnityWebRequest.Post(uri, json, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!: "+ jnote);
            }
        }
    }

    public IEnumerable<Notes> GetNotes()
    {
        return dB.GetConnection().Table<Notes>();
    }

    public IEnumerable<Notes> GetNotesFromProject(string name)
    {
        return dB.GetConnection().Table<Notes>().Where(x => x.project_id == name);
    }

    public int UpdateNote(Notes note)
    {

        return dB.GetConnection().Update(note);
    }

}
