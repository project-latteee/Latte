using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiiEmulate.hardware.memory
{
    internal class Memory
    {
        private const long MemorySize = 2L * 1024 * 1024 * 1024; // 2GB
        private const int ChunkSize = 1024 * 1024 * 1024; // 1GB per chunk
        private byte[][] memoryChunks;
        private Dictionary<uint, MappedIoRegion> ioRegions;

        public Memory()
        {
            int numChunks = (int)(MemorySize / ChunkSize);
            if (MemorySize % ChunkSize != 0)
            {
                numChunks++;
            }

            memoryChunks = new byte[numChunks][];
            for (int i = 0; i < numChunks; i++)
            {
                memoryChunks[i] = new byte[ChunkSize];
            }

            ioRegions = new Dictionary<uint, MappedIoRegion>();
        }

        private class MappedIoRegion
        {
            public uint StartAddress { get; }
            public uint Size { get; }
            public Func<uint, uint> ReadHandler { get; }
            public Action<uint, uint> WriteHandler { get; }

            public MappedIoRegion(uint startAddress, uint size, Func<uint, uint> readHandler, Action<uint, uint> writeHandler)
            {
                StartAddress = startAddress;
                Size = size;
                ReadHandler = readHandler;
                WriteHandler = writeHandler;
            }
        }
        private bool TryHandleIoRead(uint address, out uint value)
        {
            foreach (var region in ioRegions.Values)
            {
                if (address >= region.StartAddress && address < region.StartAddress + region.Size)
                {
                    value = region.ReadHandler(address);
                    return true;
                }
            }

            value = 0;
            return false;
        }
        private bool TryHandleIoWrite(uint address, uint value)
        {
            foreach (var region in ioRegions.Values)
            {
                if (address >= region.StartAddress && address < region.StartAddress + region.Size)
                {
                    region.WriteHandler(address, value);
                    return true;
                }
            }

            return false;
        }

        public uint ReadUInt32(uint address)
        {
            if (TryHandleIoRead(address, out uint value))
            {
                return value;
            }
            CheckAddress(address, 4);
            return BitConverter.ToUInt32(new byte[]
            {
                GetByte(address),
                GetByte(address + 1),
                GetByte(address + 2),
                GetByte(address + 3)
            }, 0);
        }

        public void WriteUInt32(uint address, uint value)
        {
            if (TryHandleIoWrite(address, value))
            {
                return;
            }
            CheckAddress(address, 4);
            byte[] bytes = BitConverter.GetBytes(value);
            SetByte(address, bytes[0]);
            SetByte(address + 1, bytes[1]);
            SetByte(address + 2, bytes[2]);
            SetByte(address + 3, bytes[3]);
        }

        public byte ReadByte(uint address)
        {
            CheckAddress(address, 1);
            return GetByte(address);
        }

        public void WriteByte(uint address, byte value)
        {
            CheckAddress(address, 1);
            SetByte(address, value);
        }

        private byte GetByte(uint address)
        {
            int chunkIndex = (int)(address / ChunkSize);
            int offset = (int)(address % ChunkSize);
            return memoryChunks[chunkIndex][offset];
        }

        private void SetByte(uint address, byte value)
        {
            int chunkIndex = (int)(address / ChunkSize);
            int offset = (int)(address % ChunkSize);
            memoryChunks[chunkIndex][offset] = value;
        }
        public void Clear()
        {
            foreach (var chunk in memoryChunks)
            {
                Array.Clear(chunk, 0, chunk.Length);
            }
        }

        private void CheckAddress(uint address, int length)
        {
            if (address >= MemorySize || (address + length) > MemorySize)
            {
                throw new IndexOutOfRangeException($"Memory access out of bounds: {address:X}");
            }
        }

        public void MapIoRegion(uint startAddress, uint size, Func<uint, uint> readHandler, Action<uint, uint> writeHandler)
        {
            if (ioRegions.ContainsKey(startAddress))
            {
                throw new ArgumentException($"IO region starting at {startAddress:X} is already mapped.");
            }

            ioRegions[startAddress] = new MappedIoRegion(startAddress, size, readHandler, writeHandler);
        }
    }
}
