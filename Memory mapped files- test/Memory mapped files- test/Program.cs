﻿using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace A
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "my string";
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew("testmap", 10000))
            {
                bool mutexCreated;
                Mutex mutex = new Mutex(true, "testmapmutex", out mutexCreated);
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(str);
                }
                mutex.ReleaseMutex();

                Console.WriteLine("Start Process B and press ENTER to continue.");
                Console.ReadLine();

                Console.WriteLine("Start Process C and press ENTER to continue.");
                Console.ReadLine();

                mutex.WaitOne();
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    BinaryReader reader = new BinaryReader(stream);
                    var variable = reader.ReadString();
                    Console.WriteLine("Process A says: {0}",variable );
                    Console.WriteLine("Process B says: {0}", reader.ReadBoolean());
                    Console.WriteLine("Process C says: {0}", reader.ReadBoolean());
                }
                mutex.ReleaseMutex();
            }
        }
    }
}
