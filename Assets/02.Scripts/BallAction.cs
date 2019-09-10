using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAction : MonoBehaviour
{
    public Transform player;
    PlayerMove pm;

    private void Start()
    {
       pm = player.GetComponent<PlayerMove>();
    }

    //부딪힌 대상의 태그가 Block이면 비활성화 시킴
    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Block")
        {
            pm.blockCnt -=10;
            col.gameObject.SetActive(false);
        }
    }
}
