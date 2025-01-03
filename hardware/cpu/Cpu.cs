using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WiiEmulate.hardware.memory;

namespace WiiEmulate.hardware.cpu
{
    internal class Cpu
    {
        // General Purpose Registers (GPRs)
        public uint[] Gpr = new uint[32];

        // Special Purpose Registers (SPRs)
        public uint[] Pc; // Program Counter    
        public uint[] Cr; // Condition Register
        public uint[] Lr; // Link Register
        public uint[] Ctr; // Count Register
        public uint[] Xer; // Exception Register

        // Instruction Fields
        private const uint PRIMARY_OPCODE_MASK = 0xFC000000;
        private const uint SECONDARY_OPCODE_MASK = 0x000007FF;
        private const uint IMMEDIATE_VALUE_MASK = 0x000FFFFF;
        private const int PRIMARY_OPCODE_SHIFT = 26;
        private const int SECONDARY_OPCODE_SHIFT = 11;
        private const int IMMEDIATE_VALUE_SHIFT = 0;

        public byte[] Memory = new byte[1024 * 1024]; // 1 MB

        public Cpu()
        {
            Pc = new uint[1] { 0 };
            Cr = new uint[8]; // Condition Register divided into 8 fields: CR0-CR7, each 4 bits
            Lr = new uint[1] { 0 }; // Link Register stores the return address for function calls
            Ctr = new uint[1] { 0 }; // Count Register is used for loop counting and branching
            Xer = new uint[1] { 0 }; // Exception Register is used to store the exception code
        }

        // Instruction Execution
        public void ExecuteInstruction(uint instruction)
        {
            // Extract primary opcode (bits 0-5)
            uint primaryOpcode = (instruction & PRIMARY_OPCODE_MASK) >> PRIMARY_OPCODE_SHIFT;

            switch (primaryOpcode)
            {
                case 0x0C: // addic - add immediate carrying
                    ExecuteAddic(instruction);
                    break;
                case 0x0E: // addi - add immediate
                    ExecuteAddi(instruction);
                    break;
                case 0x0F: // addis - add immediate shifted
                    ExecuteAddis(instruction);
                    break;
                case 0x10: // bcx - branch conditional
                    ExecuteBranch(instruction);
                    break;
                default:
                    throw new NotImplementedException($"Opcode {primaryOpcode:X2} not implemented");
            }

            // Increment the program counter
            Pc[0] += 4;
        }

        private void ExecuteAddic(uint instruction)
        {
            int rt = (int)((instruction >> 21) & 0x1F);
            int ra = (int)((instruction >> 16) & 0x1F);
            short imm = (short)(instruction & 0xFFFF);

            uint result = Gpr[ra] + (uint)imm;
            Gpr[rt] = result;

            // Set carry bit in XER
            bool carry = (result < Gpr[ra]) || (imm > 0 && result < (uint)imm);
            Xer[0] = (uint)((Xer[0] & ~0x20000000u) | (carry ? 0x20000000u : 0));
        }

        private void ExecuteAddi(uint instruction)
        {
            int rt = (int)((instruction >> 21) & 0x1F);
            int ra = (int)((instruction >> 16) & 0x1F);
            short imm = (short)(instruction & 0xFFFF);

            // If RA=0, then the immediate value is added to 0
            uint operand = (ra == 0) ? 0 : Gpr[ra];
            Gpr[rt] = operand + (uint)imm;
        }

        private void ExecuteAddis(uint instruction)
        {
            int rt = (int)((instruction >> 21) & 0x1F);
            int ra = (int)((instruction >> 16) & 0x1F);
            uint imm = (instruction & 0xFFFF) << 16;

            // If RA=0, then the immediate value is added to 0
            uint operand = (ra == 0) ? 0 : Gpr[ra];
            Gpr[rt] = operand + imm;
        }

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
                Lr[0] = Pc[0] + 4;
            }

            bool conditionMet = EvaluateBranchCondition(bo, bi);
            if (conditionMet)
            {
                if (aa)
                {
                    Pc[0] = (uint)bd;
                }
                else
                {
                    Pc[0] = Pc[0] + (uint)bd;
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
                Ctr[0]--;
            }

            bool ctrOk = !checkCtr || (decrementCtr && Ctr[0] == 0) || (!decrementCtr && Ctr[0] != 0);
            bool condOk = !checkCondition || (crBit == branchIfTrue);

            return ctrOk && condOk;
        }
    }
}
