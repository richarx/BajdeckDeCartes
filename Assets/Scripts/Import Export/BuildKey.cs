
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class BuildKey : ScriptableObject
{
    [SerializeField] private string _stored;
    private const string PEPPER = "BloubPouetTrucMachin";

    public string Value => Convert.ToBase64String(GetKeyBytes());

    public void StoreKey(string key)
    {
        _stored = key;
    }

    public byte[] GetKeyBytes()
    {
        var src = Encoding.UTF8.GetBytes((_stored ?? "") + PEPPER);
        using (var sha = SHA256.Create()) return sha.ComputeHash(src);
    }
}