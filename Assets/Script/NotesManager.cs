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
//using UnityEditor.SearchService;
//using UnityEditor.Experimental.GraphView;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

//using UnityEngine.UIElements;
/// <summary>
/// Handling the Note creation, listing, communication with DB
/// Everything that has to do with the notes. 
/// Notes class--> Notes.cs
/// NotesRoot class -->NotesRoot.cs (this is for the api list call)  
/// </summary>
public struct TableTypes
{
    public string urlContains;
    public string type;
}
public class NotesManager : MonoBehaviour
{
    public GameObject headCamera;
    public GameObject newNote; //UI
    public GameObject savedNotes; //UI
    public Button savedNotePrefab; //UI listitem
    public GameObject savedContext;//UI list content
    public GameObject oneContext;//UI simple note view data content
    public TableTypes[] tableTypes;
    public Button saveB; //UI savebutton
    public Button colorGO;//coloring on/off switch
    [HideInInspector]
    public Button gOname; //selected gameobject UI
    public Button gOpos;//note pos based on selected go UI
    public Button gOuser;//note creator based on selected go UI
    public Button layerNote; //Note pop ups 
    
    public TMP_InputField textNote; //UI text
    public TMP_InputField titleNote;//UI text
    private string baseUser_id;
    private TouchScreenKeyboard keyboard;

    //scripts
    LoginScreenUI loginScreenUI;
    authManager authSc;
    ContactService contactService;
    Build buildSc;


    // Start is called before the first frame update
    void Start()
    {
       
       loginScreenUI = GameObject.Find("LoginScreenUI").GetComponent<LoginScreenUI>();
        authSc = GameObject.Find("AuthManager").GetComponent<authManager>();
        contactService = GameObject.Find("AuthManager").GetComponent<ContactService>();
        buildSc = GameObject.Find("Building").GetComponent<Build>();
        // OnGetNotesByProject();
        gOuser = Instantiate(savedNotePrefab, oneContext.transform);
        gOuser.transform.GetComponentInChildren<TMP_Text>().text = authSc.userData.name;
        gOname = Instantiate(savedNotePrefab, oneContext.transform);
        gOpos = Instantiate(savedNotePrefab, oneContext.transform);

        //  AddNoteToDB();
        //OnGetNotesbyname();
    }

