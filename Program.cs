using System;
using System.Runtime.InteropServices;
using WiiEmulate.hardware.cpu;
using WiiEmulate.hardware.memory;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Initialize CPU and memory
            var cpu = new Cpu();
            var memory = new Memory();

            // Example GPU I/O region: 0xC0000000 to 0xC0000FFF
            memory.MapIoRegion(0xC0000000, 0x1000,
                address => {
                    Console.WriteLine($"Read from GPU at {address:X}");
                    return 0xDEADBEEF; // Example read value
                },
                (address, value) => {
                    Console.WriteLine($"Write to GPU at {address:X}, value: {value:X}");
                }
            );

            // Read and write to the region
            uint value = memory.ReadUInt32(0xC0000000); // Triggers readHandler
            memory.WriteUInt32(0xC0000004, 0x12345678); // Triggers writeHandler

            // Test JIT Compiler
            TestJitCompiler();
        }
        catch (AccessViolationException ex)
        {
            Console.WriteLine($"Access violation error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void TestJitCompiler()
    {
        // Initialize JIT Compiler
        var jitCompiler = new JitCompiler();

        // Example PowerPC instructions (32-bit opcodes)
        uint[] instructions = {
            0x7C632214, // ADD r3, r3, r4 (PowerPC instruction)
            0x7C6322D4, // SUB r3, r3, r4 (PowerPC instruction)
            0x7C632038, // AND r3, r3, r4 (PowerPC instruction)
            0x7C632378, // OR r3, r3, r4 (PowerPC instruction)
            0x48000000  // B 0x0 (Branch to address 0x0, example)
        };

        // Compile the instructions
        IntPtr compiledBlock = jitCompiler.Compile(instructions);

        // Cache the compiled block (optional, for demonstration)
        jitCompiler.CacheCompiledBlock(0x1000, compiledBlock);

        // Execute the compiled block
        Console.WriteLine("Executing compiled block...");
        ExecuteCompiledBlock(compiledBlock);

        // Free the compiled block
        jitCompiler.FreeCompiledBlock(compiledBlock);
    }

    static void ExecuteCompiledBlock(IntPtr compiledBlock)
    {
        if (compiledBlock == IntPtr.Zero)
        {
            throw new ArgumentException("Compiled block pointer is null.");
        }

        // Use a delegate to execute the compiled code
        var func = Marshal.GetDelegateForFunctionPointer<Action>(compiledBlock);

        try
        {
            Console.WriteLine("Executing compiled function...");
            func(); // Execute the compiled code
        }
        catch (AccessViolationException ex)
        {
            Console.WriteLine($"Access violation during execution: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during execution: {ex.Message}");
        }
    }
}