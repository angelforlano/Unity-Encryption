using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

public class Test : MonoBehaviour
{
    public string text;
    public string userPass = "securePassword";
    public string encriptedText;
    public string decriptedText;

    UTF8Encoding encoder;

    
    void Start()
    {
        encoder = new UTF8Encoding();
        Debug.Log(Convert.ToBase64String(encoder.GetBytes("keyString")));
        Debug.Log(encoder.GetBytes("keyString").Take(16));
        Debug.Log(Convert.ToBase64String(encoder.GetBytes("keyString").Take(16).ToArray()));
        
        
        //Debug.Log(Convert.ToBase64String(Secure.ShowKey()));
        //encriptedText = Secure.EncryptByUserPassword(text, userPass);
        //decriptedText = Secure.DecryptString(encriptedText);
        //Debug.Log(Convert.ToBase64String(Secure.GenerateIV()));
    }
}