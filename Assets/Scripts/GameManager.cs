﻿using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance { get; set; }

        public GameObject LobbyMenu;
        public GameObject ConnectMenu;
        public GameObject HostMenu;

        public Client Client;

        public Server Server;

        private bool _isHosting;

        void Start()
        {
            Instance = this;

            ConnectMenu.SetActive(false);
            HostMenu.SetActive(false);

            _isHosting = false;

            DontDestroyOnLoad(gameObject);

        }

        void Update()
        {
            if (Client != null && Client.NetClient.connection != null)
            {
                //Debug.Log("Address: " + Client.connection.address);
            }

            if (_isHosting)
            {
                Debug.Log("Port: " + NetworkServer.listenPort);
                Debug.Log("Connections: " + NetworkServer.connections.Count);
            }


        }

        public void ConnectButton()
        {
            LobbyMenu.SetActive(false);
            ConnectMenu.SetActive(true);
            HostMenu.SetActive(false);
            Debug.Log("Connect");
        }

        public void HostButton()
        {
            try
            {
                Server = gameObject.AddComponent<Server>();
                Server.Init();
                
                Client = new Client();
                Client.InitHost();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            LobbyMenu.SetActive(false);
            HostMenu.SetActive(true);
            ConnectMenu.SetActive(false);
            Debug.Log("Host");
        }

        public void ConnectToServerButton()
        {
            string hostAddress = GameObject.Find("HostInput").GetComponent<InputField>().text;

            if (string.IsNullOrEmpty(hostAddress))
            {
                hostAddress = "127.0.0.1";
            }

            try
            {
                Client = new Client();
                Client.Init(hostAddress, 1300);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void BackButton()
        {
            LobbyMenu.SetActive(true);
            ConnectMenu.SetActive(false);
            HostMenu.SetActive(false);

            if (Client != null && Client.NetClient.isConnected)
            {
                Client.NetClient.Disconnect();
                Client = null;
            }

            if (_isHosting)
            {
                _isHosting = false;
                Server.ShutDown();
                Server = null;
            }

        }

        public void StartGame()
        {
            SceneManager.LoadScene("dejarik");
        }

    }
}
