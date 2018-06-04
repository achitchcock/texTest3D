using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cameraPIP : MonoBehaviour
{
    public Button startBtn;
    public Button Drone_video_btn;
    public Button Phone_video_btn;
    public Button showToast;
    public Button startDrone;
    public Button takeOffBtn;
    public Button landBtn;
    public Button enableCam;
    //public Button startYawBtn;
    //public Button stopYawBtn;
    public GameObject left_display;
    public GameObject right_display;
    public GameObject left_pip_display;
    public GameObject right_pip_display;
    public Text connection_status;
    public Text connected_hardware;
    public Text flight_controller_status;
    public Text fps_display;
    private float lastFrame;
    private bool update_status_flag;
    private bool update_display_flag;
    private bool frame_ready_flag;
    private bool drone_camera_flag;
    private bool phone_camera_flag;

    // Drone Camera
    private Texture2D tex2d;

    // phoneCam Camera
    private WebCamTexture webTex;

    public Texture noVideo;

    // Display positions
    private Vector3 defaultPos;
    private Vector3 defaultScale;
    private Vector3 viewcale;
    private Vector3 leftView;
    private Vector3 rightView;

    // colors
    private ColorBlock on;
    private ColorBlock off;

    // Use this for initialization
    void Start()
    {
        update_display_flag = true;
        update_status_flag = false;
        frame_ready_flag = false;
        drone_camera_flag = false;
        phone_camera_flag = false;
        defaultPos = new Vector3(-236, -283, 0);
        defaultScale = new Vector3(926, 768, 1);
        viewcale = new Vector3(1270, 1050, 1);
        leftView = new Vector3(-640, 0, 0);
        rightView = new Vector3(640, 0, 0);
        startBtn.onClick.AddListener(update_status_toggle);
        Drone_video_btn.onClick.AddListener(toggle_drone_cam);
        Phone_video_btn.onClick.AddListener(toggle_phone_cam);
        startDrone.onClick.AddListener(startDroneFunc);
        showToast.onClick.AddListener(toast);
        takeOffBtn.onClick.AddListener(takeOff);
        landBtn.onClick.AddListener(land);
        lastFrame = Time.time;
        on = Drone_video_btn.colors;
        off = startBtn.colors;
        off.normalColor = new Color(0.8f, 0f, 0f, 1f);
        off.highlightedColor = new Color(0.8f, 0f, 0f, 1f);
        off.pressedColor = new Color(0.8f, 0f, 0f, 1f);
        tex2d = new Texture2D(960, 720);
        webTex = new WebCamTexture(500, 500, 5);
        webTex.Stop(); // just to be safe
    }

    void update_status_toggle()
    {
        if (update_status_flag)
        {
            update_status_flag = false;
            startBtn.colors = on;
            startBtn.GetComponentInChildren<Text>().text = "START";
        }
        else
        {
            update_status_flag = true;
            startBtn.colors = off;
            startBtn.GetComponentInChildren<Text>().text = "STOP";
        }
    }

    void toggle_drone_cam()
    {
        if (drone_camera_flag)
        {
            drone_camera_flag = false;
            Drone_video_btn.colors = off;
        }
        else
        {
            drone_camera_flag = true;
            Drone_video_btn.colors = on;
        }
    }

    void toggle_phone_cam()
    {
        if (phone_camera_flag)
        {
            phone_camera_flag = false;
            Phone_video_btn.colors = on;
        }
        else
        {
            phone_camera_flag = true;
            Phone_video_btn.colors = off;
        }
    }

    void set_frame_ready(string message)
    {
        if (message == "true")
        {
            frame_ready_flag = true;
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
                connection_status.text = obj_Activity.Call<string>("getConnectionStatus");
                connected_hardware.text = obj_Activity.Call<string>("getProductText");
                flight_controller_status.text = obj_Activity.Call<string>("getState");
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
        if (drone_camera_flag )
        {
            if (frame_ready_flag)
            {
                frame_ready_flag = false;
                using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {

                        byte[] t = obj_Activity.Call<byte[]>("getVid");
                        if (null != t)
                        {
                            tex2d.LoadImage(t);
                            tex2d.Apply();
                            left_display.GetComponent<Renderer>().material.mainTexture = tex2d;
                            right_display.GetComponent<Renderer>().material.mainTexture = tex2d;
                            fps_display.text = "" + Math.Round(1 / (Time.time - lastFrame), 2) + "fps";
                            lastFrame = Time.time;
                        }

                    }
                }
            }
        }
        else
        {
            left_display.GetComponent<Renderer>().material.mainTexture = noVideo;
            right_display.GetComponent<Renderer>().material.mainTexture = noVideo;
        }
        if ( phone_camera_flag )
        {
            if (webTex.isPlaying == false) { webTex.Play(); }
            left_pip_display.GetComponent<Renderer>().material.mainTexture = webTex;
            right_pip_display.GetComponent<Renderer>().material.mainTexture = webTex;
        }
        else
        {
            webTex.Stop();
            left_pip_display.GetComponent<Renderer>().material.mainTexture = noVideo;
            right_pip_display.GetComponent<Renderer>().material.mainTexture = noVideo;
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
    void Update()
    {
        if (update_status_flag)
        {
            updateText();
        }
        if (update_display_flag)
        {
            updateDisplay();
        }
    }
}
