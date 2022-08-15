#include <iostream>
#include <stdlib.h>
#include <stdio.h>
#include <malloc.h>
#include "../EmulatorCore/EmulatorCore.h"

int main()
{
    std::cout << "BitMagic Emulator Test\n";

    // main memory
    //void* m_ptr = _aligned_malloc(64 * 1024, 64);

    //if (!m_ptr)
    //{
    //    std::cout << printf("Could not allocate memory.");
    //    return -1;
    //}

    //auto memory_ptr = (int8_t*)m_ptr;

    //for (int i = 0; i < 64 * 1024; i++)
    //    memory_ptr[i] = 0;

    //// Rom Bank
    //void* r_ptr = _aligned_malloc(0x4000 * 32, 64);

    //if (!r_ptr)
    //{
    //    std::cout << printf("Could not allocate memory for ROM banks.");
    //    return -1;
    //}

    //for (int i = 0; i < 0x4000 * 32; i++)
    //    ((int8_t*)r_ptr)[i] = 0;

    //// Ram Bank
    //void* b_ptr = _aligned_malloc(0x2000 * 256, 64);

    //if (!b_ptr)
    //{
    //    std::cout << printf("Could not allocate memory for RAM banks.");
    //    return -1;
    //}

    //for (int i = 0; i < 0x2000 * 256; i++)
    //    ((int8_t*)b_ptr)[i] = 0;


    struct state state {};
    struct vera_state vera_state {};

    state.memory_ptr = new int8_t[0xa000];
    state.rom_ptr = new int8_t[0x4000 * 32];
    state.rambank_ptr = new int8_t[0x2000 * 256];
    state.vera_ptr = &vera_state;

    vera_state.vram_ptr = new int8_t[0x20000];

    for (int i = 0; i < 0x20000; i++)
        vera_state.vram_ptr[i] = 0;

    for (int i = 0; i < 0xa000; i++)
        state.memory_ptr[i] = 0;

    for (int i = 0; i < 0x2000 * 256; i++)
        state.rambank_ptr[i] = 0;

    // initiliase machine
    state.a = 0x02;
    state.x = 0x02;
    state.y = 0;
    state.pc = 0x810; // arbitary for now
    state.stackpointer = 0x1ff;
    state.clock = 0x0;
    
    state.decimal = false;
    state.carry = true;
    state.breakFlag = false;
    state.interruptDisable = false;
    state.negative = false;
    state.overflow = false;
    state.zero = false;
    state.interrupt = false;

    state.rambank_ptr[0x2000] = 0xff;
    state.rom_ptr[0x0000] = 0x02;

    state.memory_ptr[0x1234] = 0x01;

    state.memory_ptr[0x810] = 0x0c;
    state.memory_ptr[0x811] = 0x34;
    state.memory_ptr[0x812] = 0x12;
    state.memory_ptr[0x813] = 0xdb;
    state.memory_ptr[0x814] = 0x00;
    state.memory_ptr[0x815] = 0x00;
    state.memory_ptr[0x816] = 0x00;
    state.memory_ptr[0x817] = 0x00;

    state.memory_ptr[0x900] = 0x40;


    int x = fnEmulatorCode(& state);

    delete[] state.memory_ptr;
    delete[] state.rom_ptr;
    delete[] state.rambank_ptr;
    delete[] vera_state.vram_ptr;

    //_aligned_free(m_ptr);
    //_aligned_free(r_ptr);
    //_aligned_free(b_ptr);

    std::cout << printf("Emulator returned %d\n", x);
}

