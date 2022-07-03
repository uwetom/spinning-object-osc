using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class ControllerScript : MonoBehaviour
{
   private OSCReceiver _receiver;

    // Start is called before the first frame update
    void Start()
    {
        _receiver = gameObject.GetComponent<OSCReceiver>();

        _receiver.Bind("/pos/", MessageReceived);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // MessageReceived implementation
    protected void MessageReceived(OSCMessage message)
    {
        Debug.Log(message.Values[0].DoubleValue);

        double xRot = message.Values[0].DoubleValue;
        double yRot = message.Values[1].DoubleValue;



    }

}
