using System;
using System.Collections.Generic;

namespace WiiEmulate.hardware.cpu
{
    internal class Cpu
    {
        // General Purpose Registers (GPRs)
        public uint[] Gpr = new uint[32];

        // Special Purpose Registers (SPRs)
        public uint Msr; // Machine State Register
        public Dictionary<uint, uint> Spr = new Dictionary<uint, uint>();

        // Program Counter, Link Register, Count Register, etc.
        public uint Pc; // Program Counter
        public uint Lr; // Link Register
        public uint Ctr; // Count Register
        public uint Xer; // Fixed-Point Exception Register

        // Condition Register (CR)
        public uint[] Cr = new uint[8]; // CR0-CR7, each 4 bits

        // Memory
        public byte[] Memory = new byte[1024 * 1024]; // 1 MB

        public Cpu()
        {
            // Initialize SPRs
            InitializeSprs();

            // Initialize registers
            Pc = 0;
            Lr = 0;
            Ctr = 0;
            Xer = 0;
            Msr = 0;
        }

        // https://wiiubrew.org/wiki/Hardware/Espresso
        private void InitializeSprs()
        {
            // Initialize SPRs with default values
            Spr[0x1] = 0; // XER
            Spr[0x8] = 0; // LR
            Spr[0x9] = 0; // CTR
            Spr[0x12] = 0; // DSISR
            Spr[0x13] = 0; // DAR
            Spr[0x16] = 0; // DEC
            Spr[0x19] = 0; // SDR1
            Spr[0x1A] = 0; // SRR0
            Spr[0x1B] = 0; // SRR1
            Spr[0x10C] = 0; // UTBL
            Spr[0x10D] = 0; // UTBU
            Spr[0x110] = 0; // SPRG0
            Spr[0x111] = 0; // SPRG1
            Spr[0x112] = 0; // SPRG2
            Spr[0x113] = 0; // SPRG3
            Spr[0x11A] = 0; // EAR
            Spr[0x11C] = 0; // TBL
            Spr[0x11D] = 0; // TBU
            Spr[0x11F] = 0; // PVR
            Spr[0x210] = 0; // IBAT0U
            Spr[0x211] = 0; // IBAT0L
            Spr[0x212] = 0; // IBAT1U
            Spr[0x213] = 0; // IBAT1L
            Spr[0x214] = 0; // IBAT2U
            Spr[0x215] = 0; // IBAT2L
            Spr[0x216] = 0; // IBAT3U
            Spr[0x217] = 0; // IBAT3L
            Spr[0x218] = 0; // DBAT0U
            Spr[0x219] = 0; // DBAT0L
            Spr[0x21A] = 0; // DBAT1U
            Spr[0x21B] = 0; // DBAT1L
            Spr[0x21C] = 0; // DBAT2U
            Spr[0x21D] = 0; // DBAT2L
            Spr[0x21E] = 0; // DBAT3U
            Spr[0x21F] = 0; // DBAT3L
            Spr[0x230] = 0; // IBAT4U
            Spr[0x231] = 0; // IBAT4L
            Spr[0x232] = 0; // IBAT5U
            Spr[0x233] = 0; // IBAT5L
            Spr[0x234] = 0; // IBAT6U
            Spr[0x235] = 0; // IBAT6L
            Spr[0x236] = 0; // IBAT7U
            Spr[0x237] = 0; // IBAT7L
            Spr[0x238] = 0; // DBAT4U
            Spr[0x239] = 0; // DBAT4L
            Spr[0x23A] = 0; // DBAT5U
            Spr[0x23B] = 0; // DBAT5L
            Spr[0x23C] = 0; // DBAT6U
            Spr[0x23D] = 0; // DBAT6L
            Spr[0x23E] = 0; // DBAT7U
            Spr[0x23F] = 0; // DBAT7L
            Spr[0x380] = 0; // UGQR0
            Spr[0x381] = 0; // UGQR1
            Spr[0x382] = 0; // UGQR2
            Spr[0x383] = 0; // UGQR3
            Spr[0x384] = 0; // UGQR4
            Spr[0x385] = 0; // UGQR5
            Spr[0x386] = 0; // UGQR6
            Spr[0x387] = 0; // UGQR7
            Spr[0x388] = 0; // UHID2
            Spr[0x389] = 0; // UWPAR
            Spr[0x38A] = 0; // UDMAU
            Spr[0x38B] = 0; // UDMAL
            Spr[0x390] = 0; // GQR0
            Spr[0x391] = 0; // GQR1
            Spr[0x392] = 0; // GQR2
            Spr[0x393] = 0; // GQR3
            Spr[0x394] = 0; // GQR4
            Spr[0x395] = 0; // GQR5
            Spr[0x396] = 0; // GQR6
            Spr[0x397] = 0; // GQR7
            Spr[0x398] = 0; // HID2
            Spr[0x399] = 0; // WPAR
            Spr[0x39A] = 0; // DMA_U
            Spr[0x39B] = 0; // DMA-l
            Spr[0x3A8] = 0; // UMMCR0
            Spr[0x3A9] = 0; // UPMC1
            Spr[0x3AA] = 0; // UPMC2
            Spr[0x3AB] = 0; // USIA
            Spr[0x3AC] = 0; // UMMCR1
            Spr[0x3AD] = 0; // UPMC3
            Spr[0x3AE] = 0; // UPMC4
            Spr[0x3B0] = 0; // HID5
            Spr[0x3B2] = 0; // PCSR
            Spr[0x3B3] = 0; // SCR
            Spr[0x3B4] = 0; // CAR
            Spr[0x3B5] = 0; // BCR
            Spr[0x3B6] = 0; // WPSAR
            Spr[0x3B8] = 0; // MMCR0
            Spr[0x3B9] = 0; // PMC1
            Spr[0x3BA] = 0; // PMC2
            Spr[0x3BB] = 0; // SIA
            Spr[0x3BC] = 0; // MMCR1
            Spr[0x3BD] = 0; // PMC3
            Spr[0x3BE] = 0; // PMC4
            Spr[0x3D0] = 0; // DCATE
            Spr[0x3D1] = 0; // DCATR
            Spr[0x3D8] = 0; // DMATL0
            Spr[0x3D9] = 0; // DMATU0
            Spr[0x3DA] = 0; // DMATR0
            Spr[0x3DB] = 0; // DMATL1
            Spr[0x3DC] = 0; // DMATU1
            Spr[0x3DD] = 0; // DMATR1
            Spr[0x3EF] = 0; // PIR
            Spr[0x3F0] = 0; // HID0
            Spr[0x3F1] = 0; // HID1
            Spr[0x3F2] = 0; // IABR
            Spr[0x3F3] = 0; // HID4
            Spr[0x3F4] = 0; // TDCL
            Spr[0x3F5] = 0; // DABR
            Spr[0x3F9] = 0; // L2CR
            Spr[0x3FA] = 0; // TDCH
            Spr[0x3FB] = 0; // ICTC
            Spr[0x3FC] = 0; // THRM1
            Spr[0x3FD] = 0; // THRM2
            Spr[0x3FE] = 0; // THRM3
        }

