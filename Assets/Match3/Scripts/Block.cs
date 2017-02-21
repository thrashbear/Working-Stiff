using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Block:MonoBehaviour {
    //позиции блока 
    public int ID;
    public int x;
    public int y;

    private Vector3 myScale;
    private float startTime;

    public static Transform select;
    public static Transform moveTo;
    void Start() {
        myScale = transform.localScale;
        startTime = Time.time;
    }

    void Update() {
        //эффект роста 
        if(Time.time - startTime < 3) {//после спауна блока, расти он будет не более 3 секунд 
            transform.localScale = Vector3.Lerp(new Vector3(0, 0, 0), myScale, (Time.time - startTime) / 2);
        }
    }

    void OnMouseOver() {
        // курсор мыши над блоком 
        transform.localScale = new Vector3(myScale.x + 0.2f, myScale.y + 0.2f, myScale.z + 0.2f);
        //выделение объектов 
        if(Input.GetMouseButtonDown(0)) {
            if(!select) {//если ни один не выделен, выделяем данный блок 
                select = transform;
            } else if(select != transform && !moveTo) { //если один блок уже выделен, проверим не выделен ли он снова. Выделяем второй объект 
                moveTo = transform;
            }
        }
    }

    void OnMouseExit() {
        //сбрасываем скейл блока 
        transform.localScale = myScale;
    }
}

