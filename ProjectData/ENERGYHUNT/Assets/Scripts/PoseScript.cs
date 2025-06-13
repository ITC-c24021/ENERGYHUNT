using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]

public class PoseScript : MonoBehaviour
{
    [SerializeField, Header("ポーズ画面")]
    GameObject poseImage;

    [SerializeField, Header("ポーズUI")]
    GameObject[] poseUI;


    //UI切り替え変数
    private int uiNum = 0;


    float maxScale = 1.1f;

    float maxTime = 1f;


    //ポーズ画面アクション
    private InputAction stickAction, selectAction, poseAction;


    private AnimationCurve animationCurve;


    private Coroutine currentCoroutine;


    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    AudioClip selectSE;

    [SerializeField]
    AudioClip decisionSE;


    void Start()
    {
        //プレイヤーのActionMapを取得
        var input = GetComponent<PlayerInput>();
        var actionMap = input.currentActionMap;

        stickAction = actionMap["Move"];
        selectAction = actionMap["PickUp"];
        poseAction = actionMap["PutIn"];

        animationCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f));

        poseUI[uiNum].SetActive(true);
        StartAnimationForScene();
    }

    void Update()
    {
        //ポーズ画面に移動
        var poseAct = poseAction.triggered;

        if (poseAct)
        {
            //動けなくする
            Time.timeScale = 0;

            poseImage.SetActive(true);
        }


        //決定
        var selectAct = selectAction.triggered;

        if (selectAct && Time.timeScale == 0)
        {
            switch (uiNum)
            {
                case 0:
                    audioSource.PlayOneShot(decisionSE);

                    Invoke("DeletePanel", 0.3f);

                    //動けるようにする
                    Time.timeScale = 1;
                    break;
                case 1:
                    audioSource.PlayOneShot(decisionSE);
                    //動けるようにする
                    Time.timeScale = 1;
                    Invoke("SelectTitle", 0.3f);
                    break;
            } 
        }


        //UI切り替え
        var stickAct = stickAction.ReadValue<Vector2>().y;

        if (stickAct > 0 && Time.timeScale == 0 && uiNum != 0)
        {
            poseUI[uiNum].SetActive(false);

            uiNum = 0;

            audioSource.PlayOneShot(selectSE);

            StartAnimationForScene();

            poseUI[uiNum].SetActive(true);
        }

        if (stickAct < 0 && Time.timeScale == 0 && uiNum != 1)
        {
            poseUI[uiNum].SetActive(false);

            uiNum = 1;
            audioSource.PlayOneShot(selectSE);

            StartAnimationForScene();

            poseUI[uiNum].SetActive(true);
        }
    }

    private void DeletePanel()
    {
        poseImage.SetActive(false);
    }

    private void SelectTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    void StartAnimationForScene()
    {
        // 現在のコルーチンを停止
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);

            // 現在のUIのスケールを元に戻す
            GameObject currentUI = poseUI[uiNum];
            currentUI.transform.localScale = new Vector3(0.3f, 0.3f, 1);  // 元のスケールにリセット
        }

        // 選択された UI の初期スケールを取得
        GameObject targetUI = poseUI[uiNum];
        Vector3 originalScale = targetUI.transform.localScale;

        // 新しいコルーチンを開始
        currentCoroutine = StartCoroutine(SelectUIScaleLoop(targetUI, originalScale));
    }

    //ScaleのUpとDownの処理
    IEnumerator SelectUIScaleLoop(GameObject targetUI, Vector3 originalScale)
    {
        float elapsedTime = 0f;
        Vector3 targetScale = originalScale * maxScale;

        //無限ループ
        while (true)
        {
            while (elapsedTime < maxTime)
            {
                elapsedTime += Time.unscaledDeltaTime;

                // カーブに従ったスケール値を計算
                float t = elapsedTime / maxTime;
                float scaleFactor = animationCurve.Evaluate(t);
                targetUI.transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleFactor);

                yield return null;
            }

            elapsedTime = 0f; // 経過時間をリセット
        }
    }
}
