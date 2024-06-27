using UnityEngine;
using UnityEngine.Events;
using Winch.Core;

namespace DredgeHardMode
{
    [AddToMainMenuScene]
    internal class MainMenuModifier : MonoBehaviour
    {
        //UnityAction action;

        public void Start()
        {
            /*try
            {
                GameObject slotList = GameObject.Find("Canvases/MenuCanvas/SaveSlotWindow/Container/Panel/SlotList");

                Transform child;
                /*foreach (Transform tranform in slotList.transform)
                {
                    child = tranform.gameObject;
                    WinchCore.Log.Error(child.GetComponent<SaveSlotUI>().hasSaveFile);
                    if (child.GetComponent<SaveSlotUI>()._HasSaveFile) continue;
                    GameObject button = Instantiate(child.transform.GetChild(2)).gameObject;

                    button.transform.parent = child.transform;
                    button.transform.position = tranform.GetChild(3).position;

                    BasicButton basicButton = button.GetComponent<BasicButton>();
                    action += OnClick;
                    basicButton.onClick.AddListener(action);
                }

                for(int i = 0; i < slotList.transform.childCount; i++)
                {
                    child = slotList.transform.GetChild(i);
                    WinchCore.Log.Error(child.name);

                    GameObject button = Instantiate(child.GetChild(2)).gameObject;
                    button.SetActive(true);
                    button.transform.parent = child;
                    button.transform.position = child.GetChild(3).position;
                    button.layer = 999;

                    BasicButton basicButton = button.GetComponent<BasicButton>();
                    action += OnClick;
                    basicButton.onClick.AddListener(action);

                    DontDestroyOnLoad(button);
                }
            }
            catch (Exception ex)
            {
                WinchCore.Log.Error(ex);
            }*/
        }

        public void OnClick()
        {
            WinchCore.Log.Error("Pressing button");
        }
    }
}
