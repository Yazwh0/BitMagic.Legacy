#include <iostream>

extern "C" 
{
    struct state 
    {
        unsigned int a;
        unsigned int x;
        unsigned int y;
        unsigned int pc;
        unsigned int stackpointer;
        bool decimal;
        bool breakFlag;
        bool overflow;
        bool negative;
        uint64_t clock;

        bool carry;
        bool zero;
        bool interruptDisable;
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
    state.a = 0x40;
    state.x = 0x01;
    state.y = 0;
    state.pc = 0x810; // arbitary for now
    state.stackpointer = 0x1fe;
    state.clock = 0x0;
    
    state.decimal = false;
    state.carry = false;
    state.breakFlag = false;
    state.interruptDisable = false;
    state.negative = true;
    state.overflow = false;
    state.zero = true;

    memory_ptr[0x1ff] = 0x20;

    memory_ptr[0x810] = 0x68;
    memory_ptr[0x811] = 0xdb;
    memory_ptr[0x812] = 0xdb;
    memory_ptr[0x813] = 0xa9;
    memory_ptr[0x814] = 0x10;
    memory_ptr[0x815] = 0xdb;


    int x = fnEmulatorCode(memory_ptr, &state); 

    delete [] memory_ptr;

    std::cout << printf("Emulator returned %d\n", x);
}

