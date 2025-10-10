using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class BoosterSaver : MonoBehaviour
{
    public static BoosterSaver Instance { get; private set; }
    public int BoostersCount
    {
        get { return _boostersCount; }
        set
        {
            _boostersCount = value;
            Save();
        }
    }
    public int BoostersSpeCount
    {
        get { return _boosterSpeCount; }
        set
        {
            _boosterSpeCount = value;
            Save();
        }
    }
    readonly string key = "koala_horra_mamamia";
    readonly string salt = "zefzefdfg";

    [SerializeField] GameObject specialBoosterPrefab = null;
    [SerializeField] Printer _printer = null;
    int _boostersCount = 0;
    int _boosterSpeCount = 0;

    private void Awake()
    {
        Instance = this;
        Load();
    }

    void Save()
    {
        string boosterNumber = EncryptInt(_boostersCount, key, salt);
        string boosterSpeNumber = EncryptInt(_boosterSpeCount, key, salt);

        PlayerPrefs.SetString("BoosterNumber", boosterNumber);
        PlayerPrefs.SetString("BoosterSpeNumber", boosterSpeNumber);
    }

    private void Load()
    {
        string saveBoosterNumber = PlayerPrefs.GetString("BoosterNumber", "");
        string saveBoosterSpeNumber = PlayerPrefs.GetString("BoosterSpeNumber", "");

        if (saveBoosterNumber != "")
        {
            _boostersCount = 0;
            int boosterNumber = DecryptInt(saveBoosterNumber, key, salt);
            if (boosterNumber != 0)
                _printer.PrintBoosters(boosterNumber).Forget();

        }
        if (saveBoosterSpeNumber != "")
        {
            _boosterSpeCount = 0;
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
