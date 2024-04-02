using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using UnityEditor.Experimental.GraphView;
using System.Linq;

[System.Serializable]
public class NoteListObject
{

    public List<Notes> data;
}
public class notes : MonoBehaviour
{
    public GameObject headCamera;
    public GameObject newNote;
    public GameObject savedNotes;
    public GameObject savedNotePrefab;
    public GameObject savedContext;
    
    public TMP_Text textNote;
    ContactService contactService;
    private TouchScreenKeyboard keyboard;

    // Start is called before the first frame update
    void Start()
    {
        contactService= new ContactService();
#if !UNITY_EDITOR
        CreateNotesDB();
#endif

        // OnGetNotesByProject();


        AddNoteToDB();
        OnGetNotesbyname();
    }

    // Update is called once per frame
    void Update()
    {
       // transform.position= headCamera.transform.position+ new Vector3(headCamera.transform.forward.x, 0, headCamera.transform.forward.z).normalized/2;
       // transform.LookAt(headCamera.transform.position);
    }

    private void ToConsole(Notes[] notes)
    {
        Debug.Log("NOtes: " + notes.Length + notes.ToString());
        foreach(var note in notes)
        {
            //var nN= Instantiate(savedNotePrefab,savedContext.transform);
            Debug.Log("1"+ note.ToString());
           // nN.transform.GetComponentInChildren<TMP_Text>().text = note.ToString();
            //ToConsole(note.ToString());
        }
    }

    private void ToConsole(string msg)
    {
        
        Debug.Log(msg);
    }

    public void CreateNotesDB()
    {
        contactService.CreateNotesTable();
        Debug.Log("Notes Table Created");
    }


    public void ShowKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.ASCIICapable);
    }
    public void AddNoteToDB()
    {

        Notes note = new Notes
        {
            user_id = AuthenticationService.Instance.PlayerId,
            title = "Projectname" + System.DateTime.Now.ToString(),
            text = "textNote.text" ,
            gobject = "none",
            project_id = "none",
            position = transform.position.ToString()
        };

        StartCoroutine(contactService.PostData_Coroutine(note));
        // int pk= contactService.AddNote(note);

        //Debug.Log("Primary Key: "+pk);
    }

    //Create the note list in the handy thingy
    public void OnGetNotesbyname()
    {
        StartCoroutine(GetRequest("http://localhost:3000/notes/11"));
        //var notes = contactService.GetNotes();
        //ToConsole(notes);
    }

   IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            switch(webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(string.Format("Something went wrong: {0}", webRequest.error));
                    break;
                case UnityWebRequest.Result.Success:

                     var note= JsonConvert.DeserializeObject<Notes>(webRequest.downloadHandler.text);
                    Debug.Log("Got: " + note.ToString());
                    CallNoteUpdate(note);
                    /* var notes= JsonConvert.DeserializeObject<NotesRoot>(webRequest.downloadHandler.text);

                     Debug.Log("Got: " + webRequest.downloadHandler.text);
                     
                     foreach (var note in notes.data) 
                     {
                     //Debug.Log(note.ToString());
                     }*/
                    //ToConsole(notes);


                    break;

            }
        }
    }


    public void CallNoteUpdate(Notes note)
    {
        if (note != null)
        {
            StartCoroutine(UploadNotes(note, 11));
        }
    }
    IEnumerator UploadNotes(Notes oldnote, int pk)
    {
        var newNote= UpdateNote(oldnote);
        Debug.Log("NewNote: "+newNote.ToString());
        var jNewNote= JsonUtility.ToJson(newNote);
        //byte[] myData = System.Text.Encoding.UTF8.GetBytes("This is some test data");
        using (UnityWebRequest www = UnityWebRequest.Put("http://localhost:3000/notes/11", jNewNote))
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

    public void OnGetNotesByProject()
    {
        //var notes = contactService.GetNotesFromProject("none");
        //ToConsole(notes);
    }

    public void ChangeTab()
    {
        if(savedNotes.activeSelf)
        {
            savedNotes.SetActive(false);
            newNote.SetActive(true);
            
            for (var i = savedContext.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(savedContext.transform.GetChild(i).gameObject);
            }
        }
        else
        { newNote.SetActive(false);
        savedNotes.SetActive(true);
            OnGetNotesbyname();
        }
    }



    public Notes UpdateNote(Notes oldNote)
    {

        Notes note = new Notes
        {
            //Id= oldNote.Id,
            user_id=oldNote.user_id,
            title = "Projectname" + System.DateTime.Now.ToString(),
            text = "just updated",
            gobject = oldNote.gobject,
            project_id = oldNote.project_id,
            position = oldNote.position
        };

        int pk = contactService.UpdateNote(note);
        return note;
        //Debug.Log("Primary Key: "+pk);
    }

}
