using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ServerListener : MonoBehaviour
{
    private TcpListener _tcpListener;

    // Use this for initialization
    void Start()
    {
        Listen();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMessageEnter(Message message)
    {
        var obj = JsonUtility.FromJson<PlayerMovement>(message.Data);
        Debug.Log(obj.Direction);
    }

    private void Listen()
    {
        StartCoroutine(Listening());
    }

    private IEnumerator Listening()
    {
        _tcpListener = new TcpListener(IPAddress.Loopback, 54321);
        _tcpListener.Start();

        while (true)
        {
            yield return new WaitUntil(_tcpListener.Pending);

            var incomming = _tcpListener.AcceptTcpClient();
            var messageBody = incomming.GetStream();
            var reader = new StreamReader(messageBody);
            var message = new Message();
            OnMessageEnter(message.Deserialize(reader.ReadToEnd()));
            yield return null;
        }
    }
}