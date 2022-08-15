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
	struct vera_state
	{
		int8_t* vram_ptr;
		int8_t data0_address;
		int8_t data1_address;
		int8_t data0_step;
		int8_t data1_step;
	};

	struct state 
	{
		int8_t* memory_ptr;
		int8_t* rom_ptr;
		int8_t* rambank_ptr;
		vera_state* vera_ptr;

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

	EMULATORCODE_API int fnEmulatorCode(state* state);
}