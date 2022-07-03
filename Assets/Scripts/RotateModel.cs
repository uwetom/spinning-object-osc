using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using extOSC;

public class RotateModel : MonoBehaviour
{	
	public GameObject rotationObject;

	public Material transparentMaterial;
	public Material opaqueMaterial;

    public float opacity = 1;
    public Slider opacitySlider;

    public float xVal = 0;
    public Slider xSlider;

    public float yVal = 0;
    public Slider ySlider;

    public float zVal = 0;
    public Slider zSlider;

    public float speed = 0;

    public List<float> previousAngles;

    private Quaternion previousAngle;

    private OSCReceiver _receiver;

    private float xRotVal = 0;
    private float yRotVal = 0;

    // Start is called before the first frame update
    void Start()
    {
        // opacitySlider.onValueChanged.AddListener(delegate {OpacityValueChangeCheck(); });
        // xSlider.onValueChanged.AddListener(delegate {rotationValueChangeCheck(); });
        // ySlider.onValueChanged.AddListener(delegate {rotationValueChangeCheck(); });
       //  zSlider.onValueChanged.AddListener(delegate {rotationValueChangeCheck(); });
		
		previousAngle = Quaternion.Euler(0,0,0);

		previousAngles = new List<float>();

        _receiver = gameObject.GetComponent<OSCReceiver>();

        _receiver.Bind("/pos/", MessageReceived);
    }

    void Update(){

    	 Quaternion current = Quaternion.Euler(yRotVal, xRotVal,0);

    	 float angle = Quaternion.Angle(previousAngle, current);

        
        previousAngle = current;


		previousAngles.Add(angle);


		//only store last 10 angles
		if(previousAngles.Count >= 20){
			previousAngles.RemoveAt(0);
		}

		//find average
		float total = 0;
		for(int i = 0; i< previousAngles.Count; i++){
			total += previousAngles[i];
		}
		float average = total/previousAngles.Count;
		Debug.Log(average);

		//if(average <= 0.1){
		//	rotationObject.GetComponent<Renderer>().material = opaqueMaterial;
		//	rotationObject.GetComponent<Renderer>().material.color = new Color(1.0f,1.0f,1.0f);
 		
		//}else{
			rotationObject.GetComponent<Renderer>().material = transparentMaterial;
 			rotationObject.GetComponent<Renderer>().material.color = new Color(1.0f,1.0f,1.0f, 1.0f-(average/10.0f));
 		//}
 		// = new Color(255f,255f,255f,1-(average/20));


		//rotationObject.GetComponent<Renderer>().material.SetFloat("_Opacity", 1-(average/20));


          // Quaternion target = Quaternion.Euler(xSlider.value, ySlider.value, zSlider.value);

        transform.rotation = current;
    }

    public void OpacityValueChangeCheck()
    {
     
 	//	rotationObject.GetComponent<Renderer>().material.color.a = opacitySlider.value;
   


      

       // rotationObject.GetComponent<Renderer>().material.SetFloat("_Opacity",opacitySlider.value);
   

    }

    public void rotationValueChangeCheck()
    {
       

        //GetComponent<Renderer>().material.SetFloat("_Opacity",opacitySlider.value);

          Quaternion target = Quaternion.Euler(xSlider.value, ySlider.value, zSlider.value);

        // Dampen towards the target rotation
        //  float smooth = 5.0f;
      
       // Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * smooth);


       
    }


    // MessageReceived implementation
    protected void MessageReceived(OSCMessage message)
    {
        xRotVal = (float)message.Values[0].DoubleValue + 90;
        yRotVal = (float)message.Values[1].DoubleValue + 90;
        //Debug.Log("xRotVal");
        //Debug.Log(xRotVal);
    }

}
