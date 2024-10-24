using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public class ByteArrayHandler
{
    // 使用 List<byte> 來存儲基準 byte 數據
    public List<byte> BaseByteList { get; private set; } = new List<byte>();

    // 設定初始資料
    public void SetInitialByteList(byte[] initialByteArray)
    {
        BaseByteList.Clear();
        BaseByteList.AddRange(initialByteArray);
        Console.WriteLine("Initial byte array has been set.");
    }

    // 使用 HashSet 來追蹤變更索引，再進行批次更新操作
    public void CompareAndUpdateWithHashSet(byte[] newByteArray)
    {
        if (BaseByteList.Count != newByteArray.Length)
        {
            Console.WriteLine("New byte array length does not match the base byte array length.");
            return;
        }

        // 使用 HashSet 來記錄變更的索引
        HashSet<int> changedIndices = new HashSet<int>();

        // 逐一比對每個索引位置的 byte 值，使用 XOR 判斷是否相同
        for (int i = 0; i < BaseByteList.Count; i++)
        {
            // 使用 XOR 判斷值是否不同（結果為 0 表示相同）
            if ((BaseByteList[i] ^ newByteArray[i]) != 0)
            {
                changedIndices.Add(i); // 記錄變更索引
            }
        }

        // 如果存在變更索引，進行批量更新操作
        if (changedIndices.Count > 0)
        {
            // 批量覆蓋變更的索引
            foreach (int index in changedIndices)
            {
                BaseByteList[index] = newByteArray[index];
                Console.WriteLine($"Updated index {index}: {newByteArray[index]}");
            }

            // 根據變更數量決定是否進行整段更新來提升效能
            if (changedIndices.Count > BaseByteList.Count * 0.1)
            {
                // 若變更數量大於總長度的 10%，使用整段更新
                Console.WriteLine("Large number of changes detected, performing bulk update...");
                Array.Copy(newByteArray, BaseByteList.ToArray(), newByteArray.Length);
            }

            Console.WriteLine("Base byte array has been partially updated with HashSet tracking.");
        }
        else
        {
            Console.WriteLine("No changes detected, base byte array remains the same.");
        }
    }

    // 打印 BaseByteList 中的所有 byte 值 
    public void PrintByteList()
    {
        for (int i = 0; i < BaseByteList.Count; i++)
        {
            Console.WriteLine($"Index {i}: {BaseByteList[i]}");
        }
    }
}



