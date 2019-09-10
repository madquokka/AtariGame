using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class PlayerMove : Agent
{

    public float moveSpeed = 5f;
    public Transform ball;
    public float shootSpeed = 7f;
    public List<GameObject>blocks;
    public Text gResult;
    public int blockCnt;

    Rigidbody rb;
    Vector3 originPlayer;
    Vector3 originBall;
    bool isPlay = false;


public override void InitializeAgent()
{
        rb = ball.GetComponent<Rigidbody>();

        //블록 수 저장하기
        blockCnt = blocks.Count;

        //초기 위치저장하기
        originPlayer = transform.localPosition;
        originBall = ball.localPosition;

        gResult.gameObject.SetActive(false);

        Invoke("ShootBall",1f);

    }

    private void ShootBall()
    {

        //공을 60도 ~ 120도 사이에서 랜덤하게 발사
        float degree = Random.Range(60f, 120f);
        Vector3 shootDir = new Vector3(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad), 0);
        rb.velocity = shootDir * shootSpeed;
        isPlay = true;
    }

    public override void CollectObservations()
    {
        // 1. 나의위치
        AddVectorObs(transform.position);

        // 2. 공의 위치
        AddVectorObs(ball.position);

        // 3. 공의 속도(크기,방향)
        AddVectorObs(rb.velocity);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        CheckBlocks();
        AddReward(0.01f);

        // 플레이어 좌우이동
        float h = Mathf.Clamp(vectorAction[0], -1f, 1f);

        Vector3 dir = new Vector3(h, 0, 0).normalized;

        transform.position += dir * moveSpeed * Time.deltaTime;

        //이동한계 -5.0f ~ 5.0f

        Vector3 myPos = transform.localPosition;
        myPos.x = Mathf.Clamp(myPos.x, -5.6f, 5.5f);
        transform.localPosition = myPos;
    }

   

    //게임클리어 또는 out을 체크하는 함
    void CheckBlocks()
    {
        if (!isPlay)
        {
            return;
        }

        //게임클리어
        if (blockCnt <= 0)
        {
            isPlay = false;
            CheckResult("Game Clear");
            AddReward(2f);
        }

        //게임오버
        else if (ball.position.y < transform.position.y)
        {
            isPlay = false;
            CheckResult("Game Over");
            AddReward(-3f);
        }
    }

    private void CheckResult(string result)
    {
        //UI 활성화
        gResult.gameObject.SetActive(true);

        //UI에 result 문자열 출력
        gResult.text = result;

        //1.5초 뒤에 모든 것을 원위치 시킨다.
        Invoke("Done", 1.5f);

        //리셋할때 AgentReset 함수를 실행시키면서 , 외부 Tensorflow에게도 리셋 되었다는 사건과 상벌점 결과를 전달하려함 
        //Done(); <- 텐서플로우에 전달하기 위해 "" 안에 직접 리셋 넣으면 안
    }

public override void AgentReset()
{
        //블록 원위치

        foreach(GameObject block in blocks)
        {
            block.SetActive(true);
        }

        //플레이어 원위치, 공으위치 원위치
        transform.localPosition = originPlayer;
        ball.localPosition = originBall;
        rb.velocity = Vector3.zero;
        

        //UI 비활성화
        gResult.gameObject.SetActive(false);

        //블록 수 추가
        blockCnt = blocks.Count;

        //공 재발사
        Invoke("ShootBall", 1f);

    }
}
