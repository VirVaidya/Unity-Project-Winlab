using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
public class VoiceCommandHandler : MonoBehaviour, IMixedRealitySpeechHandler
{
    GameObject protein; // The object to change color
    Material defaultMaterial;
    Material transmaterial;
    bool commandListActive = true;

    void Start()
    {
        transmaterial = Resources.Load("Transmaterial", typeof(Material)) as Material;
        Color32 hardtransparent = new Color32(0x00, 0x00, 0x00, 0x00);    
    }

    void Update() 
    {
        protein = GameObject.Find("Protein");
        defaultMaterial = protein.transform.Find("Atoms").transform.GetChild(0).GetComponent<Renderer>().material;
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
 
        switch (eventData.Command.Keyword.ToLower()) //Have to hard code every command
        {
            case "hide atoms":
                hideAtoms();
                break;
            case "hide helices":
                hidehelices();
                break;
            case "hide sheets":
                hidesheets();
                break;
            case "hide connections":
                hideconnects();
                break;
            case "show protein":
                showeverything();
                break;
            case "hide protein":
                hideeverything();
                break;
            case "show atoms":
                showAtoms();
                break;
            case "show helices":
                showhelices();
                break;
            case "show sheets":
                showsheets();
                break;
            case "show connections":
                showconnects();
                break;
            case "show carbon":
                showspecificatom("C");
                break;
            case "show oxygen":
                showspecificatom("O");
                break;
            case "show nitrogen":
                showspecificatom("N");
                break;
            case "show hydrogen":
                showspecificatom("H");
                break;
            case "show sulfer":
                showspecificatom("S");
                break;
            case "show phosphorous":
                showspecificatom("P");
                break;
            case "show bromine":
                showspecificatom("BR");
                break;
            case "show colors":
                showcolors();
                break;
            case "show zinc":
                showspecificatom("ZN");
                break;
            case "show chlorine":
                showspecificatom("CL");
                break;
            case "show fluorine":
                showspecificatom("F");
                break;
            case "show calcium":
                showspecificatom("CA");
                break;
            case "show magnesium":
                showspecificatom("MG");
                break;
            case "scale up":
                scalebigger();
                break;
            case "scale down":
                scalesmaller();
                break;
            case "come here":
                comehere();
                break;
            case "show phosphate sequence":
                showrna();
                break;
            case "hide phosphate sequence":
                hiderna();
                break;
            case "show covalent bonds":
                showlinks();
                break;
            case "hide covalent bonds":
                hidelinks();
                break;
            case "show water molecules":
                showspecificatom("HOH");
                break;
            case "toggle commands":
                toggleList();
                break;
            case "close simulation":
                EditorApplication.ExitPlaymode();
                break;
            case "teleport text box":
                teleportTextBox();
                break;
            case "hide text box":
                hidetextbox();
                break;
            case "show text box":
                showtextbox();
                break;
            case "move keyboard back":
                MoveKeyboardback();
                break;
            case "move keyboard forward":
                MoveKeyboardForward();
                break;
            case "delete protein":
                Destroy(protein.transform.parent);
                break;
            default:
                Debug.Log(eventData.Command.Keyword.ToLower());
                break;
        }
    }

    private void comehere()
    { 
        Transform cameraTransform = Camera.main.transform;
        Vector3 cameraPosition = cameraTransform.position;
        Vector3 cameraForward = cameraTransform.forward;
        float distanceInFront = 0.2f; 
        Vector3 newPosition = cameraPosition + cameraForward * distanceInFront;
        GameObject hitbox = protein.transform.parent.gameObject;
        hitbox.transform.position = newPosition;
    }

