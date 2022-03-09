using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionScript : MonoBehaviour
{
    public MonoBehaviour activeTechnique;
    public List<MonoBehaviour> techniques = new List<MonoBehaviour>();
    private int currentTechniqueIdx = 0;

    // Start is called before the first frame update
    void Start()
    {
        techniques.Add(GetComponent<SelectionScript>());
        techniques.Add(GetComponent<GoGoScript>());
        techniques.Add(GetComponent<FastGoGoScript>());
        techniques.Add(GetComponent<StretchGoGoScript>());

        foreach (var t in techniques)
        {
            t.enabled = false;
        }
        activeTechnique = techniques[currentTechniqueIdx];
        activeTechnique.enabled = true;

        Debug.Log("Active Technique: " + activeTechnique);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
     *Toggles Techniques which are listed in technique. 
     */
    public void toggleTechnique()
    {
        techniques[currentTechniqueIdx].enabled = false; // disable previous technique
        currentTechniqueIdx++;
        if (currentTechniqueIdx == techniques.Count) currentTechniqueIdx = 0;
        activeTechnique = techniques[currentTechniqueIdx];
        activeTechnique.enabled = true; // enable current technique
        Debug.Log("Toggle Technique: " + activeTechnique);
    }
}
