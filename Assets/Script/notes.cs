using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    }

    // Update is called once per frame
    void Update()
    {
       // transform.position= headCamera.transform.position+ new Vector3(headCamera.transform.forward.x, 0, headCamera.transform.forward.z).normalized/2;
       // transform.LookAt(headCamera.transform.position);
    }

    private void ToConsole(IEnumerable<Notes> notes)
    {
        foreach (var note in notes)
        {
            var nN= Instantiate(savedNotePrefab,savedContext.transform);
            Debug.Log("1");
            nN.transform.GetComponentInChildren<TMP_Text>().text = note.ToString();
            ToConsole(note.ToString());
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
            Creator= "Name",
            Title = "Projectname" + System.DateTime.Now.ToString(),
            Text = textNote.text,
            Time= System.DateTime.Now,
            Object = "none",
            Building = "none",
            Level = "none",
            Room = "none"
        };
       
       int pk= contactService.AddNote(note);
       
       //Debug.Log("Primary Key: "+pk);
    }

    //Create the note list in the handy thingy
    public void OnGetNotesbyname()
    {
        var notes = contactService.GetNotes();
        ToConsole(notes);
    }

    public void OnGetNotesByProject()
    {
        var notes = contactService.GetNotesFromProject("none");
        ToConsole(notes);
    }

    public void ChangeTab()
    {
        if(savedNotes.activeSelf)
        {
            savedNotes.SetActive(false);
            newNote.SetActive(true);
            
            for (var i = savedContext.transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(savedContext.transform.GetChild(i).gameObject);
            }
        }
        else
        { newNote.SetActive(false);
        savedNotes.SetActive(true);
            OnGetNotesbyname();
        }
    }



    public void UpdateNote(Notes oldNote)
    {

        Notes note = new Notes
        {
            Id= oldNote.Id,
            Creator=oldNote.Creator,
            Title = "Projectname" + System.DateTime.Now.ToString(),
            Text = textNote.text,
            Object = oldNote.Object,
            Building = oldNote.Building,
            Level = oldNote.Level,
            Room = oldNote.Room
        };

        int pk = contactService.UpdateNote(note);

        //Debug.Log("Primary Key: "+pk);
    }

}
