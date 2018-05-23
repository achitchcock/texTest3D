using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cubemove : MonoBehaviour {

    public Button startBtn;
    public Button moveBtn;
    public Button showToast;
    public Button startDrone;
    public Button takeOffBtn;
    public Button landBtn;
    public Button enableCam;
    //public Button startYawBtn;
    //public Button stopYawBtn;
    //public GameObject cube;
    //public GameObject cubeHolder;
    public GameObject display;
    public GameObject display2;
    public Text status;
    public Text connected;
    public Text fcStatus;
    private bool spin;
    private bool move;
    private bool away = true;
    private bool droneCam;
    private Texture2D tex2d;
    private WebCamTexture webTex;
    private Vector3 defaultPos;
    private Vector3 defaultScale;
    private Vector3 viewcale;
    private Vector3 leftView;
    private Vector3 rightView;

    // colors
    private ColorBlock on;
    private ColorBlock off;

    // Use this for initialization
    void Start () {
        move = false;
        droneCam = false;
        defaultPos = new Vector3(-236,-283,0);
        defaultScale = new Vector3(926,768,1);
        viewcale = new Vector3(1270,1050,1);
        leftView = new Vector3(-640,0,0);
        rightView = new Vector3(640,0,0);
        startBtn.onClick.AddListener(spinToggle);
        moveBtn.onClick.AddListener(moveToggle);
        startDrone.onClick.AddListener(startDroneFunc);
        showToast.onClick.AddListener(toast);
        takeOffBtn.onClick.AddListener(takeOff);
        landBtn.onClick.AddListener(land);
        enableCam.onClick.AddListener(swapCam);
        //startYawBtn.onClick.AddListener(StartYaw);
        //stopYawBtn.onClick.AddListener(stopYaw);
        on = startBtn.colors;
        moveBtn.colors = on;
        off = startBtn.colors;
        off.normalColor = new Color(0.8f, 0f, 0f, 1f);
        off.highlightedColor = new Color(0.8f, 0f, 0f, 1f);
        off.pressedColor = new Color(0.8f, 0f, 0f, 1f);
        tex2d = new Texture2D(960, 720);
        webTex = new WebCamTexture(500, 500, 5);
        webTex.Stop();
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
            display.transform.localPosition = defaultPos;
            display2.transform.localPosition = defaultPos;
            display.transform.localScale = defaultScale;
            display2.transform.localScale = defaultScale;
        }
        else
        {
            move = true;
            moveBtn.colors = off;
            display.transform.localPosition = leftView;
            display2.transform.localPosition = rightView;
            display.transform.localScale = viewcale;
            display2.transform.localScale = viewcale;
        }
    }

    void swapCam()
    {
        if(droneCam == true)
        {
            droneCam = false;
            webTex.Play();
            enableCam.colors = off;
        }
        else
        {
            droneCam = true;
            webTex.Stop();
            enableCam.colors = on;

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
        if (droneCam == true)
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
                        display2.GetComponent<Renderer>().material.mainTexture = tex2d;
                    }

                }
            }
        }
        else
        {
            if(webTex.isPlaying == false) { webTex.Play(); }
            display.GetComponent<Renderer>().material.mainTexture = webTex;
            display2.GetComponent<Renderer>().material.mainTexture = webTex;
        }
    }

    void takeOff()
    {
        using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                obj_Activity.Call("takeOff");
            }
        }
    }

    void land()
    {
        using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                obj_Activity.Call("land");
            }
        }
    }

    void StartYaw()
    {
        using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                obj_Activity.Call("yawSome");
            }
        }
    }

    void stopYaw()
    {
        using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                obj_Activity.Call("stopYaw");
            }
        }
    }



	// Update is called once per frame
	void Update () {
        if (spin)
        {
            //cube.transform.Translate(new Vector3(0, 0.01f, 0));
            //cube.transform.Rotate(new Vector3(0, 0.3f, 0.5f));
            updateText();
        }
        if (move)
        {
            updateDisplay();
            /*if (away)
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
            }*/
        }

        
	}
}