    // Update is called once per frame
    void Update()
    {
        
       // transform.position= headCamera.transform.position+ new Vector3(headCamera.transform.forward.x, 0, headCamera.transform.forward.z).normalized/2;
       // transform.LookAt(headCamera.transform.position);
    }


     
    public void ToConsole(List<Notes> notes) //list out notes
    {
        Debug.Log("Notes: " + notes.ToString());
        foreach(var note in notes)
        {
            

            GameObject[] noteLayer = GameObject.FindGameObjectsWithTag("Note");
            foreach(var noteItem in noteLayer)
            {
                Destroy(noteItem.gameObject);
            }
            var nN = Instantiate(savedNotePrefab, savedContext.transform);
            nN.transform.SetSiblingIndex(0);
            Debug.Log("1" + note.ToString());
            nN.transform.GetComponentInChildren<TMP_Text>().text = authSc.GetUserNameByID(Int32.Parse(note.user_id)) + " " + note.title;
            nN.onClick.AddListener(() => ShowNote(JsonUtility.ToJson(note)));
            
            //The pop ups of the notes as a layer throughout the bulding
            var lN= Instantiate(layerNote);//make the pop ups of notes
            
            if ((note.position).Contains("Item"))
            {   var trans = JsonHelper.FromJson<string>(note.position);
                lN.tag = "Note";
                lN.transform.GetChild(0).transform.Find("NoteTitle").GetComponent<TMP_Text>().text = note.title;
                lN.transform.GetChild(0).transform.Find("Creator").GetComponent<TMP_Text>().text = "Creator: " + authSc.GetUserNameByID(Int32.Parse(note.user_id));
                lN.onClick.AddListener(() => ShowNote(JsonUtility.ToJson(note)));
                lN.transform.position= JsonConvert.DeserializeObject<Vector3>(trans[0]);
            lN.transform.rotation = JsonConvert.DeserializeObject<Quaternion>(trans[1]);
            }
            
            
            
            //nN.onClick.AddListener(() => OnGetNotesbyID(note.));//get the id
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
        var goN = Instantiate(layerNote);
        Transform transform= gameObject.transform;
        if (buildSc.selectedGo != null)
        {
            Vector3 vec3 = new Vector3(transform.position.x + (transform.position.x - buildSc.selectedGo.transform.position.x) / 2, transform.position.y, transform.position.z);
            goN.transform.position = vec3;
        }
        else
            goN.transform.position = transform.position;


        
        goN.transform.rotation.SetLookRotation(Camera.main.transform.position);
        
        string[] trans=new String[2];
        trans[0] = JsonUtility.ToJson(goN.transform.position);
        trans[1]= JsonUtility.ToJson(goN.transform.rotation);
        var jtrans = JsonHelper.ToJson(trans);
        Notes note = new()
        {
            user_id = authSc.userData.ID.ToString(),//AuthenticationService.Instance.PlayerId,
            title = titleNote.text,//+" Projectname" + System.DateTime.Now.ToString(),
            text = textNote.text,
            gobject = gOname.transform.GetComponentInChildren<TMP_Text>().text,
            project_id = "30",
            
            position = jtrans//jsoned transform
        };
         var jnote = JsonUtility.ToJson(note);
        goN.onClick.AddListener(() => ShowNote(jnote));
        goN.tag = "Note";
        goN.transform.GetChild(0).transform.Find("NoteTitle").GetComponent<TMP_Text>().text = note.title;
        goN.transform.GetChild(0).transform.Find("Creator").GetComponent<TMP_Text>().text = "Creator: "+ authSc.GetUserNameByID(Int32.Parse(note.user_id));

        StartCoroutine(contactService.PostData_Coroutine(jnote, "http://"+authSc.ipAddress+":3000/notes"));
        // int pk= contactService.AddNote(note);

        //Debug.Log("Primary Key: "+pk);
    }

     public void ShowNoteItem(string jnote)
    {
        Notes note = JsonConvert.DeserializeObject<Notes>(jnote);
        Transform noteTrans= JsonConvert.DeserializeObject<Transform>(note.position);

        //jnnote to note
        //note to shownote
    }

    public Notes JnoteToNote(string jnote)
    {
        Notes note= JsonConvert.DeserializeObject<Notes>(jnote);

        return note;
    }



    //Create the note list in the handy thingy
    public void OnGetNotesbyname()
    {
        StartCoroutine(contactService.GetRequest("http://"+authSc.ipAddress+":3000/notes"));
        //var notes = contactService.GetNotes();
        //ToConsole(notes);
    }


    public void OnGetOneNotebyID(string id)
    {
        StartCoroutine(contactService.GetRequest("http://"+authSc.ipAddress+":3000/notes/:" + id));
        //var notes = contactService.GetNotes();
        //ToConsole(notes);
    }

    public void OnGetNoteListbyProjectID(string id)
    {
        StartCoroutine(contactService.GetRequest("http://"+authSc.ipAddress+":3000/notes/byproject/" + id));
        //var notes = contactService.GetNotes();
        //ToConsole(notes);
    }

    public void GetProjectList()
    {
        Debug.Log("GetProjectList()");
        //StartCoroutine(GetRequest("http://"+authSc.ipAddress+":3000/projects"));
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
           
            //gOname = Instantiate(savedNotePrefab, oneContext.transform);
            gOuser.transform.GetComponentInChildren<TMP_Text>().text = authSc.userData.name;
            gOname.transform.GetComponentInChildren<TMP_Text>().text = "Select Object";
            gOpos.transform.GetComponentInChildren<TMP_Text>().text = "Select Object";
            textNote.text = "";
            titleNote.text = "";
            //text.text = "List";
            for (var i = savedContext.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(savedContext.transform.GetChild(i).gameObject);
            }
           
        }
        else
        { newNote.SetActive(false);
        savedNotes.SetActive(true);
            OnGetNoteListbyProjectID("30");
            
            //text.text = "New";
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

    public void ShowNote(string jnote)
    {
        newNote.SetActive(true);
        
        savedNotes.SetActive(false) ;
        
        var oldNote = JsonConvert.DeserializeObject<Notes>(jnote);
        /*
             //Id= oldNote.Id,
             user_id = oldNote.user_id,
             title = "Projectname" + System.DateTime.Now.ToString(),
             text = "just updated",
             gobject = oldNote.gobject,
             project_id = oldNote.project_id,
             position = oldNote.position

         */

        titleNote.text=oldNote.title;
        textNote.text = oldNote.text;
        
        //var project = Instantiate(savedNotePrefab, oneContext.transform);

        gOname.transform.GetComponentInChildren<TMP_Text>().text = oldNote.gobject;
        var trans = JsonHelper.FromJson<string>(oldNote.position);
        gOpos.transform.GetComponentInChildren<TMP_Text>().text = trans[0];

        //project.transform.GetComponentInChildren<TMP_Text>().text = oldNote.user_id;
        gOuser.transform.GetComponentInChildren<TMP_Text>().text = authSc.GetUserNameByID(Int32.Parse(oldNote.user_id));
      //  Debug.Log("username: " + authSc.GetUserNameByID(Int32.Parse(oldNote.user_id)));
        //nN.onClick.AddListener(() => OnGetNotesbyID(note.));//get the id
        //nN.gameObject.name = note.ID;
        //ToConsole(note.ToString());
        //int pk = UpdateNote(note.id);
        //Debug.Log("1" + note.ToString())
        //Debug.Log("Primary Key: "+pk);
    }

}
