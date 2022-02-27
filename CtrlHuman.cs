using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlHuman : BaseHuman {
    // Start is called before the first frame update
	new void Start () {
		base.Start();
	}

    // Update is called once per frame
    new void Update(){
        base.Update();
        if(Input.GetMouseButtonDown(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            Debug.Log(hit.collider.tag);
            if(hit.collider.tag == "Terrain"){
                MoveTo(hit.point);
                string sendStr="Move|";
                sendStr += NetManager.GetDesc() +  ",";
                sendStr += hit.point.x.ToString() + ",";
                sendStr += hit.point.y.ToString() + ",";
                sendStr += hit.point.z.ToString() + ",";
                NetManager.Send(sendStr);
            }
        }
    }

}
