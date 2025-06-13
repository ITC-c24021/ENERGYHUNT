using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnergyTank : MonoBehaviour
{
    //エネルギー増加中のカウント
    float energyTimer;

    [SerializeField, Header("エネルギーが貯まる速さ")]
    float chargeSpeed = 1f;

    [SerializeField, Header("タンクの容量")]
    int maxEnergy;

    //エネルギー所持量
    public int energyAmount;

    [SerializeField, Header("携帯用のタンクオブジェクト")]
    GameObject enargyTankObj;

    //マテリアル変更用のメッシュ
    MeshRenderer enargyMesh;

    [SerializeField, Header("エネルギーのマテリアル")]
    Material[] energyMat;

    [SerializeField, Header("貯蓄用のタンクオブジェクト")]
    GameObject savingTankObj;

    //貯蓄用のタンクオブジェクトのスクリプト
    SavingTank savingTankSC;

    //タンクに預けられるかの判定
    bool canSave = false;

    //エネルギーを移す時に使うボタンからの入力を受け取る変数
    InputAction putInAction;

    //エネルギーコアを所持しているかどうかの判定
    [SerializeField, Header("エネルギーチャージ中かどうかの判定")]
    bool charge;

    [SerializeField, Header("エネルギーがドロップしたとき用のオブジェクト")]
    GameObject dropEnergyObj;

    //ドロップオブジェクトのスクリプト
    DropEnergy dropEnergySC;

    [SerializeField, Header("PlayerControllerスクリプト")]
    PlayerController playerControllerSC;

    [SerializeField, Header("Barrierスクリプト")]
    BarrierScript barrierSC;

    //所持タンクの初期位置
    Vector3 startPos;
    //初期スケール
    Vector3 startScale;

    [SerializeField, Header("エネルギー満タン時のSE")]
    AudioSource fullEnargySE;

    [SerializeField, Header("タンクエリアのエフェクト")]
    GameObject areaEffect;

    [SerializeField, Header("ポーズ画面")]
    GameObject poseImage;

    void Start()
    {
        //PlayerInputコンポーネントを取得
        var input = GetComponent<PlayerInput>();

        //取得したPlayerInputのActionMapを取得
        var actionMap = input.currentActionMap;

        //ActionMap内の取り出し、預入れアクションを取得
        putInAction = actionMap["PutIn"];

        //スクリプト取得
        dropEnergySC = dropEnergyObj.GetComponent<DropEnergy>();
        savingTankSC = savingTankObj.GetComponent<SavingTank>();

        //スタート時の携帯タンクの位置とスケールを取得
        startPos = enargyTankObj.transform.localPosition;
        startScale = enargyTankObj.transform.localScale;

        //携帯タンクのメッシュを取得
        enargyMesh = enargyTankObj.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        //エネルギーを移すボタンが押されたかどうかをチェック
        var putIn = putInAction.triggered;

        if (putIn)
        {
            Time.timeScale = 0;

            poseImage.SetActive(true);
        }

        //エネルギーコア所持時のエネルギー増加
        if (charge && energyAmount < maxEnergy)
        {
            energyTimer += Time.deltaTime * chargeSpeed;

            //1秒ごとにエネルギー増加
            if (energyTimer >= 1)
            {
                energyTimer = 0;

                ChargeEnergy(1);
            }
        }
        else
        {
            energyTimer = 0;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == $"TankArea{playerControllerSC.controllerNum}")
        {
            canSave = true;

            //エネルギーの預け入れができる状態だったら預け入れる
            if (canSave && energyAmount >= maxEnergy && !barrierSC.isBarrier && !playerControllerSC.isDead)
            {
                TankCharging();
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == $"TankArea{playerControllerSC.controllerNum}")
        {
            canSave = false;
        }
    }


    //エネルギー増加とスケールアップ
    public void ChargeEnergy(int amount)
    {

        //現在のエネルギー量に取得分加える
        energyAmount += amount;

        //所持エネルギーがタンク容量を超えないようにする
        if (energyAmount >= maxEnergy)
        {
            //セットエリア表示
            areaEffect.SetActive(true);

            //上限固定
            energyAmount = maxEnergy;

            //エネルギー満タン時の見た目に変更
            enargyMesh.material = energyMat[1];

            //満タン時のSE再生
            fullEnargySE.Play();
        }

        //エネルギー量に応じてタンクのメーターをスケールアップし、メーターのポジションを調整
        enargyTankObj.transform.localScale
            = new Vector3(startScale.x, energyAmount * 0.045f, startScale.z);
        enargyTankObj.transform.localPosition
            = new Vector3(startPos.x, startPos.y - (energyAmount * 0.0215f), startPos.z);
    }

    //エネルギーコア所持状態変更
    public void HaveCore(bool have)
    {
        if (have)
        {
            charge = true;
        }
        else
        {
            charge = false;
        }
    }

    //エネルギーリセット
    public void EnergyReset()
    {
        //エネルギーリセット
        energyAmount = 0;

        //マテリアルをリセット
        enargyMesh.material = energyMat[0];

        //メーターのスケールとポジションをリセット
        enargyTankObj.transform.localScale = startScale;
        enargyTankObj.transform.localPosition = startPos;
    }

    //死亡時の所持エネルギードロップ
    public void DropEnergy(int target)
    {
        //エネルギーコア所持状態の解除
        HaveCore(false);

        //タンクのエネルギー量が0でない場合、ドロップオブジェクトを落とす
        if (energyAmount != 0)
        {
            //ドロップオブジェクトを自身の位置に移動してドロップ
            dropEnergyObj.transform.localScale = new Vector3(energyAmount * 0.3f, energyAmount * 0.3f, energyAmount * 0.3f);
            dropEnergyObj.transform.position = enargyTankObj.transform.position;

            //ドロップオブジェクトにエネルギーを引継ぎ
            dropEnergySC.SetEnergyAmount(energyAmount);

            //ドロップオブジェクトのターゲットを選択
            StartCoroutine(dropEnergySC.SelectPos(target));
        }

        //エネルギー非表示
        enargyTankObj.SetActive(false);

        //エリア非表示
        areaEffect.SetActive(false);

        //エネルギーリセット
        energyAmount = 0;

        //マテリアルをリセット
        enargyMesh.material = energyMat[0];

        //メーターのスケールとポジションをリセット
        enargyTankObj.transform.localScale = startScale;
        enargyTankObj.transform.localPosition = startPos;

    }

    //貯蓄用のタンクにエネルギータンクを移す
    public void TankCharging()
    {
        //貯蓄用のタンクのエネルギー増加
        savingTankSC.Charge();

        //エネルギー非表示
        enargyTankObj.SetActive(false);

        //エリア非表示
        areaEffect.SetActive(false);

        //エネルギーリセット
        energyAmount = 0;

        //マテリアルをリセット
        enargyMesh.material = energyMat[0];

        //メーターのスケールとポジションをリセット
        enargyTankObj.transform.localScale = startScale;
        enargyTankObj.transform.localPosition = startPos;
    }

    public void ChargeSpeedUP(float speed)
    {
        chargeSpeed = speed;
    }
}
