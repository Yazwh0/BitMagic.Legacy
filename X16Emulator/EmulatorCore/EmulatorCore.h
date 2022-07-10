// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the EMULATORCODE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// EMULATORCODE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef EMULATORCODE_EXPORTS
#define EMULATORCODE_API __declspec(dllexport)
#else
#define EMULATORCODE_API __declspec(dllimport)
#endif
#include <cstdint>

// This class is exported from the dll
//class EMULATORCODE_API CEmulatorCode {
//public:
//	CEmulatorCode(void);
//	int TestFunc();
//	// TODO: add your methods here.
//};


//extern EMULATORCODE_API int nEmulatorCode;

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

	EMULATORCODE_API int fnEmulatorCode(int8_t* mainMemory, state* state);
}