using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour{
    public GameObject humanPrefab;
    public BaseHuman myHuman;
    public Dictionary<string, BaseHuman> otherHuman  = new Dictionary<string, BaseHuman>();
    public Text debug_info; 
    // Start is called before the first frame update
    void Start(){
        debug_info =GameObject.Find("Debug_info").GetComponent<Text>();
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.Connect("127.0.0.1", 8890);
        InitHuman();
        NetManager.Send("List|");
    }

    // Update is called once per frame
    void Update(){
        NetManager.Update();
        debug_info.text = NetManager.debug_info;
    }
    void InitHuman(){
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        float x = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);
        obj.transform.position = new Vector3(x, 0, z);
        myHuman = obj.AddComponent<CtrlHuman>();
        myHuman.decs = NetManager.GetDesc();

        string sendStr = "Enter|";
        sendStr += NetManager.GetDesc() + ",";
        sendStr += obj.transform.position.x + ",";
        sendStr += obj.transform.position.y + ",";
        sendStr += obj.transform.position.z + ",";
        sendStr += obj.transform.eulerAngles.y + ",";
        NetManager.Send(sendStr);
    }
    void addOtherHuman(string decs, float x, float y, float z, float eulY, int hp){
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        obj.transform.position = new Vector3(x, y, z);
        obj.transform.eulerAngles = new Vector3(0, eulY, 0);
        BaseHuman h = obj.AddComponent<SyncHuman>();
        h.decs = decs;
        h.hp = hp;
        otherHuman.Add(decs, h);
    }
    public void OnEnter(string msgArgs){
        Debug.Log("OnEnter " + msgArgs);
        string[] split = msgArgs.Split(',');
        string decs = split[0];

        if(decs == NetManager.GetDesc())
            return;
        
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulY = float.Parse(split[4]);
        addOtherHuman(decs, x, y, z, eulY, 100);
    }
    public void OnList(string msgArgs){
        Debug.Log("OnList" + msgArgs);
        string[] split = msgArgs.Split(',');
        int char_num = (split.Length-1)/6;
        for(int i = 0; i < char_num; i++){
            string decs = split[i * 6 + 0];

            if(decs == NetManager.GetDesc())
                continue;

            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float eulY = float.Parse(split[i * 6 + 4]);
            int hp = int.Parse(split[i * 6 + 5]);
            addOtherHuman(decs, x, y, z, eulY, hp);
        }
    }
    public void OnMove(string msgArgs){
        Debug.Log("OnMove " + msgArgs);
        string[] split = msgArgs.Split(',');
        string decs = split[0];

        if(!otherHuman.ContainsKey(decs))
            return;
        
        BaseHuman h = otherHuman[decs];
        Vector3 targetPos = new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
        h.MoveTo(targetPos);
    }

     public void OnLeave(string msgArgs){
        Debug.Log("OnLeave " + msgArgs);
    }
}
