﻿using System.Text;
using Lab01GUI.Services.Interfaces;

namespace Lab01GUI.Services.Implementation;
public class MD5Service : IMD5Service
{
    // Constants
    private const int BlockSize = 64; // 512 bits
    private const int PaddingModLength = 56; // BlockSize - 8 bytes for length

    // Round shift values
    private static readonly int[] S =
    [
	    7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
        5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
        4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
        6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21
    ];

    // Constant K Values
    private static readonly uint[] K =
    [
	    0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
        0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
        0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
        0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
        0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
        0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
        0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
        0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
        0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
        0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
        0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
        0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
        0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
        0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
        0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
        0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
    ];

    public byte[] GetHash(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        return GetHash(inputBytes);
    }

    public byte[] GetHash(byte[] input)
    {
        uint[] h = [0x67452301, 0xefcdab89, 0x98badcfe, 0x10325476];

        byte[] paddedInput = PadInput(input);
        for (int i = 0; i < paddedInput.Length / BlockSize; ++i)
        {
            uint[] M = new uint[16];
            Buffer.BlockCopy(paddedInput, i * BlockSize, M, 0, BlockSize);

            uint[] currentHash = (uint[])h.Clone();
            ProcessChunk(M, currentHash);

            for (int j = 0; j < 4; j++)
            {
                h[j] += currentHash[j];
            }
        }

        return UintArrayToByteArray(h);
    }

    public string GetHashString(string input) => 
        BitConverter.ToString(GetHash(input)).Replace("-", "");

    public string GetHashString(byte[] input) =>
	    BitConverter.ToString(GetHash(input)).Replace("-", "");

    private static byte[] UintArrayToByteArray(uint[] uintArray)
    {
	    byte[] byteArray = new byte[uintArray.Length * sizeof(uint)];

	    for (int i = 0; i < uintArray.Length; i++)
	    {
		    BitConverter.GetBytes(uintArray[i]).CopyTo(byteArray, i * sizeof(uint));
	    }

	    return byteArray;
    }

    private static byte[] PadInput(byte[] input)
    {
        List<byte> padded = [..input, 0x80];
        while (padded.Count % BlockSize != PaddingModLength)
        {
            padded.Add(0x00);
        }

        padded.AddRange(BitConverter.GetBytes((long)input.Length * 8));

        return padded.ToArray();
    }

    private static void ProcessChunk(uint[] M, uint[] h)
    {
        uint A = h[0], B = h[1], C = h[2], D = h[3];

        for (uint k = 0; k < BlockSize; ++k)
        {
            uint F = 0, g = 0;
            if (k < 16)
            {
                F = (B & C) | (~B & D);
                g = k;
            }
            else if (k < 32)
            {
                F = (D & B) | (~D & C);
                g = (5 * k + 1) % 16;
            }
            else if (k < 48)
            {
                F = B ^ C ^ D;
                g = (3 * k + 5) % 16;
            }
            else
            {
                F = C ^ (B | ~D);
                g = 7 * k % 16;
            }

            uint temp = D;
            D = C;
            C = B;
            B += LeftRotate(A + F + K[k] + M[g], S[k]);
            A = temp;
        }

        h[0] = A;
        h[1] = B;
        h[2] = C;
        h[3] = D;
    }

    private static uint LeftRotate(uint x, int c)
    {
        return (x << c) | (x >> (32 - c));
    }
}