using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    [Serializable]
    public class EditorData
    {
        public TextMeshProUGUI ValueText;
        public Slider slider;
    }
    
    public List<EditorData> editorDatas = new List<EditorData>();
    
    public float posX => editorDatas[0].slider.value;
    public float posY => editorDatas[1].slider.value;
    public float posZ => editorDatas[2].slider.value;
    public float rotX => editorDatas[3].slider.value;
    public float rotY => editorDatas[4].slider.value;
    public float rotZ => editorDatas[5].slider.value;


    private void Awake()
    {
        editorDatas[0].slider.value = transform.position.x;
        editorDatas[1].slider.value = transform.position.y;
        editorDatas[2].slider.value = transform.position.z;
        
        editorDatas[3].slider.value = transform.eulerAngles.x;
        editorDatas[4].slider.value = transform.eulerAngles.y;
        editorDatas[5].slider.value = transform.eulerAngles.z;
    }

    private void Update()
    {
       
        transform.position = new Vector3(posX, posY, posZ);
       
        transform.rotation =  Quaternion.Euler(new Vector3(rotX, rotY, rotZ));
    }
}
