using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIElement : MonoBehaviour
{
    [SerializeField] private GameObject PopUpUI;
    [SerializeField] private TextMeshProUGUI TextElement;
    [SerializeField] private ElementType SpecialType;

   private void Awake()
    {
        if(SpecialType == ElementType.VersionText)
        {
            TextElement.text = Application.version;
        }
    }



    public void SelectStage(string SceneName)
    {
       SceneManager.LoadScene(SceneName);
    }

    public void ToggleUI()
    {
        if (PopUpUI.activeSelf)
        {
            PopUpUI.SetActive(false);
        }
        else
        {
            PopUpUI.SetActive(true);
        }
    }


    public enum ElementType
    {
        None,
        VersionText,


    }



}
