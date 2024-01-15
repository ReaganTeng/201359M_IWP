using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject MainPanel;
    [HideInInspector]
    public GameObject SettingsPanel;

   

    private void Awake()
    {
        MainPanel = GameObject.FindGameObjectWithTag("MainPanel");
        SettingsPanel = GameObject.FindGameObjectWithTag("SettingsPanel");

        togglePanel(SettingsPanel);

       
    }


    

    public void SendJSON(string datatosend, float data)
    {
        string stringAsJson = $"{data}";
        Debug.Log("JSON data prepared: " + stringAsJson);
        // Specify the path where you want to save the JSON file
        string path = Path.Combine(Application.dataPath, $"{datatosend}.json");
        // Write JSON to the file
        File.WriteAllText(path, stringAsJson);
        Debug.Log("SENDING JSON");



        //var req = new UpdateUserDataRequest
        //{
        //    Data = new Dictionary<string, string>
        //    {
        //        {datatosend, ((int)data).ToString() }
        //    }
        //};
        //PlayFabClientAPI.UpdateUserData(req, result => Debug.Log("Data sent success!"), OnError);
    }




    //FOR HIGH SCORE
    //void LoadHSJSON()
    //{
    //    PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnJSONHSDataReceived, OnError);
    //}
    //void OnJSONHSDataReceived(GetUserDataResult r)
    //{
    //    //for (int x = 0; x < skillboxes.Length; x++)
    //    //{
    //    //Debug.Log("received JSON data");
    //    if (!r.Data.ContainsKey("HighestScore")
    //        //&& r.Data.ContainsKey(skillboxes[x].name)
    //        )
    //    {
    //        SendJSON("HighestScore", 0);
    //        highestScore = 0;
    //    }
    //    else
    //    {
    //        highestScore = int.Parse(r.Data["HighestScore"].Value);
    //    }
    //    //}
    //}
    ////


    ////FOR TIME
    //public void LoadTimeJSON()
    //{
    //    PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnJSONTimeDataReceived, OnError);
    //}
    //void OnJSONTimeDataReceived(GetUserDataResult r)
    //{
    //    //for (int x = 0; x < skillboxes.Length; x++)
    //    //{
    //    //Debug.Log("received JSON data");
    //    if (!r.Data.ContainsKey("HighestTime")
    //        //&& r.Data.ContainsKey(skillboxes[x].name)
    //        )
    //    {
    //        SendJSON("HighestTime", 0);
    //        longestTime = 0;
    //    }
    //    else
    //    {
    //        Debug.Log("TIME LOADED");
    //        longestTime = int.Parse(r.Data["HighestTime"].Value);
    //    }
    //    //}
    //}
    //
    public void togglePanel_CloseButton(GameObject closeButtonGO)
    {
        CanvasGroup panelCG = closeButtonGO.GetComponentInParent<CanvasGroup>();
        panelCG.interactable = !panelCG.interactable;
        panelCG.blocksRaycasts = !panelCG.blocksRaycasts;
        if (panelCG.interactable
           && panelCG.blocksRaycasts)
        {
            panelCG.alpha = 1;
        }
        else
        {
            panelCG.alpha = 0;
        }

    }



    public void Update()
    {
        if (!SettingsPanel.GetComponent<CanvasGroup>().interactable
       
       )
        {
            if (!MainPanel.GetComponent<CanvasGroup>().interactable)
            {
                togglePanel(MainPanel);
            }

        }
        else
        {
            if (MainPanel.GetComponent<CanvasGroup>().interactable)
            {
                togglePanel(MainPanel);
            }
        }
    }

    public void togglePanel(GameObject panel)
    {
        //Debug.Log("TOGGLING PANEL");
        CanvasGroup panelCG = panel.GetComponent<CanvasGroup>();
        panelCG.interactable = !panelCG.interactable;
        panelCG.blocksRaycasts = !panelCG.blocksRaycasts;
        if (panelCG.interactable
           && panelCG.blocksRaycasts)
        {
            panelCG.alpha = 1;
        }
        else
        {
            panelCG.alpha = 0;
        }
    }




    


    //public void SendDataToJson()
    //{

    //}

    public void PlayGame()
    {
        SceneManager.LoadScene("HubWorld");
    }
}
