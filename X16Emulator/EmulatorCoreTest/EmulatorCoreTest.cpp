#include <iostream>

extern "C" 
{
    struct state 
    {
        int a;
        int x;
        int y;
        int pc;
        int clock;

        bool carry;
        bool zero;
        bool interruptDisable;
        bool decimal;
        bool breakFlag;
        bool overflow;
        bool negative;
    };

    int __fastcall fnEmulatorCode(int8_t* mainMemory, state* test);
}

int main()
{
    std::cout << "BitMagic Emulator Test\n";

    // main memory
    int8_t* memory_ptr = new int8_t[64 * 1024];
    struct state state;

    for (int i = 0; i < 64 * 1024; i++)
        memory_ptr[i] = 0;

    // initiliase machine
    state.a = 0;
    state.x = 0x10;
    state.y = 0;
    state.pc = 0x810; // arbitary for now
    state.clock = 0;
    
    state.decimal = false;
    state.carry = false;
    state.breakFlag = false;
    state.interruptDisable = false;
    state.negative = false;
    state.overflow = false;
    state.zero = false;

    memory_ptr[0x810] = 0x81;
    memory_ptr[0x811] = 0x20;
    memory_ptr[0x812] = 0xdb;


    int x = fnEmulatorCode(memory_ptr, &state);

    delete [] memory_ptr;

    std::cout << printf("Emulator returned %d\n", x);
}

