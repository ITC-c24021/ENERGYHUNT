using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    [SerializeField] BombScript bombScript;
    [SerializeField] PlayerController[] playerController;
    //private EnergyTank energyTank;

    [SerializeField, Header("バリアオブジェクト")]
    GameObject[] barrier;

    [SerializeField] Vector3 tagetScale;

    [SerializeField, Header("エフェクトの円")]
    GameObject circle;

    [SerializeField, Header("エフェクトの中心")]
    GameObject center;

    [SerializeField, Header("エフェクトのビリビリ1")]
    GameObject thander1;

    [SerializeField, Header("エフェクトのビリビリ2")]
    GameObject thander2;


    //爆発範囲
    [SerializeField] float explosionRange = 2;

    [SerializeField] int bomberNum;

    public float maxTime = 2f;

    [SerializeField]
    AudioSource audioSource;

    //自爆判定
    public bool isSelf = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /*public IEnumerator BomeEffect()
    {
        float timer = 0f;
        Vector3 originalScale = transform.localScale;
    
        while (timer < maxTime)
        {
            float ScaleChangeTime = timer / maxTime;
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, tagetScale * explosionRange, ScaleChangeTime);
            yield return null;
        }        
    }*/

    public void Owner(int playerNum)
    {
        bomberNum = playerNum;
    }

    private void IsSelfReset()
    {
        isSelf = false;
    }
    
    //エネルギードロップ
    private IEnumerator DropEnergry(GameObject hitPlayer)
    {
        var energyTank = hitPlayer.GetComponent<EnergyTank>();

        //死亡判定が早いとプレイヤーの死亡関数が呼ばれない為、猶予を設ける
        yield return new WaitForSeconds(0.0f);

        if (bomberNum != 0)
        {
            //放電を当てたプレイヤーの死亡判定
            Debug.Log(bomberNum - 1);
            var isDead = playerController[bomberNum - 1].isDead;

            if (isDead)//当てたプレイヤーが死んでいた時、自爆判定にする
            {
                energyTank.DropEnergy(0);
            }
            else//死んでいなかったら、当てられたプレイヤーのエネルギーを奪う
            {
                energyTank.DropEnergy(bomberNum);
            }
        }
        else
        {
            energyTank.DropEnergy(0);
        }   
     
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "P1" || other.gameObject.tag == "P2" || other.gameObject.tag == "P3" || other.gameObject.tag == "P4")
        {
            //放電SE
            audioSource.Play();

            //当たったプレイヤーの番号を取得
            var playerNum = other.GetComponent<PlayerController>().controllerNum;

            //バリアをつけていないとき
            if (!barrier[playerNum - 1].GetComponent<BarrierScript>().isBarrier)
            {
                StartCoroutine(DropEnergry(other.gameObject));

                //var energyTank = other.gameObject.GetComponent<EnergyTank>();
                /*
                //自爆判定
                if (playerNum == bomberNum)
                {
                    isSelf = true;

                    Invoke("IsSelfReset", 2.0f);
                }
                */
                /*
                //放電を当てたプレイヤーが死んでいたら
                var isDead = playerController[bomberNum - 1].isDead;

                if (isDead || isSelf)
                {
                    energyTank.DropEnergy(5);   
                }
                else
                {
                    energyTank.DropEnergy(bomberNum);
                } */
            }
        }

        if (other.gameObject.tag == "Floor")
        {
            audioSource.Play();

            //当たったブロックのスクリプトを取得
            var blockScript = other.GetComponent<BlockScript>();

            //壊れている時は通らないようにする
            if (!blockScript.isBroken)
            {
                blockScript.SetBlock(false);
            }
        }
    }

    //強化タンクによる範囲変更
    public void ChangeRange()
    {
        gameObject.transform.localScale *= 1.1f;
        circle.transform.localScale *= 1.1f;
        center.transform.localScale *= 1.1f;
        thander1.transform.localScale *= 0.8f;
        thander2.transform.localScale *= 0.8f;
    }
}
