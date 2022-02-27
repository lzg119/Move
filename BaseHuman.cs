using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour{
    protected bool ismoving = false;
    private Vector3 targetPosition;
    public float speed = 1.2f;
    private Animator animator;
    public string decs = "";
    public int hp = -1;
    // Start is called before the first frame update
    protected void Start(){
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected void Update(){
        MoveUpdate();
    }
    public void MoveTo(Vector3 pos){
        targetPosition = pos;
        ismoving = true;
        animator.SetBool("isMoving", true);
    }
    public void MoveUpdate(){
        if(ismoving == false)
            return;

        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, targetPosition, speed * Time.deltaTime);
        transform.LookAt(targetPosition);
        if(Vector3.Distance(pos, targetPosition) < 0.05f){
            ismoving = false;
            animator.SetBool("isMoving", false);
        }
    }
}
