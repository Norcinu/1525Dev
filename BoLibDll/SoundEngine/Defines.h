/*
	Defines.h
	04/05/12
	Global Definitions that need to be outside game class
*/
#include <Windows.h>

#define SCREEN1 0
#define SCREEN2 1
#define MAX_SCREENS 2

#define BMP_PATH "D:\\1200\\BMP\\"
#define X_PATH "D:\\1200\\X\\"
#define WAV_PATH "D:\\1200\\WAV\\"
#define FX_PATH "D:\\1200\\FX\\"

#ifdef _DEBUG_BUILD
	#define DATA_PATH "D:\\1200\\DATABACKUP\\"
#else
	#define DATA_PATH "D:\\1200\\DATA\\"
#endif

#define VIDEO_PATH "D:\\1200\\VIDEO\\"

//#define MODEL_NUMBER 1200
#define RELEASE_NUMBER 1

#define MINIMUM_BET 100
#define TOTAL_STAKES 1

#define MIN_TRANSFER 10

#define BASE_PERCENTAGE (float)90.1f

#define PublicKey 976458532

#define TOTAL_WINNING_LINES 10

#ifdef _AUTOPLAY
	#define SOAK_BUILD
	#define WORKING_CAPITAL_LOG
//	#define FAST_PLAY
//	#define FIXED_POP 100
//	#define SUPER_FAST_PLAY
//	#define TEST_REEL_TABLES
//	#define NMI_LOG
//	#define RECORD_NMI_LOG			0	//do not record to speed things up
//	#define TEST_DUALPOP			0	
#endif

#ifdef _IN_HOUSE_LOG
#define IN_HOUSE_LOG
#endif