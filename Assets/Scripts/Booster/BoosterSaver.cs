using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

public class BoosterSaver : MonoBehaviour
{
    readonly string key = "koala_horra_mamamia";
    readonly string salt = "zefzefdfg";

    [SerializeField] GameObject specialBoosterPrefab = null;
    [SerializeField] Printer _printer = null;

    private void Start()
    {
        Load();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus == false)
            Save();
    }

    private void OnApplicationPause(bool focus)
    {
            Save();
    }

    private void Save()
    {
        int boostersLength = FindObjectsByType<BoosterOpening>(FindObjectsSortMode.None).Length;
        int boosterSpeLength = FindObjectsByType<SpecialBooster>(FindObjectsSortMode.None).Length;

        boostersLength -= boosterSpeLength;

        string boosterNumber = EncryptInt(boostersLength, key, salt);
        string boosterSpeNumber = EncryptInt(boosterSpeLength, key, salt);

        PlayerPrefs.SetString("BoosterNumber", boosterNumber);
        PlayerPrefs.SetString("BoosterSpeNumber", boosterSpeNumber);
    }

    private void Load()
    {
        string saveBoosterNumber = PlayerPrefs.GetString("BoosterNumber", "");
        string saveBoosterSpeNumber = PlayerPrefs.GetString("BoosterSpeNumber", "");

        if (saveBoosterNumber != "")
        {
            int boosterNumber = DecryptInt(saveBoosterNumber, key, salt);
            if (boosterNumber != 0)
                _printer.PrintBoosters(boosterNumber).Forget();
            
        }
        if (saveBoosterSpeNumber != "")
        {
            int boosterSpeNumber = DecryptInt(saveBoosterSpeNumber, key, salt);
            if (boosterSpeNumber != 0)
            {

                _printer.Print(specialBoosterPrefab).Forget();
            }

        }
    }
    
    public string EncryptInt(int value, string key, string salt)
    {
        uint v = unchecked((uint)value);
        byte[] ks = KeyStream(key, salt, 4);
        uint c = v ^ BitConverter.ToUInt32(ks, 0);

        byte[] cBytes = BitConverter.GetBytes(c); // 4 octets
        byte[] tag = Tag(key, salt, cBytes);      // 4 octets
        return Convert.ToBase64String(cBytes.Concat(tag).ToArray());
    }
    
    public int DecryptInt(string token, string key, string salt)
    {
        byte[] data = Convert.FromBase64String(token);
        if (data.Length != 8)
        {
            Debug.LogError("Token invalide");
            return (0);
        }

        byte[] cBytes = data.Take(4).ToArray();
        byte[] tag = data.Skip(4).Take(4).ToArray();
        byte[] expect = Tag(key, salt, cBytes);
        if (!tag.SequenceEqual(expect))
        {
            Debug.LogError("Tag invalide");
            return (0);
        }

        byte[] ks = KeyStream(key, salt, 4);
        uint c = BitConverter.ToUInt32(cBytes, 0);
        uint v = c ^ BitConverter.ToUInt32(ks, 0);
        return unchecked((int)v);
    }
    
    private byte[] KeyStream(string key, string salt, int n)
    {
        using var h = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        byte[] outBuf = new byte[n];
        int filled = 0;
        int counter = 0;
        while (filled < n)
        {
            byte[] block = h.ComputeHash(Encoding.UTF8.GetBytes($"{salt}|{counter}"));
            int toCopy = Math.Min(block.Length, n - filled);
            Buffer.BlockCopy(block, 0, outBuf, filled, toCopy);
            filled += toCopy;
            counter++;
        }
        return outBuf;
    }
    
    private byte[] Tag(string key, string salt, byte[] cipher)
    {
        using var h = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        byte[] mac = h.ComputeHash(Encoding.UTF8.GetBytes(salt).Concat(cipher).ToArray());
        return mac.Take(4).ToArray();
    }
}