        // Helper methods to read/write SPRs
        public uint ReadSpr(uint index)
        {
            if (Spr.ContainsKey(index))
                return Spr[index];
            throw new ArgumentException($"SPR index {index:X} not found.");
        }

        public void WriteSpr(uint index, uint value)
        {
            if (Spr.ContainsKey(index))
                Spr[index] = value;
            else
                throw new ArgumentException($"SPR index {index:X} not found.");
        }

        // MSR Bit Manipulation
        public bool IsLittleEndian() => (Msr & 0x1) != 0;
        public void SetLittleEndian(bool enable) => Msr = enable ? (Msr | 0x1) : (Msr & ~0x1u);

        public bool IsExternalInterruptEnabled() => (Msr & 0x8000) != 0;
        public void SetExternalInterruptEnable(bool enable) => Msr = enable ? (Msr | 0x8000) : (Msr & ~0x8000u);

        // Instruction Execution
        public void ExecuteInstruction(uint instruction)
        {
            uint primaryOpcode = (instruction >> 26) & 0x3F;

            switch (primaryOpcode)
            {
                case 0x0C: ExecuteAddic(instruction); break;
                case 0x0E: ExecuteAddi(instruction); break;
                case 0x0F: ExecuteAddis(instruction); break;
                case 0x10: ExecuteBranch(instruction); break;
                case 0x1F: ExecuteExtendedOpcode(instruction); break;
                case 0x20: ExecuteLwz(instruction); break;
                case 0x21: ExecuteStw(instruction); break;
                case 0x18: ExecuteB(instruction); break;
                case 0x19: ExecuteBl(instruction); break;
                default: throw new NotImplementedException($"Opcode {primaryOpcode:X} not implemented");
            }

            Pc += 4; // Increment program counter
        }

