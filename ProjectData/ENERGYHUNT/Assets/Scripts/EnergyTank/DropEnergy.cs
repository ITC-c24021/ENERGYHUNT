using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropEnergy : MonoBehaviour
{
    //エネルギー量
    public int energyAmount;

    [SerializeField, Header("Explosionスクリプト")]
    ExplosionScript[] explosionScript;

    [SerializeField, Header("プレイヤーのポジション")]
    GameObject[] playerObj;

    [SerializeField, Header("EnergyTankスクリプト")]
    EnergyTank[] energytankSC;

    [SerializeField, Header("DropEnergyTankスクリプト")]
    DropEnergyTank dropEnergyTank;

    [SerializeField, Header("スタートのポジション")]
    Vector3 startPos;

    [SerializeField, Header("モーショントレイル")]
    GameObject motionTrail;

    //ターゲットナンバー
    int targetNum;

    //ターゲットがいるかどうか
    bool target;

    [SerializeField] float speed = 1;
    
    //所持タンクオブジェクトからエネルギーを引継ぎ
    public void SetEnergyAmount(int amount)
    {
        energyAmount = amount;
    }

    //ポジションリセット
    public void PosReset()
    {
        transform.localPosition = startPos;
    }

    //ターゲットを選択
    public IEnumerator SelectPos(int playerNum)
    {
        //ターゲットを設定
        targetNum = playerNum;

        StartCoroutine(SendDropEnergy());

        yield return null;
    }

    IEnumerator SendDropEnergy()
    {
        //ドロップオブジェクトが見えるように少し待つ
        yield return new WaitForSeconds(0.2f);

        //ターゲット判定をONにする
        target = true;

        //トレイル表示
        motionTrail.SetActive(true);

        //ターゲットに向けてドロップオブジェクトを飛ばす
        while (target)
        {
            //ドロップオブジェクトをターゲットに近づける
            transform.position = Vector3.MoveTowards(transform.position, playerObj[targetNum].transform.position, speed);

            //ドロップオブジェクトがターゲットに着いたら
            if (transform.position == playerObj[targetNum].transform.position)
            {
                //トレイル非表示
                motionTrail.SetActive(false);

                //ターゲットがプレイヤーなら
                if (targetNum != 0)
                {
                    //ターゲットプレイヤーのエネルギー増加
                    energytankSC[targetNum].ChargeEnergy(energyAmount);
                }
                //ターゲットが吸収オブジェクトなら
                else
                {
                    //吸収オブジェクトのエネルギーを増加
                    dropEnergyTank.ScaleChange(energyAmount);
                }

                //ターゲットとポジションをリセット
                target = false;
                PosReset();
            }

            //1フレーム待つ
            yield return null;
        }
    }
}
