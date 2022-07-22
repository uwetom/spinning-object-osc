using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using extOSC;

public class RotateModelOSC : MonoBehaviour
{	
	public GameObject rotationObject;

	public Material transparentMaterial;
	public Material opaqueMaterial;


    public float speed = 0;

    public List<float> previousAngles;

    private List<Quaternion> previousQuaternions;

    private Quaternion previousAngle;

    private OSCReceiver _receiver;

    private float xRotVal = 0;
    private float yRotVal = 0;
    private float zRotVal = 0;

    // Start is called before the first frame update
    void Start()
    {
     
		previousAngle = Quaternion.Euler(0,0,0);

		previousAngles = new List<float>();

        previousQuaternions = new List<Quaternion>();

        _receiver = gameObject.GetComponent<OSCReceiver>();

        _receiver.Bind("/pos/", MessageReceived);
    }

    void Update(){

    	Quaternion current = Quaternion.Euler(yRotVal,zRotVal,xRotVal);

        transform.rotation = current;
      

        if(Input.GetKeyDown(KeyCode.Escape)){
		    Application.Quit();
        }

    }

    // MessageReceived implementation
    protected void MessageReceived(OSCMessage message)
    {
        xRotVal = message.Values[0].FloatValue;
        yRotVal = message.Values[1].FloatValue;
        zRotVal = message.Values[2].FloatValue;
    }

   

}
