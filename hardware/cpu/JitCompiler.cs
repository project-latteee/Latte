using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WiiEmulate.hardware.cpu
{
    internal class JitCompiler
    {
        private readonly Dictionary<uint, IntPtr> compiledBlocks; // Cache of compiled blocks.

        public JitCompiler()
        {
            compiledBlocks = new Dictionary<uint, IntPtr>();
        }

        public bool TryGetCompiledBlock(uint address, out IntPtr compiledBlock)
        {
            return compiledBlocks.TryGetValue(address, out compiledBlock);
        }

        public IntPtr Compile(uint[] instructions)
        {
            // Allocate executable memory
            int codeSize = instructions.Length * 16; // Estimated size per instruction.
            IntPtr codePtr = AllocateExecutableMemory(codeSize);

            // Translate instructions to x86 machine code
            byte[] machineCode = TranslateToMachineCode(instructions);

            // Copy the machine code to executable memory
            Marshal.Copy(machineCode, 0, codePtr, machineCode.Length);

            return codePtr;
        }

        public void CacheCompiledBlock(uint address, IntPtr compiledBlock)
        {
            if (!compiledBlocks.ContainsKey(address))
            {
                compiledBlocks[address] = compiledBlock;
            }
        }

        private byte[] TranslateToMachineCode(uint[] instructions)
        {
            List<byte> machineCode = new List<byte>();

            foreach (uint instruction in instructions)
            {
                // Decode PowerPC instruction
                uint opcode = (instruction >> 26) & 0x3F; // Extract the opcode (bits 26-31)
                uint rs = (instruction >> 21) & 0x1F;     // Source register (bits 21-25)
                uint rt = (instruction >> 16) & 0x1F;     // Target register (bits 16-20)
                uint rd = (instruction >> 11) & 0x1F;     // Destination register (bits 11-15)
                uint imm = instruction & 0xFFFF;          // Immediate value (bits 0-15)

                // Translate PowerPC instruction to x86
                byte[] translatedCode;
                switch (opcode)
                {
                    case 0x1F: // ADD (example)
                        // x86 equivalent: add eax, ebx
                        translatedCode = new byte[] { 0x01, 0xD8 }; // add eax, ebx
                        break;

                    case 0x1E: // SUB (example)
                        // x86 equivalent: sub eax, ebx
                        translatedCode = new byte[] { 0x29, 0xD8 }; // sub eax, ebx
                        break;

                    case 0x18: // OR (example)
                        // x86 equivalent: or eax, ebx
                        translatedCode = new byte[] { 0x09, 0xD8 }; // or eax, ebx
                        break;

                    case 0x12: // Branch (example)
                        // x86 equivalent: jmp <address>
                        translatedCode = new byte[] { 0xE9 }; // jmp opcode
                        translatedCode = CombineArrays(translatedCode, BitConverter.GetBytes(imm)); // relative offset
                        break;

                    case 0x20: // ADD (example)
                        // x86 equivalent: add eax, ebx
                        translatedCode = new byte[] { 0x01, 0xD8 }; // add eax, ebx
                        break;

                    case 0x24: // SUB (example)
                        // x86 equivalent: sub eax, ebx
                        translatedCode = new byte[] { 0x29, 0xD8 }; // sub eax, ebx
                        break;

                    case 0x2A: // AND (example)
                        // x86 equivalent: and eax, ebx
                        translatedCode = new byte[] { 0x21, 0xD8 }; // and eax, ebx
                        break;

                    case 0x1C: // OR (example)
                        // x86 equivalent: or eax, ebx
                        translatedCode = new byte[] { 0x09, 0xD8 }; // or eax, ebx
                        break;

                    case 0x08: // Branch (example)
                        // x86 equivalent: jmp <address>
                        translatedCode = new byte[] { 0xE9 }; // jmp opcode
                        translatedCode = CombineArrays(translatedCode, BitConverter.GetBytes(imm)); // relative offset
                        break;

                    default:
                        // Unsupported instruction: emit a "ret" instruction to terminate execution
                        translatedCode = new byte[] { 0xC3 }; // x86 "ret" instruction
                        break;
                }

                // Log the translated instruction
                Console.WriteLine($"Translated instruction: {BitConverter.ToString(translatedCode)}");

                // Add the translated code to the machine code list
                machineCode.AddRange(translatedCode);
            }

            return machineCode.ToArray();
        }

        private IntPtr AllocateExecutableMemory(int size)
        {
            // Allocate memory with EXECUTE permission
            IntPtr memory = VirtualAlloc(IntPtr.Zero, (uint)size, AllocationType.Commit, MemoryProtection.ExecuteReadWrite);
            if (memory == IntPtr.Zero)
            {
                throw new Exception("Failed to allocate executable memory.");
            }
            return memory;
        }

        public void FreeCompiledBlock(IntPtr codePtr)
        {
            if (codePtr != IntPtr.Zero)
            {
                VirtualFree(codePtr, 0, FreeType.Release);
            }
        }

        public IEnumerable<IntPtr> GetAllCompiledBlocks()
        {
            return compiledBlocks.Values;
        }

        // P/Invoke for memory allocation
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualFree(IntPtr lpAddress, uint dwSize, FreeType dwFreeType);

        [Flags]
        private enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        private enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        [Flags]
        private enum FreeType
        {
            Decommit = 0x4000,
            Release = 0x8000
        }

        private byte[] CombineArrays(byte[] first, byte[] second)
        {
            byte[] result = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, result, 0, first.Length);
            Buffer.BlockCopy(second, 0, result, first.Length, second.Length);
            return result;
        }
    }
}