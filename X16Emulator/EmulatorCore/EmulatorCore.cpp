// EmulatorCode.cpp : Defines the exported functions for the DLL.
//

#include "pch.h"
#include "framework.h"
#include "EmulatorCore.h"
#include <cstdint>


// This is an example of an exported variable
//EMULATORCODE_API int nEmulatorCode=0;

extern "C" 
{
    int __fastcall asm_func(state* state);

    // This is an example of an exported function.
    // int8_t* mainMemory,
    EMULATORCODE_API int fnEmulatorCode(state* state)
    {
        return asm_func(state);
     }
}

// This is the constructor of a class that has been exported.
//CEmulatorCode::CEmulatorCode()
//{
//    return;
//}
//
//int CEmulatorCode::TestFunc()
//{
//    
//
//    return 5;
//}