        // Integer Arithmetic Instructions
        private void ExecuteAddic(uint instruction)
        {
            int rt = (int)((instruction >> 21) & 0x1F);
            int ra = (int)((instruction >> 16) & 0x1F);
            short imm = (short)(instruction & 0xFFFF);

            uint result = Gpr[ra] + (uint)imm;
            Gpr[rt] = result;

            // Set carry bit in XER
            bool carry = (result < Gpr[ra]) || (imm > 0 && result < (uint)imm);
            Xer = (uint)((Xer & ~0x20000000u) | (carry ? 0x20000000u : 0));
        }

        private void ExecuteAddi(uint instruction)
        {
            int rt = (int)((instruction >> 21) & 0x1F);
            int ra = (int)((instruction >> 16) & 0x1F);
            short imm = (short)(instruction & 0xFFFF);

            uint operand = (ra == 0) ? 0 : Gpr[ra];
            Gpr[rt] = operand + (uint)imm;
        }

        private void ExecuteAddis(uint instruction)
        {
            int rt = (int)((instruction >> 21) & 0x1F);
            int ra = (int)((instruction >> 16) & 0x1F);
            uint imm = (instruction & 0xFFFF) << 16;

            uint operand = (ra == 0) ? 0 : Gpr[ra];
            Gpr[rt] = operand + imm;
        }

        // Branch Instructions
        private void ExecuteBranch(uint instruction)
        {
            int bo = (int)((instruction >> 21) & 0x1F);
            int bi = (int)((instruction >> 16) & 0x1F);
            short bd = (short)(instruction & 0xFFFC); // Mask out last 2 bits
            bool lk = (instruction & 0x1) == 1;
            bool aa = (instruction & 0x2) == 2;

            // Save return address if LK bit is set
            if (lk)
            {
                Lr = Pc + 4;
            }

            bool conditionMet = EvaluateBranchCondition(bo, bi);
            if (conditionMet)
            {
                if (aa)
                {
                    Pc = (uint)bd;
                }
                else
                {
                    Pc += (uint)bd;
                }
            }
        }

        private bool EvaluateBranchCondition(int bo, int bi)
        {
            bool crBit = ((Cr[bi >> 2] >> (3 - (bi & 3))) & 1) == 1;

            // Decode BO field
            bool decrementCtr = (bo & 0x04) == 0;
            bool checkCtr = (bo & 0x02) == 0;
            bool checkCondition = (bo & 0x10) == 0;
            bool branchIfTrue = (bo & 0x08) != 0;

            if (decrementCtr)
            {
                Ctr--;
            }

            bool ctrOk = !checkCtr || (decrementCtr && Ctr == 0) || (!decrementCtr && Ctr != 0);
            bool condOk = !checkCondition || (crBit == branchIfTrue);

            return ctrOk && condOk;
        }

