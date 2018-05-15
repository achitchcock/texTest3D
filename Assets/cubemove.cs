using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cubemove : MonoBehaviour {

    public Button startBtn;
    public Button moveBtn;
    public Button showToast;
    public Button startDrone;
    public GameObject cube;
    public GameObject cubeHolder;
    public GameObject display;
    public Text status;
    public Text connected;
    public Text fcStatus;
    private bool spin;
    private bool move;
    private bool away = true;
    private Texture2D tex2d;

    // colors
    private ColorBlock on;
    private ColorBlock off;

    // Use this for initialization
    void Start () {
        move = false;
        startBtn.onClick.AddListener(spinToggle);
        moveBtn.onClick.AddListener(moveToggle);
        startDrone.onClick.AddListener(startDroneFunc);
        showToast.onClick.AddListener(toast);
        on = startBtn.colors;
        moveBtn.colors = on;
        off = startBtn.colors;
        off.normalColor = new Color(0.8f, 0f, 0f, 1f);
        off.highlightedColor = new Color(0.8f, 0f, 0f, 1f);
        off.pressedColor = new Color(0.8f, 0f, 0f, 1f);
        tex2d = new Texture2D(960, 720);
    }

    void spinToggle()
    {
        if(spin)
        {
            spin = false;
            startBtn.colors = on;
            startBtn.GetComponentInChildren<Text>().text = "START";

        }
        else
        {
            spin = true;
            startBtn.colors = off;
            startBtn.GetComponentInChildren<Text>().text = "STOP";
        }
    }
	
    void moveToggle()
    {
        if (move)
        {
            move = false;
            moveBtn.colors = on;
        }
        else
        {
            move = true;
            moveBtn.colors = off;
        }
    }


    void toast()
    {
        using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                obj_Activity.Call("showToast", "Button Clicked in Unity");
            }
        }
    }

    void updateText()
    {
        using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                obj_Activity.Call("refreshSDKRelativeUI");
                obj_Activity.Call("flightControllerStatus");
                status.text = obj_Activity.Call<string>("getConnectionStatus");
                connected.text = obj_Activity.Call<string>("getProductText");
                fcStatus.text = obj_Activity.Call<string>("getState");
            }
        }
    }

    void startDroneFunc()
    {
        using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                obj_Activity.Call("setupDroneConnection");
            }
        }
    }

    void updateDisplay()
    {
        using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                
                byte[] t = obj_Activity.Call<byte[]>("getVid");
                if (null != t)
                {
                    tex2d.LoadImage(t);
                    tex2d.Apply();
                    display.GetComponent<Renderer>().material.mainTexture = tex2d;
                }
                
            }
        }
    }


	// Update is called once per frame
	void Update () {
        if (spin)
        {
            //cube.transform.Translate(new Vector3(0, 0.01f, 0));
            cube.transform.Rotate(new Vector3(0, 0.3f, 0.5f));
            updateText();
        }
        if (move)
        {
            updateDisplay();
            if (away)
            {
                cubeHolder.transform.Translate(new Vector3(0,0,0.5f));
                if(cubeHolder.transform.position.z > 30)
                {
                    away = false;
                }
            }
            else
            {
                cubeHolder.transform.Translate(new Vector3(0, 0, -0.5f));
                if (cubeHolder.transform.position.z < -16)
                {
                    away = true;
                }
            }
        }

        
	}
}
