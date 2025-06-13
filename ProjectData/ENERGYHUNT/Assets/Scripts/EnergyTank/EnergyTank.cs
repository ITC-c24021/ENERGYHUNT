using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnergyTank : MonoBehaviour
{
    //�G�l���M�[�������̃J�E���g
    float energyTimer;

    [SerializeField, Header("�G�l���M�[�����܂鑬��")]
    float chargeSpeed = 1f;

    [SerializeField, Header("�^���N�̗e��")]
    int maxEnergy;

    //�G�l���M�[������
    public int energyAmount;

    [SerializeField, Header("�g�їp�̃^���N�I�u�W�F�N�g")]
    GameObject enargyTankObj;

    //�}�e���A���ύX�p�̃��b�V��
    MeshRenderer enargyMesh;

    [SerializeField, Header("�G�l���M�[�̃}�e���A��")]
    Material[] energyMat;

    [SerializeField, Header("���~�p�̃^���N�I�u�W�F�N�g")]
    GameObject savingTankObj;

    //���~�p�̃^���N�I�u�W�F�N�g�̃X�N���v�g
    SavingTank savingTankSC;

    //�^���N�ɗa�����邩�̔���
    bool canSave = false;

    //�G�l���M�[���ڂ����Ɏg���{�^������̓��͂��󂯎��ϐ�
    InputAction putInAction;

    //�G�l���M�[�R�A���������Ă��邩�ǂ����̔���
    [SerializeField, Header("�G�l���M�[�`���[�W�����ǂ����̔���")]
    bool charge;

    [SerializeField, Header("�G�l���M�[���h���b�v�����Ƃ��p�̃I�u�W�F�N�g")]
    GameObject dropEnergyObj;

    //�h���b�v�I�u�W�F�N�g�̃X�N���v�g
    DropEnergy dropEnergySC;

    [SerializeField, Header("PlayerController�X�N���v�g")]
    PlayerController playerControllerSC;

    [SerializeField, Header("Barrier�X�N���v�g")]
    BarrierScript barrierSC;

    //�����^���N�̏����ʒu
    Vector3 startPos;
    //�����X�P�[��
    Vector3 startScale;

    [SerializeField, Header("�G�l���M�[���^������SE")]
    AudioSource fullEnargySE;

    [SerializeField, Header("�^���N�G���A�̃G�t�F�N�g")]
    GameObject areaEffect;

    [SerializeField, Header("�|�[�Y���")]
    GameObject poseImage;

    void Start()
    {
        //PlayerInput�R���|�[�l���g���擾
        var input = GetComponent<PlayerInput>();

        //�擾����PlayerInput��ActionMap���擾
        var actionMap = input.currentActionMap;

        //ActionMap���̎��o���A�a����A�N�V�������擾
        putInAction = actionMap["PutIn"];

        //�X�N���v�g�擾
        dropEnergySC = dropEnergyObj.GetComponent<DropEnergy>();
        savingTankSC = savingTankObj.GetComponent<SavingTank>();

        //�X�^�[�g���̌g�у^���N�̈ʒu�ƃX�P�[�����擾
        startPos = enargyTankObj.transform.localPosition;
        startScale = enargyTankObj.transform.localScale;

        //�g�у^���N�̃��b�V�����擾
        enargyMesh = enargyTankObj.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        //�G�l���M�[���ڂ��{�^���������ꂽ���ǂ������`�F�b�N
        var putIn = putInAction.triggered;

        if (putIn)
        {
            Time.timeScale = 0;

            poseImage.SetActive(true);
        }

        //�G�l���M�[�R�A�������̃G�l���M�[����
        if (charge && energyAmount < maxEnergy)
        {
            energyTimer += Time.deltaTime * chargeSpeed;

            //1�b���ƂɃG�l���M�[����
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

            //�G�l���M�[�̗a�����ꂪ�ł����Ԃ�������a�������
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


    //�G�l���M�[�����ƃX�P�[���A�b�v
    public void ChargeEnergy(int amount)
    {

        //���݂̃G�l���M�[�ʂɎ擾��������
        energyAmount += amount;

        //�����G�l���M�[���^���N�e�ʂ𒴂��Ȃ��悤�ɂ���
        if (energyAmount >= maxEnergy)
        {
            //�Z�b�g�G���A�\��
            areaEffect.SetActive(true);

            //����Œ�
            energyAmount = maxEnergy;

            //�G�l���M�[���^�����̌����ڂɕύX
            enargyMesh.material = energyMat[1];

            //���^������SE�Đ�
            fullEnargySE.Play();
        }

        //�G�l���M�[�ʂɉ����ă^���N�̃��[�^�[���X�P�[���A�b�v���A���[�^�[�̃|�W�V�����𒲐�
        enargyTankObj.transform.localScale
            = new Vector3(startScale.x, energyAmount * 0.045f, startScale.z);
        enargyTankObj.transform.localPosition
            = new Vector3(startPos.x, startPos.y - (energyAmount * 0.0215f), startPos.z);
    }

    //�G�l���M�[�R�A������ԕύX
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

    //�G�l���M�[���Z�b�g
    public void EnergyReset()
    {
        //�G�l���M�[���Z�b�g
        energyAmount = 0;

        //�}�e���A�������Z�b�g
        enargyMesh.material = energyMat[0];

        //���[�^�[�̃X�P�[���ƃ|�W�V���������Z�b�g
        enargyTankObj.transform.localScale = startScale;
        enargyTankObj.transform.localPosition = startPos;
    }

    //���S���̏����G�l���M�[�h���b�v
    public void DropEnergy(int target)
    {
        //�G�l���M�[�R�A������Ԃ̉���
        HaveCore(false);

        //�^���N�̃G�l���M�[�ʂ�0�łȂ��ꍇ�A�h���b�v�I�u�W�F�N�g�𗎂Ƃ�
        if (energyAmount != 0)
        {
            //�h���b�v�I�u�W�F�N�g�����g�̈ʒu�Ɉړ����ăh���b�v
            dropEnergyObj.transform.localScale = new Vector3(energyAmount * 0.3f, energyAmount * 0.3f, energyAmount * 0.3f);
            dropEnergyObj.transform.position = enargyTankObj.transform.position;

            //�h���b�v�I�u�W�F�N�g�ɃG�l���M�[�����p��
            dropEnergySC.SetEnergyAmount(energyAmount);

            //�h���b�v�I�u�W�F�N�g�̃^�[�Q�b�g��I��
            StartCoroutine(dropEnergySC.SelectPos(target));
        }

        //�G�l���M�[��\��
        enargyTankObj.SetActive(false);

        //�G���A��\��
        areaEffect.SetActive(false);

        //�G�l���M�[���Z�b�g
        energyAmount = 0;

        //�}�e���A�������Z�b�g
        enargyMesh.material = energyMat[0];

        //���[�^�[�̃X�P�[���ƃ|�W�V���������Z�b�g
        enargyTankObj.transform.localScale = startScale;
        enargyTankObj.transform.localPosition = startPos;

    }

    //���~�p�̃^���N�ɃG�l���M�[�^���N���ڂ�
    public void TankCharging()
    {
        //���~�p�̃^���N�̃G�l���M�[����
        savingTankSC.Charge();

        //�G�l���M�[��\��
        enargyTankObj.SetActive(false);

        //�G���A��\��
        areaEffect.SetActive(false);

        //�G�l���M�[���Z�b�g
        energyAmount = 0;

        //�}�e���A�������Z�b�g
        enargyMesh.material = energyMat[0];

        //���[�^�[�̃X�P�[���ƃ|�W�V���������Z�b�g
        enargyTankObj.transform.localScale = startScale;
        enargyTankObj.transform.localPosition = startPos;
    }

    public void ChargeSpeedUP(float speed)
    {
        chargeSpeed = speed;
    }
}
