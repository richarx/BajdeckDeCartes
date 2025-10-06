using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Conversion
{
    public class Data
    {
        public byte Number;
        public byte Quality;
        public byte Wear;
        public ushort UUID;
    }
    private static Save _save = null;
    private const int HMAC_TRUNC_BYTES = 8;
    private const string PLAYER_PREF_KEY = "TheHorseTookAllTheRocks";

    public static bool IsAllowed(string code)
    {
        if (_save == null)
        {
            _save = SaveBase.Load<Save>();
        }
        return !_save.excludedCodes.Contains(code);
    }

    public static void ExcludeCode(string code)
    {
        if (_save == null)
        {
            _save = SaveBase.Load<Save>();
        }
        if (!_save.excludedCodes.Contains(code))
        {
            _save.excludedCodes.Add(code);
            _save.Save();
        }
    }

    public static string ToCode(CardInstance card, string key)
    {
        if (card.Data == null)
        {
            Debug.LogWarning("CardInstance.ToCode: missing CardData");
            return null;
        }

        byte number = (byte)card.Data.Number;
        byte quality = (byte)card.Quality;
        byte wear = (byte)Mathf.Clamp(card.WearLevel, 0, 255);
        ushort uuid = card.UUID;

        // Données brutes (sans version)
        byte[] buf = new byte[1 + 1 + 1 + 2 + HMAC_TRUNC_BYTES];
        int i = 0;
        buf[i++] = number;
        buf[i++] = quality;
        buf[i++] = wear;
        buf[i++] = (byte)(uuid >> 8);
        buf[i++] = (byte)(uuid & 0xFF);

        // HMAC
        byte[] hmac = ComputeHmac(buf, 0, 5, key);
        Array.Copy(hmac, 0, buf, i, HMAC_TRUNC_BYTES);

        // Encode en base64-url
        return Convert.ToBase64String(buf)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public static Data FromCode(string code, string key)
    {
        try
        {
            Data data = new Data();
            // decode base64-url
            string padded = code.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }

            byte[] buf = Convert.FromBase64String(padded);
            if (buf.Length < 5 + HMAC_TRUNC_BYTES)
                throw new Exception("Invalid code length");

            int i = 0;
            data.Number = buf[i++];
            data.Quality = buf[i++];
            data.Wear = buf[i++];
            data.UUID = (ushort)((buf[i++] << 8) | buf[i++]);

            // vérification HMAC
            byte[] expected = ComputeHmac(buf, 0, 5, key);
            for (int j = 0; j < HMAC_TRUNC_BYTES; j++)
            {
                if (buf[i + j] != expected[j])
                    throw new Exception("Invalid signature");
            }

            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"FromCode failed: {ex.Message}");
        }
        return null;
    }

    private static byte[] ComputeHmac(byte[] data, int offset, int count, string key)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        {
            byte[] hash = hmac.ComputeHash(data, offset, count);
            byte[] truncated = new byte[HMAC_TRUNC_BYTES];
            Array.Copy(hash, truncated, HMAC_TRUNC_BYTES);
            return truncated;
        }
    }

    [Serializable]
    private class Save : SaveBase
    {
        [SerializeField]
        public List<string> excludedCodes = new();
        protected override string PrefKey => "TheHorseTookAllTheRocks";
    }

}
