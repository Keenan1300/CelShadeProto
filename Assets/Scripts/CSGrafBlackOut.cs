using Unity.Cinemachine;
using UnityEngine;

public class CSGrafBlackOut : MonoBehaviour
{


    public CinemachineCamera ShowtimeCam;
    public CinemachineCamera PostCam;

    //Player Data
    public GameObject Camholder;
    public PlayerCamController Camerascript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Cam Holder Func
        ConnectToCamHolder();
    }

    // Update is called once per frame
    void Update()
    {
          
    }

    void ConnectToCamHolder()
    {
        Camholder = GameObject.FindWithTag("CamHolder");

        if (Camerascript == null && Camholder != null)
        {
            Camerascript = Camholder.GetComponent<PlayerCamController>();
            Camerascript.GraffitiCam = ShowtimeCam;
        }
        else
        {
            Debug.Log("Connect to CamHolder Failed");
        }

    }

    public void TurnOffCutScene()
    {

        if (Camerascript != null)
        {
            Camerascript.DisableSprayScene();
            Destroy(gameObject);
        }

  
    }    


    //Fun Cinematic Black Bars overlay on camera!
    public void CutsceneBars()
    {

        if (Camerascript != null)
        {
            Camerascript.CinematicBlackBars();
           
        }
    }

    public void ChangetoPreviewCamera()
    {
        Debug.Log("See your work!");

        if (Camerascript == null && Camholder != null)
        {
            Camerascript = Camholder.GetComponent<PlayerCamController>();
            Camerascript.GraffitiCam = ShowtimeCam;
        }
    }


}
