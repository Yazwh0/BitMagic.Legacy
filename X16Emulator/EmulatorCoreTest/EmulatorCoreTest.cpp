#include <iostream>
#include <stdlib.h>
#include <stdio.h>
#include <malloc.h>

extern "C" 
{
    struct state 
    {
        uint64_t clock;
        uint16_t pc;
        uint16_t stackpointer;
        uint8_t a;
        uint8_t x;
        uint8_t y;

        bool decimal;
        bool breakFlag;
        bool overflow;
        bool negative;
        bool carry;
        bool zero;
        bool interruptDisable;
        bool interrupt;
    };

    int __fastcall fnEmulatorCode(int8_t* mainMemory, state* test);
}

int main()
{
    std::cout << "BitMagic Emulator Test\n";


    // main memory
    // align to 512 bytes for AVX copy
    void* ptr = _aligned_malloc(64 * 1024, 512);

    if (!ptr)
    {
        std::cout << printf("Could not allocate memory.");
        return -1;
    }

    int8_t* memory_ptr = (int8_t*)ptr;

    struct state state {};

    for (int i = 0; i < 64 * 1024; i++)
        memory_ptr[i] = 0;

    // initiliase machine
    state.a = 0xff;
    state.x = 0x72;
    state.y = 0;
    state.pc = 0x810; // arbitary for now
    state.stackpointer = 0x1ff;
    state.clock = 0x0;
    
    state.decimal = false;
    state.carry = false;
    state.breakFlag = false;
    state.interruptDisable = false;
    state.negative = false;
    state.overflow = false;
    state.zero = false;
    state.interrupt = true;


    memory_ptr[0xfffe] = 0x00;
    memory_ptr[0xffff] = 0x09;

    memory_ptr[0x810] = 0x0f;
    memory_ptr[0x811] = 0x10;
    memory_ptr[0x812] = 0x02;
    memory_ptr[0x813] = 0xdb;
    memory_ptr[0x814] = 0xa9;
    memory_ptr[0x815] = 0x10;
    memory_ptr[0x816] = 0xdb;

    memory_ptr[0x900] = 0x40;


    int x = fnEmulatorCode(memory_ptr, &state); 

    //delete [] memory_ptr;
    _aligned_free(ptr);

    std::cout << printf("Emulator returned %d\n", x);
}

