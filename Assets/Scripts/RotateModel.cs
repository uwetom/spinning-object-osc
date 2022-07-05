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

    private List<Quaternion> previousQuaternions;

    private Quaternion previousAngle;

    private OSCReceiver _receiver;

    private float xRotVal = 0;
    private float yRotVal = 0;
    private float zRotVal = 0;

    // Start is called before the first frame update
    void Start()
    {
        // opacitySlider.onValueChanged.AddListener(delegate {OpacityValueChangeCheck(); });
        // xSlider.onValueChanged.AddListener(delegate {rotationValueChangeCheck(); });
        // ySlider.onValueChanged.AddListener(delegate {rotationValueChangeCheck(); });
       //  zSlider.onValueChanged.AddListener(delegate {rotationValueChangeCheck(); });
		
		previousAngle = Quaternion.Euler(0,0,0);

		previousAngles = new List<float>();
        previousQuaternions = new List<Quaternion>();

        _receiver = gameObject.GetComponent<OSCReceiver>();

        _receiver.Bind("/pos/", MessageReceived);
    }

    void Update(){

    	 Quaternion current = Quaternion.Euler(yRotVal,-zRotVal,-xRotVal);

         previousQuaternions.Add(current);

        //only store last 10 quaternions
		if(previousQuaternions.Count >= 20){
			previousQuaternions.RemoveAt(0);
		}


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
		//Debug.Log(average);

		//if(average <= 0.1){
		//	rotationObject.GetComponent<Renderer>().material = opaqueMaterial;
		//	rotationObject.GetComponent<Renderer>().material.color = new Color(1.0f,1.0f,1.0f);
 		
		//}else{
			rotationObject.GetComponent<Renderer>().material = transparentMaterial;

            float calTransparency = 1.0f-(average/30.0f);
            calTransparency = Mathf.Clamp(calTransparency,0,1);
 			rotationObject.GetComponent<Renderer>().material.color = new Color(1.0f,1.0f,1.0f,calTransparency );
 		//}
 		// = new Color(255f,255f,255f,1-(average/20));


		//rotationObject.GetComponent<Renderer>().material.SetFloat("_Opacity", 1-(average/20));


          // Quaternion target = Quaternion.Euler(xSlider.value, ySlider.value, zSlider.value);

        transform.rotation = current;
        //Quaternion.Lerp(current,previousAngle,0.25f);


        if(Input.GetKeyDown(KeyCode.Escape)){
		    Application.Quit();
        }


    }

    public void OpacityValueChangeCheck()
    {
     
 	//	rotationObject.GetComponent<Renderer>().material.color.a = opacitySlider.value;
   


      

       // rotationObject.GetComponent<Renderer>().material.SetFloat("_Opacity",opacitySlider.value);
   

    }

    public void rotationValueChangeCheck()
    {
       

        //GetComponent<Renderer>().material.SetFloat("_Opacity",opacitySlider.value);

         // Quaternion target = Quaternion.Euler(xSlider.value, ySlider.value, zSlider.value);

        // Dampen towards the target rotation
        //  float smooth = 5.0f;
      
       // Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * smooth);


       
    }


    // MessageReceived implementation
    protected void MessageReceived(OSCMessage message)
    {
        xRotVal = message.Values[0].FloatValue;
        yRotVal = message.Values[1].FloatValue;
        zRotVal = message.Values[2].FloatValue;
    }


    private Quaternion AverageQuaternion(){

        //Global variable which holds the amount of rotations which
        //need to be averaged.
        int addAmount = previousQuaternions.Count;
        
        //Global variable which represents the additive quaternion
        Quaternion addedRotation = Quaternion.identity;
        
        //The averaged rotational value
        Quaternion averageRotation = new Quaternion(0,0,0,0);
        
        //multipleRotations is an array which holds all the quaternions
        //which need to be averaged.
       // Quaternion[] multipleRotations new Quaternion[totalAmount];
        
        //Loop through all the rotational values.
        foreach(Quaternion singleRotation in previousQuaternions){
        
            //Temporary values
            float w;
            float x;
            float y;
            float z;
        
            //Amount of separate rotational values so far
            addAmount++;
        
            float addDet = 1.0f / (float)addAmount;
            addedRotation.w += singleRotation.w;
            w = addedRotation.w * addDet;
            addedRotation.x += singleRotation.x;
            x = addedRotation.x * addDet;
            addedRotation.y += singleRotation.y;
            y = addedRotation.y * addDet;
            addedRotation.z += singleRotation.z;
            z = addedRotation.z * addDet;
        
            //Normalize. Note: experiment to see whether you
            //can skip this step.
            float D = 1.0f / (w*w + x*x + y*y + z*z);
            w *= D;
            x *= D;
            y *= D;
            z *= D;
        
            //The result is valid right away, without
            //first going through the entire array.
            averageRotation = new Quaternion(x, y, z, w);
        }

        return averageRotation;

    }

}
