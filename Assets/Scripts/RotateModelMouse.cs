using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateModelMouse : MonoBehaviour
{	
	public GameObject rotationObject;
    public float transparencySensitivity = 10f;
    public int transparencySmoothing = 10;

    private bool transparent = false;

    public Material opaqueMaterial;
    public Material transparentMaterial;

    public float speed = 0;

    public List<float> previousQuaternionDifferences;

    private List<Quaternion> previousQuaternions;

    private Quaternion previousAngle;

    private Vector3 currentAngle;

    private List<Vector3> previousAngles;

    private Quaternion current;

    public float playbackSpeed = 1/100;
    
    private float previousTime = 0;

    private enum Mode {SLOWDOWN, SLOW, SPEEDUP, NORMAL};

    private Mode currentMode = Mode.NORMAL;

    private float nextModeChangeTime = 0;

    // Start is called before the first frame update
    void Start()
    {
		previousAngle = Quaternion.Euler(0,0,0);

		previousAngles = new List<Vector3>();

        previousQuaternions = new List<Quaternion>();

        previousQuaternionDifferences = new List<float>();

        current = Quaternion.Euler(0,0,0);

        Cursor.lockState = CursorLockMode.Locked;
         Cursor.visible = false; 

        nextModeChangeTime = Time.time + Random.Range(9,11);

        //opaqueMaterial = rotationObject.GetComponent<Renderer>().material;

    }

    void Update(){

        CheckMousePosition();

        previousTime +=  Time.deltaTime;

        if(currentMode == Mode.NORMAL){
    	    current = Quaternion.Euler(previousAngles[0].x,previousAngles[0].y,previousAngles[0].z);
            previousAngles.RemoveAt(0);
        }else if(currentMode == Mode.SLOWDOWN){
            if(previousTime > (Time.deltaTime * 2)){
                current = Quaternion.Euler(previousAngles[0].x,previousAngles[0].y,previousAngles[0].z);
                previousAngles.RemoveAt(0);
                previousTime = 0;
            }
        }else if(currentMode == Mode.SLOW){
            current = Quaternion.Euler(previousAngles[0].x,previousAngles[0].y,previousAngles[0].z);
            previousAngles.RemoveAt(0);
        }else if(currentMode == Mode.SPEEDUP){
            current = Quaternion.Euler(previousAngles[1].x,previousAngles[1].y,previousAngles[1].z);
            previousAngles.RemoveAt(0);
            previousAngles.RemoveAt(0);
        }



        transform.rotation = current;


        //store difference between current angle and last
    	float angle = Quaternion.Angle(previousAngle, current);
        previousAngle = current;

		previousQuaternionDifferences.Add(angle);
        
        if(previousQuaternionDifferences.Count >= transparencySmoothing){
			previousQuaternionDifferences.RemoveAt(0);
		}
	
        //set object transparency
        float newTransparency = calculateTransparency();
        if(newTransparency < 0.8f && !transparent){
            rotationObject.GetComponent<Renderer>().material = transparentMaterial;
            rotationObject.GetComponent<Renderer>().material.color = new Color(1.0f,1.0f,1.0f,newTransparency);
            transparent = true;
        }else if(newTransparency >= 0.8f && transparent){
            rotationObject.GetComponent<Renderer>().material = opaqueMaterial;
            transparent = false;
        }
              
        //set scale
        float newScale = 1 - (newTransparency/3);
        transform.localScale = new Vector3(newScale,newScale,newScale);
        
        //Quit if escape key pressed
        if(Input.GetKeyDown(KeyCode.Escape)){
		    Application.Quit();
        }


       // if(previousAngles.Count){

       // }


        //check if switch mode

        if(currentMode == Mode.NORMAL){
            //check to see if want to slow down
            if(Time.time > nextModeChangeTime){
                currentMode = Mode.SLOWDOWN;
               
                nextModeChangeTime = Time.time + Random.Range(9,11);
            }
        }else if(currentMode == Mode.SLOWDOWN){
            if(Time.time > nextModeChangeTime){
                currentMode = Mode.SLOW;
                nextModeChangeTime = Time.time + Random.Range(9,11);
            }
        }else if(currentMode == Mode.SLOW){
             if(Time.time > nextModeChangeTime){
                 currentMode = Mode.SPEEDUP;
             }
        }else if(currentMode == Mode.SPEEDUP){
            if(previousAngles.Count <= 1){
                currentMode = Mode.NORMAL;
                previousAngles.Clear();
                nextModeChangeTime = Time.time + Random.Range(9,11);
            }
        }

       Debug.Log(newTransparency);
    }

    // MessageReceived implementation
    protected void CheckMousePosition()
    {   

        previousAngles.Add(currentAngle);


        float mouseX = -Input.GetAxis("Mouse X") * 3;
        float mouseY = Input.GetAxis("Mouse Y") * 3;

        currentAngle.x = 0;
        currentAngle.y =  currentAngle.y + mouseX;
        currentAngle.z =  currentAngle.z + mouseY;

   
    }

    private float calculateTransparency(){
		
		//find average change of angle
		float total = 0;
		for(int i = 0; i< previousQuaternionDifferences.Count; i++){
			total += previousQuaternionDifferences[i];
		}

		float average = total/previousQuaternionDifferences.Count;
        float calTransparency = 1.0f-(average/transparencySensitivity);

        //clamp transparency between 0 and 1
        calTransparency = Mathf.Clamp(calTransparency,0f,255f);

        return calTransparency;
    }

}
