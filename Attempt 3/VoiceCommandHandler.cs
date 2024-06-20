using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEditor;
using UnityEngine;
public class VoiceCommandHandler : MonoBehaviour, IMixedRealitySpeechHandler
{
    GameObject protein; // The object to change color
    Material defaultMaterial;
    Material transmaterial;

    void Start()
    {
        protein = gameObject;
        defaultMaterial = protein.transform.Find("Atoms").transform.GetChild(0).GetComponent<Renderer>().material;
        transmaterial = Resources.Load("Transmaterial", typeof(Material)) as Material;
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
 
        switch (eventData.Command.Keyword.ToLower())
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
                showcarbon();
                break;
            case "show oxygen":
                showoxygen();
                break;
            case "show nitrogen":
                shownitrogen();
                break;
            case "show hydrogen":
                showhydrogen();
                break;
            case "show sulfer":
                showsulfer();
                break;
            case "show phosphorous":
                showphosphorous();
                break;
            case "show bromine":
                showbromine();
                break;
            case "show colors":
                showcolors();
                break;
            case "show zinc":
                showzync();
                break;
            case "show chlorine":
                showchlorine();
                break;
            default:
                Debug.Log(eventData.Command.Keyword.ToLower());
                break;
        }
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
                string tag = child.tag;
                renderer.material.color = ElementColor.AssignColor(tag);
            }
        }
    }
    private void showcarbon()
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
                string tag = child.tag;
                if (child.tag == "C") {
                    renderer.material = defaultMaterial;
                    renderer.material.color = ElementColor.AssignColor(tag);
                }
                else {
                    renderer.material = transmaterial;
                }
            }
        }
    }

    private void showbromine()
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
                string tag = child.tag;
                if (child.tag == "BR")
                {
                    renderer.material = defaultMaterial;
                    renderer.material.color = ElementColor.AssignColor(tag);
                }
                else
                {
                    renderer.material = transmaterial;
                }
            }
        }
    }

    private void showoxygen()
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
                string tag = child.tag;
                if (child.tag == "O")
                {
                    renderer.material = defaultMaterial;
                    renderer.material.color = ElementColor.AssignColor(tag);
                }
                else
                {
                    renderer.material = transmaterial;
                }
            }
        }
    }
    private void shownitrogen()
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
                string tag = child.tag;
                if (child.tag == "N")
                {
                    renderer.material = defaultMaterial;
                    renderer.material.color = ElementColor.AssignColor(tag);
                }
                else
                {
                    renderer.material = transmaterial;
                }
            }
        }
    }

    private void showhydrogen()
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
                string tag = child.tag;
                if (child.tag == "H")
                {
                    renderer.material = defaultMaterial;
                    renderer.material.color = ElementColor.AssignColor(tag);
                }
                else
                {
                    renderer.material = transmaterial;
                }
            }
        }
    }

    private void showsulfer()
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
                string tag = child.tag;
                if (child.tag == "S")
                {
                    renderer.material = defaultMaterial;
                    renderer.material.color = ElementColor.AssignColor(tag);
                }
                else
                {
                    renderer.material = transmaterial;
                }
            }
        }
    }

    private void showphosphorous()
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
                string tag = child.tag;
                if (child.tag == "P")
                {
                    renderer.material = defaultMaterial;
                    renderer.material.color = ElementColor.AssignColor(tag);
                }
                else
                {
                    renderer.material = transmaterial;
                }
            }
        }
    }

    private void showzync()
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
                string tag = child.tag;
                if (child.tag == "ZN")
                {
                    renderer.material = defaultMaterial;
                    renderer.material.color = ElementColor.AssignColor(tag);
                }
                else
                {
                    renderer.material = transmaterial;
                }
            }
        }
    }

    private void showchlorine()
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
                string tag = child.tag;
                if (child.tag == "CL")
                {
                    renderer.material = defaultMaterial;
                    renderer.material.color = ElementColor.AssignColor(tag);
                }
                else
                {
                    renderer.material = transmaterial;
                }
            }
        }
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
        childTransform1.gameObject.SetActive(true);
        childTransform2.gameObject.SetActive(true);
        childTransform3.gameObject.SetActive(true);
        childTransform4.gameObject.SetActive(true);
    }

    private void hideeverything()
    {
        Transform childTransform1 = protein.transform.Find("Atoms");
        Transform childTransform2 = protein.transform.Find("Helices");
        Transform childTransform3 = protein.transform.Find("Sheets");
        Transform childTransform4 = protein.transform.Find("Connects");
        childTransform1.gameObject.SetActive(false);
        childTransform2.gameObject.SetActive(false);
        childTransform3.gameObject.SetActive(false);
        childTransform4.gameObject.SetActive(false);
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