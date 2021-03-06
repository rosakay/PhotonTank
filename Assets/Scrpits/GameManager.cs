using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tanks
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager instance;
        public static GameObject localPlayer;
        string gameVersion = "1";

        private GameObject defaultSpawnPoint;

        void Awake()
        {
            if (instance != null)
            {
                Debug.LogErrorFormat(gameObject,
                "Multiple instances of {0} is not allow", GetType().Name);
                DestroyImmediate(gameObject);
                return;
            }

            PhotonNetwork.AutomaticallySyncScene = true;
            DontDestroyOnLoad(gameObject);
            instance = this;

            defaultSpawnPoint = new GameObject("Default SpawnPoint");
            defaultSpawnPoint.transform.position = new Vector3(0, 0, 0);
            defaultSpawnPoint.transform.SetParent(transform, false);
        }
        void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            PhotonNetwork.GameVersion = gameVersion;
        }
        public bool ConnectToServer(string account)
        {
            PhotonNetwork.NickName = account;
            return PhotonNetwork.ConnectUsingSettings();
        }
        public override void OnConnected()
        {
            Debug.Log("PUN Connected");
        }
        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Connected to Master");
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Disconnected was called by PUN with reason {0}", cause);
        }

        public void JoinGameRoom()
        {
            var options = new RoomOptions
            {
                MaxPlayers = 6,
                EmptyRoomTtl = 2000
            };
            PhotonNetwork.JoinOrCreateRoom("Kingdom", options, null);
        }
        public override void OnCreatedRoom()
        {
            Debug.Log("Created room!!");
        }
        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Created room!!");
                PhotonNetwork.LoadLevel("GameScene");
            }
            else
            {
                Debug.Log("Joined room!!");
            }
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var localTank = GetComponent<PhotonView>();
            if (!PhotonNetwork.InRoom)
            {
                Debug.Log($"Scene Loaded: Failed, {scene.name}");
                return;
            }
            localPlayer = PhotonNetwork.Instantiate("TankPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
            Debug.Log($"Scene Loaded: Sussecc, {scene.name}");
            Debug.Log("Player Instance ID: " + localPlayer.GetInstanceID());
            //PhotonNetwork.NickName = "Karl";
            //Debug.Log($"{localTank.ViewID}");
        }

        public static List<GameObject> GetAllObjectsOfTypeInScene<T>()
        {
            var objectsInScene = new List<GameObject>();
            foreach (var go in (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject)))
            {
                if (go.hideFlags == HideFlags.NotEditable ||
                go.hideFlags == HideFlags.HideAndDontSave)
                    continue;
                if (go.GetComponent<T>() != null)
                    objectsInScene.Add(go);
            }
            return objectsInScene;
        }
        private Transform GetRandomSpawnPoint()
        {
            var spawnPoints = GetAllObjectsOfTypeInScene<SpawnPoint>();
            return spawnPoints.Count == 0
            ? defaultSpawnPoint.transform
            : spawnPoints[Random.Range(0, spawnPoints.Count)].transform;
        }
    }
}
