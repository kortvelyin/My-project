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
//using UnityEditor.Experimental.GraphView;
using System.Linq;
using System.Net;
//using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEditor;

public struct TableTypes
{
    public string urlContains;
    public string type;
}
public class NotesManager : MonoBehaviour
{
    public GameObject headCamera;
    public GameObject newNote;
    public GameObject savedNotes;
    public GameObject savedNotePrefab;
    public GameObject savedContext;
    public TableTypes[] tableTypes;
    
    public TMP_Text textNote;
    public TMP_Text titleNote;
    ContactService contactService;
    private TouchScreenKeyboard keyboard;
    LoginScreenUI loginScreenUI;
    private string baseUser_id;
   


    // Start is called before the first frame update
    void Start()
    {
       
       loginScreenUI = GameObject.Find("LoginScreenUI").GetComponent<LoginScreenUI>();

        contactService = GameObject.Find("AuthManager").GetComponent<ContactService>();
        
        // OnGetNotesByProject();


        //  AddNoteToDB();
        //OnGetNotesbyname();
    }

    // Update is called once per frame
    void Update()
    {
        
       // transform.position= headCamera.transform.position+ new Vector3(headCamera.transform.forward.x, 0, headCamera.transform.forward.z).normalized/2;
       // transform.LookAt(headCamera.transform.position);
    }


     
    public void ToConsole(List<Notes> notes)
    {
        Debug.Log("Notes: " + notes.ToString());
        foreach(var note in notes)
        {
            var nN= Instantiate(savedNotePrefab,savedContext.transform);
            Debug.Log("1"+ note.ToString());
            nN.transform.GetComponentInChildren<TMP_Text>().text = note.ToString();
            //nN.gameObject.name = note.ID;
            //ToConsole(note.ToString());
        }
    }


   

  
    private void ToConsole(string msg)
    {
        Debug.Log(msg);
    }

  /*  public void CreateNotesDB()
    {
        contactService.CreateNotesTable();
        Debug.Log("Notes Table Created");
    }*/


    public void ShowKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.ASCIICapable);
    }
    public void AddNoteToDB()
    {
        Notes note = new()
        {
            user_id = baseUser_id,//AuthenticationService.Instance.PlayerId,
            title = titleNote.text,//+" Projectname" + System.DateTime.Now.ToString(),
            text = textNote.text ,
            gobject = "none",
            project_id = "none",
            position = transform.position.ToString()
        };

        var jnote = JsonUtility.ToJson(note);
        StartCoroutine(contactService.PostData_Coroutine(jnote, "http://localhost:3000/notes"));
        // int pk= contactService.AddNote(note);

        //Debug.Log("Primary Key: "+pk);
    }

    //Create the note list in the handy thingy
    public void OnGetNotesbyname()
    {
        StartCoroutine(contactService.GetRequest("http://localhost:3000/notes"));
        //var notes = contactService.GetNotes();
        //ToConsole(notes);
    }

   


    public void GetProjectList()
    {
        Debug.Log("GetProjectList()");
        //StartCoroutine(GetRequest("http://localhost:3000/projects"));
    }

  

    public void CallNoteUpdate(Notes note)
    {
        if (note != null)
        {
            StartCoroutine(contactService.UploadNotes("note", 11,"uri"));
        }
    }
  

    public void OnGetNotesByProject()
    {
        //var notes = contactService.GetNotesFromProject("none");
        //ToConsole(notes);
    }

    public void ChangeTab(TMP_Text text)
    {
        if(savedNotes.activeSelf)
        {
            savedNotes.SetActive(false);
            newNote.SetActive(true);
            text.text = "List";
            for (var i = savedContext.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(savedContext.transform.GetChild(i).gameObject);
            }
        }
        else
        { newNote.SetActive(false);
        savedNotes.SetActive(true);
            OnGetNotesbyname();
            text.text = "New";
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

        //int pk = UpdateNote(note.id);
        return note;
        //Debug.Log("Primary Key: "+pk);
    }

}