    private void scalebigger()
    {
        GameObject hitbox = protein.transform.parent.gameObject;
        
        Vector3 newScale = hitbox.transform.localScale * 2;
        hitbox.transform.localScale = newScale;
    }
    private void scalesmaller()
    {
        GameObject hitbox = protein.transform.parent.gameObject;
        Vector3 newScale = hitbox.transform.localScale / 2;
        hitbox.transform.localScale = newScale;
    }
    private void showcolors()
    {
        Transform atoms = protein.transform.Find("Atoms");
        atoms.gameObject.SetActive(true);
        foreach (Transform child in atoms.transform)
        {
            // Check if the child has a Renderer component
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Get the tag of the child GameObject
                string name = child.name;
                renderer.material.color = ElementColor.AssignColor(name);
            }
        }
    }
    private void teleportTextBox()
    {
        Transform cameraTransform = Camera.main.transform;
        Vector3 cameraPosition = cameraTransform.position;
        Vector3 cameraForward = cameraTransform.forward;
        float distanceInFront = 0.5f;  // Adjust the distance as needed
        Vector3 newPosition = cameraPosition + cameraForward * distanceInFront;
        TMP_InputField inputField = GetComponent<TMP_InputField>();

        if (inputField != null)
        {
            inputField.gameObject.transform.position = newPosition;
            inputField.gameObject.transform.rotation = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);
        }
        else
        {
            Debug.LogError("TMP_InputField component is missing from this GameObject.");
        }
    }
    private void hidetextbox()
    {
        GameObject inputField = GameObject.Find("InputField (TMP)");
        inputField.gameObject.SetActive(false);
    }
    private void showtextbox()
    {
        GameObject inputField = GameObject.Find("InputField (TMP)");
        inputField.SetActive(true);
    }


    private void showspecificatom(string name)
    {
        Transform atoms = protein.transform.Find("Atoms");
        atoms.gameObject.SetActive(true);
        foreach (Transform child in atoms.transform)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                if ((child.name == name))
                {
                    renderer.material = defaultMaterial;
                    renderer.material.color = ElementColor.AssignColor(name);
                }
                else
                {
                    renderer.material =transmaterial;
                }
            }
        }
    }
    private void MoveKeyboardForward()
    {
        Transform cameraTransform = Camera.main.transform;
        float distanceBack = 0.05f;
        GameObject keyboard = GameObject.Find("NoneNativeKeyboard");
        keyboard.transform.position -= cameraTransform.forward * distanceBack;
        keyboard.transform.rotation = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);
        keyboard.transform.SetParent(cameraTransform);
    }

    private void MoveKeyboardback()
    {
        Transform cameraTransform = Camera.main.transform;
        float distanceBack = -0.05f;
        GameObject keyboard = GameObject.Find("NoneNativeKeyboard");
        keyboard.transform.position -= cameraTransform.forward * distanceBack;
        keyboard.transform.rotation = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);
        keyboard.transform.SetParent(cameraTransform);
    }

    private void toggleList()
    {
        Transform commandList = Camera.main.transform.Find("Commands List");
        commandList.gameObject.SetActive(!commandListActive);
        commandListActive = !commandListActive;
    }

    private void hideAtoms()
    {
        Transform childTransform = protein.transform.Find("Atoms");
        childTransform.gameObject.SetActive(false);
    }
    private void showAtoms()
    {
        Transform childTransform = protein.transform.Find("Atoms");
        childTransform.gameObject.SetActive(true);
    }
    private void showlinks()
    {
        Transform childTransform = protein.transform.Find("Links");
        childTransform.gameObject.SetActive(true);
    }
    private void hidelinks()
    {
        Transform childTransform = protein.transform.Find("Links");
        childTransform.gameObject.SetActive(false);
    }
    private void hidehelices()
    {
        Transform childTransform = protein.transform.Find("Helices");
        childTransform.gameObject.SetActive(false);
    }
    private void showhelices()
    {
        Transform childTransform = protein.transform.Find("Helices");
        childTransform.gameObject.SetActive(true);
       
    }

    private void showrna()
    {
        Transform childTransform1 = protein.transform.Find("RNA Helices");
        childTransform1.gameObject.SetActive(true);
    }

    private void hiderna()
    {
        Transform childTransform1 = protein.transform.Find("RNA Helices");
        childTransform1.gameObject.SetActive(false);
    }
    private void hidesheets()
    {
        Transform childTransform = protein.transform.Find("Sheets");
        childTransform.gameObject.SetActive(false);
    }
    private void showsheets()
    {
        Transform childTransform = protein.transform.Find("Sheets");
        childTransform.gameObject.SetActive(true);
    }
    private void hideconnects()
    {
        Transform childTransform = protein.transform.Find("Connects");
        childTransform.gameObject.SetActive(false);
    }
    private void showconnects()
    {
        Transform childTransform = protein.transform.Find("Connects");
        childTransform.gameObject.SetActive(true);
    }
    private void showeverything()
    {
        Transform childTransform1 = protein.transform.Find("Atoms");
        Transform childTransform2 = protein.transform.Find("Helices");
        Transform childTransform3 = protein.transform.Find("Sheets");
        Transform childTransform4 = protein.transform.Find("Connects");
        Transform childTransform5 = protein.transform.Find("RNA Helices");
        Transform childTransform6 = protein.transform.Find("Links");
        childTransform1.gameObject.SetActive(true);
        childTransform2.gameObject.SetActive(true);
        childTransform3.gameObject.SetActive(true);
        childTransform4.gameObject.SetActive(true);
        childTransform5.gameObject.SetActive(true);
        childTransform6.gameObject.SetActive(true);
    }

    private void hideeverything()
    {
        Transform childTransform1 = protein.transform.Find("Atoms");
        Transform childTransform2 = protein.transform.Find("Helices");
        Transform childTransform3 = protein.transform.Find("Sheets");
        Transform childTransform4 = protein.transform.Find("Connects");
        Transform childTransform5 = protein.transform.Find("RNA Helices");
        Transform childTransform6 = protein.transform.Find("Links");
        childTransform1.gameObject.SetActive(false);
        childTransform2.gameObject.SetActive(false);
        childTransform3.gameObject.SetActive(false);
        childTransform4.gameObject.SetActive(false);
        childTransform5.gameObject.SetActive(false);
        childTransform6.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
    }
    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);
    }
}