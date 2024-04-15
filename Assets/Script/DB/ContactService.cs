using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ContactService : MonoBehaviour
{
    NotesManager notesManager;
    authManager authM;
    Build build;
    DB dB;
    string jnote;


     void Start()
    {
        authM = GameObject.Find("AuthManager").GetComponent<authManager>();
        
        notesManager = GameObject.Find("NotesUIDocker").GetComponent<NotesManager>();
        build = GameObject.Find("Building").GetComponent<Build>();
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

    public void JsonToDB(string json, string url)
    {
        /*if (json != null)
        {
            for(int i = 0; i < tableTypes.Length; i++)
            {
                if (url.Contains(tableTypes[i].urlContains))
                {
                    var notes = JsonConvert.DeserializeObject<tableTypes[i].type> (json);
                }
            }
        }*/
        switch (url)
        {
            case "http://localhost:3000/notes/":
                {
                    var notes = JsonConvert.DeserializeObject<Notes>(json);
                    //the update
                    break;
                }
            case "http://localhost:3000/notes":
                {
                    var notes = JsonConvert.DeserializeObject<NotesRoot>(json);
                    notesManager.ToConsole(notes.data);
                    break;
                }
            case "http://localhost:3000/users":
                {
                    //Debug.Log(json);
                    if(authM==null)
                        authM= GameObject.Find("AuthManager").GetComponent<authManager>();
                    authM.Auth(json);
                    break;
                }
            case "http://localhost:3000/projects":
                {
                    var projects = JsonConvert.DeserializeObject<ProjectRoot>(json);
                    //list these, string on gO, klick andturn that to blocks
                    break;
                }
            case "http://localhost:3000/projects/byname/Test":
                {
                    if (build == null)
                        build = GameObject.Find("Building").GetComponent<Build>();
                    var projects = JsonConvert.DeserializeObject<ProjectRoot>(json);
                    build.ToConsole(projects.data);
                    break;
                }
            default:
                {
                   
                    Debug.Log("Couldn't find type: "+url);
                    break;
                }

        }





    }


    public IEnumerator PostData_Coroutine(string json, string uri)
    {
        Debug.Log("In post: "+json+" url: "+uri);
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


   public IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(string.Format("Something went wrong: {0}", webRequest.error));
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("In GetRequestt: " + webRequest.downloadHandler.text);
                    JsonToDB(webRequest.downloadHandler.text, uri);

                    
                  

                    break;

            }
        }
    }

       public   IEnumerator UploadNotes(string json, int pk, string uri)
    {
        //var newNote = UpdateNote(oldnote);
        //Debug.Log("NewNote: " + newNote.ToString());
        //var jNewNote = JsonUtility.ToJson(newNote);
        //byte[] myData = System.Text.Encoding.UTF8.GetBytes("This is some test data");
        using (UnityWebRequest www = UnityWebRequest.Put(uri, json))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Upload complete!");
            }
        }
    }
}
