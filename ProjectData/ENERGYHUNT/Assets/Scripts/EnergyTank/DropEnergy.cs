using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropEnergy : MonoBehaviour
{
    //�G�l���M�[��
    public int energyAmount;

    [SerializeField, Header("Explosion�X�N���v�g")]
    ExplosionScript[] explosionScript;

    [SerializeField, Header("�v���C���[�̃|�W�V����")]
    GameObject[] playerObj;

    [SerializeField, Header("EnergyTank�X�N���v�g")]
    EnergyTank[] energytankSC;

    [SerializeField, Header("DropEnergyTank�X�N���v�g")]
    DropEnergyTank dropEnergyTank;

    [SerializeField, Header("�X�^�[�g�̃|�W�V����")]
    Vector3 startPos;

    [SerializeField, Header("���[�V�����g���C��")]
    GameObject motionTrail;

    //�^�[�Q�b�g�i���o�[
    int targetNum;

    //�^�[�Q�b�g�����邩�ǂ���
    bool target;

    [SerializeField] float speed = 1;
    
    //�����^���N�I�u�W�F�N�g����G�l���M�[�����p��
    public void SetEnergyAmount(int amount)
    {
        energyAmount = amount;
    }

    //�|�W�V�������Z�b�g
    public void PosReset()
    {
        transform.localPosition = startPos;
    }

    //�^�[�Q�b�g��I��
    public IEnumerator SelectPos(int playerNum)
    {
        //�^�[�Q�b�g��ݒ�
        targetNum = playerNum;

        StartCoroutine(SendDropEnergy());

        yield return null;
    }

    IEnumerator SendDropEnergy()
    {
        //�h���b�v�I�u�W�F�N�g��������悤�ɏ����҂�
        yield return new WaitForSeconds(0.2f);

        //�^�[�Q�b�g�����ON�ɂ���
        target = true;

        //�g���C���\��
        motionTrail.SetActive(true);

        //�^�[�Q�b�g�Ɍ����ăh���b�v�I�u�W�F�N�g���΂�
        while (target)
        {
            //�h���b�v�I�u�W�F�N�g���^�[�Q�b�g�ɋ߂Â���
            transform.position = Vector3.MoveTowards(transform.position, playerObj[targetNum].transform.position, speed);

            //�h���b�v�I�u�W�F�N�g���^�[�Q�b�g�ɒ�������
            if (transform.position == playerObj[targetNum].transform.position)
            {
                //�g���C����\��
                motionTrail.SetActive(false);

                //�^�[�Q�b�g���v���C���[�Ȃ�
                if (targetNum != 0)
                {
                    //�^�[�Q�b�g�v���C���[�̃G�l���M�[����
                    energytankSC[targetNum].ChargeEnergy(energyAmount);
                }
                //�^�[�Q�b�g���z���I�u�W�F�N�g�Ȃ�
                else
                {
                    //�z���I�u�W�F�N�g�̃G�l���M�[�𑝉�
                    dropEnergyTank.ScaleChange(energyAmount);
                }

                //�^�[�Q�b�g�ƃ|�W�V���������Z�b�g
                target = false;
                PosReset();
            }

            //1�t���[���҂�
            yield return null;
        }
    }
}
