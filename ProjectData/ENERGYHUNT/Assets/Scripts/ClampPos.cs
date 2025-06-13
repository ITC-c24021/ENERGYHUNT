using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampPos : MonoBehaviour
{
    //�ړ��������|���邽�߂̃|�W�V����
    Vector3 pos;

    [SerializeField,Header("X�������͈͍̔ŏ��l")] 
    float minX;

    [SerializeField,Header("X�������͈͍̔ő�l")] 
    float maxX;

    [SerializeField,Header("Z�������͈͍̔ŏ��l")] 
    float minZ;

    [SerializeField,Header("Z�������͈͍̔ő�l")] 
    float maxZ;

    void Update()
    {
        //���̃X�N���v�g���t���Ă���I�u�W�F�N�g�̃|�W�V�������擾
        pos = transform.localPosition;

        //X����Z���͈̔͂𐧌�
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        //�|�W�V�����𐧌�
        transform.localPosition = pos;
    }
}
