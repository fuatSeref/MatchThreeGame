using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingButton : MonoBehaviour
{
   [SerializeField] private GameObject settingPanel;

   public bool isOpen;
   public void ToggleSetting()
   {
      isOpen = !isOpen;
      settingPanel.SetActive(isOpen);
   }
}
