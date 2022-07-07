using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tanks
{
    public class MainMenu : MonoBehaviourPunCallbacks
    {
        public static MainMenu instance; // 改成 Public
        private GameObject m_ui;
        private TMP_InputField m_accountInput; // 新增 輸入匡
        private Button m_loginButton; // 新增 登入按鈕
        private Button m_joinGameButton;
        void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            instance = this;

            m_ui = transform.FindAnyChild<Transform>("UI").gameObject;
            m_accountInput = transform.FindAnyChild<TMP_InputField>("AccountInput"); // 抓取輸入匡元件
            m_loginButton = transform.FindAnyChild<Button>("LoginButton"); // 抓取登入按鈕元件
            m_joinGameButton = transform.FindAnyChild<Button>("JoinGameButton");

            ResetUI(); // 抽出 UI 初始化
        }
        //public override void OnConnectedToMaster()
        //{
        //    m_joinGameButton.interactable = true;
        //}
        private void ResetUI() // 重置 UI
        {
            m_ui.SetActive(true);
            m_accountInput.gameObject.SetActive(true);
            m_loginButton.gameObject.SetActive(true);
            m_joinGameButton.gameObject.SetActive(false);
            m_accountInput.interactable = true;
            m_loginButton.interactable = true;
            m_joinGameButton.interactable = true;
        }
        public override void OnEnable()
        {
            // Always call the base to add callbacks
            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        public void Login() // 處理 登入伺服器流程
        {
            if (string.IsNullOrEmpty(m_accountInput.text))
            {
                Debug.Log("Please input your account!!");
                return;
            }
            m_accountInput.interactable = false;
            m_loginButton.interactable = false;
            if (!GameManager.instance.ConnectToServer(m_accountInput.text))
            {
                Debug.Log("Connect to PUN Failed!!");
            }
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            m_ui.SetActive(!PhotonNetwork.InRoom);
        }
        public override void OnConnectedToMaster() // 處理連線後 UI 變化
        {
            m_accountInput.gameObject.SetActive(false);
            m_loginButton.gameObject.SetActive(false);
            m_joinGameButton.gameObject.SetActive(true);
        }
    }
}
