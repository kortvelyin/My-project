using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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


    public IEnumerator PostData_Coroutine(Notes note)
    {
        string uri = "http://localhost:3000/note";
        
       using(UnityWebRequest request=UnityWebRequest.PostWwwForm(uri, JsonUtility.ToJson(note)))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
                Debug.Log(request.downloadHandler.text);
        }
    }

    public IEnumerable<Notes> GetNotes()
    {
        return dB.GetConnection().Table<Notes>();
    }

    public IEnumerable<Notes> GetNotesFromProject(string name)
    {
        return dB.GetConnection().Table<Notes>().Where(x => x.Building == name);
    }

    public int UpdateNote(Notes note)
    {

        return dB.GetConnection().Update(note);
    }

}