        // Load/Store Instructions
        private void ExecuteLwz(uint instruction)
        {
            int rt = (int)((instruction >> 21) & 0x1F);
            int ra = (int)((instruction >> 16) & 0x1F);
            short offset = (short)(instruction & 0xFFFF);

            uint address = (ra == 0) ? (uint)offset : Gpr[ra] + (uint)offset;
            Gpr[rt] = ReadMemory(address);
        }

        private void ExecuteStw(uint instruction)
        {
            int rs = (int)((instruction >> 21) & 0x1F);
            int ra = (int)((instruction >> 16) & 0x1F);
            short offset = (short)(instruction & 0xFFFF);

            uint address = (ra == 0) ? (uint)offset : Gpr[ra] + (uint)offset;
            WriteMemory(address, Gpr[rs]);
        }

        // Extended Opcode Instructions (e.g., mtspr, mfspr)
        private void ExecuteExtendedOpcode(uint instruction)
        {
            uint secondaryOpcode = instruction & 0x7FF;

            switch (secondaryOpcode)
            {
                case 0x1D3: ExecuteMtspr(instruction); break; // mtspr
                case 0x1D2: ExecuteMfspr(instruction); break; // mfspr
                default: throw new NotImplementedException($"Secondary opcode {secondaryOpcode:X} not implemented");
            }
        }

        private void ExecuteMtspr(uint instruction)
        {
            int spr = (int)(((instruction >> 16) & 0x1F) | ((instruction >> 6) & 0x3E0));
            int rs = (int)((instruction >> 21) & 0x1F);

            WriteSpr((uint)spr, Gpr[rs]);
        }

        private void ExecuteMfspr(uint instruction)
        {
            int spr = (int)(((instruction >> 16) & 0x1F) | ((instruction >> 6) & 0x3E0));
            int rt = (int)((instruction >> 21) & 0x1F);

            Gpr[rt] = ReadSpr((uint)spr);
        }

        public void ExecuteB(uint instruction)
        {
            // Extract the LI field (24-bit signed immediate)
            int li = (int)(instruction & 0x03FFFFFC); // Mask out the last 2 bits
            li <<= 6; // Shift left by 6 to get the full 24-bit signed value
            li >>= 6; // Sign-extend the value
            
            // Extract the AA bit (Absolute Address)
            bool aa = (instruction & 0x2) != 0;
            
            // Calculate the target address
            uint targetAddress;
            if (aa)
            {
                // If AA = 1, the target address is the absolute address (LI)
                targetAddress = (uint)li;
            }
            else
            {
                // If AA = 0, the target address is relative to the current PC
                targetAddress = Pc + (uint)li;
            }
            // Update the program counter to the target address
            Pc = targetAddress;
        }

        public void ExecuteBl(uint instruction)
        {
            // Extract the LI field (24-bit signed immediate)
            int li = (int)(instruction & 0x03FFFFFC); // Mask out the last 2 bits
            li <<= 6; // Shift left by 6 to get the full 24-bit signed value
            li >>= 6; // Sign-extend the value

            // Extract the AA bit (Absolute Address)
            bool aa = (instruction & 0x2) != 0;

            // Save the return address in the Link Register (LR)
            Lr = Pc + 4; // The address of the next instruction
            
            // Calculate the target address
            uint targetAddress;
            if (aa)
            {
                // If AA = 1, the target address is the absolute address (LI)
                targetAddress = (uint)li;
            }
            else
            {
                // If AA = 0, the target address is relative to the current PC
                targetAddress = Pc + (uint)li;
            }

            // Update the program counter to the target address
            Pc = targetAddress;
        }

        // Memory Access Helpers
        private uint ReadMemory(uint address)
        {
            if (address >= Memory.Length)
                throw new ArgumentException($"Invalid memory address: {address:X}");
            return BitConverter.ToUInt32(Memory, (int)address);
        }

        private void WriteMemory(uint address, uint value)
        {
            if (address >= Memory.Length)
                throw new ArgumentException($"Invalid memory address: {address:X}");
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, Memory, (int)address, bytes.Length);
        }
    }
}