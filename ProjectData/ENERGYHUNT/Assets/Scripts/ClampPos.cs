using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampPos : MonoBehaviour
{
    //移動制限を掛けるためのポジション
    Vector3 pos;

    [SerializeField,Header("X軸方向の範囲最小値")] 
    float minX;

    [SerializeField,Header("X軸方向の範囲最大値")] 
    float maxX;

    [SerializeField,Header("Z軸方向の範囲最小値")] 
    float minZ;

    [SerializeField,Header("Z軸方向の範囲最大値")] 
    float maxZ;

    void Update()
    {
        //このスクリプトが付いているオブジェクトのポジションを取得
        pos = transform.localPosition;

        //X軸とZ軸の範囲を制限
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        //ポジションを制限
        transform.localPosition = pos;
    }
}
