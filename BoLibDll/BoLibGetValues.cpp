#include <sstream>
#include <cstring>
#include <string>
#include <vector>
#include "BoLibGetValues.h"
#include "General.h"
#include "MD5.h"

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);

extern int CoinConv[COIN_CNT][CC_CNT];
extern int NoteValues[NOTE_CNT][CC_CNT];
//extern unsigned long EspRegionalVariableValues[ESP_REGIONS][ESP_VARIABLES];


const std::string MACHINE_INI = "D:\\machine\\tabmachine.ini";
char global_buffer[256] = {0};
std::string country_code_buffer = "";
char item[2]= {0};
char path_buffer[64] = {0};

struct GamesInfo
{
	char *name;
	char *hash_code;
	char *path;
};

// Local Util functions not export to DLL
namespace utils 
{
	template <class T>
	inline std::string to_string (const T& t)
	{
		std::stringstream ss;
		ss << t;
		return ss.str();
	}
	
	template <class T>
	bool FromString(T& t, const std::string& s, std::ios_base& (*f)(std::ios_base&))
	{
		std::istringstream iss(s);
		return !(iss >> f >> t).fail();
	}

	unsigned int GetDigit(const unsigned int n, const unsigned int k) 
	{
		switch(k)
		{
		case 0:return n%10;
		case 1:return n/10%10;
		case 2:return n/100%10;
		case 3:return n/1000%10;
		case 4:return n/10000%10;
		}
		return 0;
	}

	// Combines digits extracted from above function.
	// t = 5 and z = 3, yields the result of 53.
	unsigned int CombineDigits(const unsigned int t, const unsigned int z) 
	{
		unsigned int int1 = t;
		unsigned int int2 = z;
		
		unsigned int temp = int2;
		
		do
		{
			temp /= 10;
			int1 *= 10;
		} while (temp > 0);

		return int1 + int2;
	}
}
///////////////////////////////////////

// Helper defines
#define TO_STR(str) utils::to_string(str)
#define LAST_GAME_FIELDS 0x09
#define GAME_MODEL 1
//

// Functions for export
unsigned int getBoLibVersion()
{
	return BOLIBRELEASE;
}

int getCredit()
{
	return GetCredits();
}

int getBank()
{
	return GetBankDeposit();
}

int addCredit(int pennies)
{
	int toBank = 0;
	int cc = GetCountry();
	int bankTransfer = GetVariableValue(MAX_WBANK_TRANSFER);
	
	if(pennies > bankTransfer)
	{
		toBank = pennies - bankTransfer;
		pennies -= toBank;
		AddToBankDeposit(toBank);
	}
	
	return add_cdeposit(pennies);
}

int getCountryCode()
{
	return GetCountry();
}

int getUkCountryCodeB3()
{
	return CC_UKB3;
}

int getUkCountryCodeC()
{
	return CC_UKC;
}

int getSpainCountryCode()
{
	return CC_ESP;
}

char *getCountryCodeStr()
{
	//SetFileAction();
	char buffer[32] = {0};
	GetPrivateProfileSection("CountryCode", buffer, 32, MACHINE_INI.c_str());
	//ClearFileAction();
	country_code_buffer = "Country Code: ";
	country_code_buffer += buffer[0];
	return (char *)country_code_buffer.c_str();
}

bool isDualBank()
{
	return GetBankAndCreditMeter() ? true : false;
}

int getError()
{
	return GetCurrentError();
}

const char* getErrorText()
{
	return GetErrorText(GetCurrentError());
}

int getMaxCredits()
{
	return (signed)GetVariableValue(MAX_CREDITS);
}

int getMaxBank()
{
	return(signed)GetVariableValue(MAX_WIN_BANK);
}

int getTargetPercentage()
{
	return GetTargetPercentage();
}

int getDoorStatus()
{
	return GetDoorStatus();
}

int refillKeyStatus()
{
	return GetSwitchStatus(REFILL_KEY);
}

std::string ReturnDenom(const int index, const int field, const std::string& pounds)
{
	std::string ret = pounds;
	if (LastGames[index][field] >= 100)
	{
		ret.insert(ret.size()-2, ".");
		ret.insert(0, "£"); // should just handle
	}
	else
	{
		ret.insert(0, "£0.0");
	}
	return std::move(ret);
}

const char *getLastGame(int index)
{
	std::string fields[LAST_GAME_FIELDS] = {" : ", " : ", " : ", " : ", " : ", " : ", " : ", " : ", " : "};
	fields[0] = TO_STR(LastGames[index][0]); // This should be the .raw icon file
	fields[2] = TO_STR(LastGames[index][3]) + ":" + TO_STR(LastGames[index][4]) 
		+ " " + TO_STR(LastGames[index][1]) + "/" + TO_STR(LastGames[index][2]);
	fields[4] = ReturnDenom(index, 5, TO_STR(LastGames[index][5]));
	fields[6] = ReturnDenom(index, 6, TO_STR(LastGames[index][6]));
	fields[8] = ReturnDenom(index, 7, TO_STR(LastGames[index][7]));
	
	std::string game_info = "";
	for (int i = 0; i < LAST_GAME_FIELDS; i++)
		game_info += fields[i];
	
    int field_length = game_info.size()-1;
	char buffer[512] = {0}; 
	
	strncpy_s(buffer, game_info.c_str(), 511);
	return buffer;
}

unsigned long getGameModel(int index)
{
	return ModelNumbers[index];
}

unsigned long getLastGameModel(int index)
{
	return LastGames[index][0];
}

unsigned int getGameTime(int index)
{
				  //       hour							minute
	int result = (LastGames[index][1] << 16) | LastGames[index][2];
	return result;
}

unsigned int getGameDate(int index)
{
				  //		day							 month
	int result = (LastGames[index][3] << 16) | LastGames[index][4];
	return result;
}

unsigned int getGameWager(int index)
{
	return LastGames[index][5];
}

unsigned long getWinningGame(int index)
{
	return LastGames[index][6];
}

unsigned int getGameCreditLevel(int index)
{
	return LastGames[index][7];
}

unsigned long getPerformanceMeter(unsigned char Offset)
{
	return GetPerformanceMeter(Offset);
}

unsigned long getGamePerformanceMeter(unsigned int Offset, unsigned int MeterType)
{
	return GetGamePerformanceMeter(Offset, MeterType);
}

unsigned int getLocalMasterVolume()
{
	return GetLocalMasterVolume();
}

unsigned int getSwitchStatus(unsigned char offset, unsigned char mask)
{
	return GetSwitchStatus(offset, mask);
}

unsigned int getLastNote(int index)
{
	return nvr_main->lastFiveNotes[index];
}

unsigned int *getLastNotes()
{
	return nvr_main->lastFiveNotes;
}

unsigned int getTerminalType()
{
	return GetTerminalType();
}

unsigned int getHopperFloatLevel(unsigned char hopper)
{
	return GetHopperFloatLevel(hopper);
}

unsigned int getHopperDivertLevel(unsigned char hopper)
{
	return GetHopperDivertLevel(hopper);
}

unsigned char getHopperDumpSwitchActive()
{
	return GetHopperDumpSwitchActive();
}

unsigned char getHopperDumpSwitch()
{
	return GetSwitchStatus(HOPPER_DUMP_SW);
}

//DEPRECATE
unsigned int getRequestEmptyLeftHopper()
{
	//return GetRequestEmptyLeftHopper();
	return GetUtilRequestBitState(UTIL_DUMP_HOPPER1_BIT);
}
//DEPRECATE
unsigned int getRequestEmptyRightHopper()
{
	//return GetRequestEmptyRightHopper();
	return GetUtilRequestBitState(UTIL_DUMP_HOPPER2_BIT);
}

unsigned char getBnvType()
{
	return GetBnvType();
}

unsigned int getRecyclerFloatValue()
{
	return GetRecyclerFloatValue();
}

signed int getRefillCtr(unsigned char hopper)
{
	return GetRefillCtr(hopper);
}

unsigned char getLeftHopper()
{
	return LEFTHOPPER;
}

unsigned char getMiddleHopper()
{
	return MIDDLEHOPPER;
}

unsigned char getRightHopper()
{
	return RIGHTHOPPER;
}

unsigned int getMinPayoutValue()
{
	return GetMinPayoutValue();
}

unsigned long getSpecificCoinIn(int meter, int denom)
{
	return 0;
//	return CoinConv[denom][GetCountry()] * GetReconciliationMeter()
}

unsigned long getCashIn(int meter)
{
	auto HelloImJohnnyCash = 0;
	if (meter == MONEY_IN_LT) 
	{
		HelloImJohnnyCash += CoinConv[8][GetCountry()]*GetReconciliationMeter(COIN_8_LT);
		HelloImJohnnyCash += CoinConv[6][GetCountry()]*GetReconciliationMeter(COIN_6_LT);
		HelloImJohnnyCash += CoinConv[1][GetCountry()]*GetReconciliationMeter(COIN_5_LT);
		HelloImJohnnyCash += CoinConv[2][GetCountry()]*GetReconciliationMeter(COIN_4_LT);
		HelloImJohnnyCash += CoinConv[3][GetCountry()]*GetReconciliationMeter(COIN_3_LT);
		HelloImJohnnyCash += CoinConv[4][GetCountry()]*GetReconciliationMeter(COIN_2_LT);
		//HelloImJohnnyCash += CoinConv[7][GetCountry()]*GetReconciliationMeter(COIN_1_LT);

		HelloImJohnnyCash += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_LT);
		HelloImJohnnyCash += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_LT);
		HelloImJohnnyCash += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_LT);
		HelloImJohnnyCash += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_LT);
		HelloImJohnnyCash += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_LT);
		HelloImJohnnyCash += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_LT);

		HelloImJohnnyCash += GetReconciliationMeter(TITO_IN_LT);
	}
	else
	{
		HelloImJohnnyCash += CoinConv[8][GetCountry()]*GetReconciliationMeter(COIN_8_ST);
		HelloImJohnnyCash += CoinConv[6][GetCountry()]*GetReconciliationMeter(COIN_6_ST);
		HelloImJohnnyCash += CoinConv[1][GetCountry()]*GetReconciliationMeter(COIN_5_ST);
		HelloImJohnnyCash += CoinConv[2][GetCountry()]*GetReconciliationMeter(COIN_4_ST);
		HelloImJohnnyCash += CoinConv[3][GetCountry()]*GetReconciliationMeter(COIN_3_ST);
		HelloImJohnnyCash += CoinConv[4][GetCountry()]*GetReconciliationMeter(COIN_2_ST);
		//HelloImJohnnyCash += CoinConv[7][GetCountry()]*GetReconciliationMeter(COIN_1_ST);

		HelloImJohnnyCash += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_ST);
		HelloImJohnnyCash += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_ST);
		HelloImJohnnyCash += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_ST);
		HelloImJohnnyCash += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_ST);
		HelloImJohnnyCash += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_ST);
		HelloImJohnnyCash += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_ST);

		HelloImJohnnyCash += GetReconciliationMeter(TITO_IN_ST);
	}
	return HelloImJohnnyCash;
}

// MONEY_OUT_LT = 1, MONEY_OUT_ST = 8
unsigned long getCashOut(int meter)
{
	return GetPerformanceMeter(meter);
}

unsigned long getNotesIn(int meter)
{
	auto notes = 0;
	if (meter == MONEY_IN_LT)
	{
		notes += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_LT);
		notes += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_LT);
		notes += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_LT);
		notes += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_LT);
		notes += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_LT);
		notes += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_LT);
	}
	else
	{
		notes += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_ST);
		notes += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_ST);
		notes += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_ST);
		notes += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_ST);
		notes += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_ST);
		notes += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_ST);
	}
	return notes;
}

unsigned long getNotesOut(int meter)
{
	auto notes = 0;
	if (meter == MONEY_OUT_LT)
	{
		notes += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_OUT_LT);
		notes += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_OUT_LT);
		notes += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_OUT_LT);
		notes += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_OUT_LT);
		notes += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_OUT_LT);
		notes += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_OUT_LT);
	}
	else
	{	
		notes += NoteValues[5][GetCountry()]*GetReconciliationMeter(NOTE_6_OUT_ST);
		notes += NoteValues[4][GetCountry()]*GetReconciliationMeter(NOTE_5_OUT_ST);
		notes += NoteValues[3][GetCountry()]*GetReconciliationMeter(NOTE_4_OUT_ST);
		notes += NoteValues[2][GetCountry()]*GetReconciliationMeter(NOTE_3_OUT_ST);
		notes += NoteValues[1][GetCountry()]*GetReconciliationMeter(NOTE_2_OUT_ST);
		notes += NoteValues[0][GetCountry()]*GetReconciliationMeter(NOTE_1_OUT_ST);
	}
	return notes;
}

unsigned long getRefillValue(int meter)
{
	if (meter == REFILL_L_LT)
		return (GetReconciliationMeter(REFILL_L_LT)*COINVALUELEFT + 
			GetReconciliationMeter(REFILL_R_LT)*COINVALUERIGHT);
	else
		return (GetReconciliationMeter(REFILL_L_ST)*COINVALUELEFT + 
			GetReconciliationMeter(REFILL_R_ST)*COINVALUERIGHT);

	/*if (meter == REFILL_L_LT)
		return (GetReconciliationMeter(REFILL_L_LT)*GetPayoutCoinValues(COINVALUELEFT) + 
			GetReconciliationMeter(REFILL_R_LT)*GetPayoutCoinValues(COINVALUERIGHT));
	else
		return (GetReconciliationMeter(REFILL_L_ST)*GetPayoutCoinValues(COINVALUELEFT) + 
			GetReconciliationMeter(REFILL_R_ST)*GetPayoutCoinValues(COINVALUERIGHT));*/
}

unsigned long getVtp(int meter)
{
//	if (meter != GAMEWAGERED_LT || meter != GAMEWAGERED_ST)
//		return ULONG_MAX;
	/*if (meter == WAGERED_LT)
		return GetPerformanceMeter(WAGERED_LT);
	else
		return GetPerformanceMeter(WAGERED_ST);*/
	
	unsigned long total = 0;
	for (int i = 01; i < getNumberOfGames(); i++)
	{
		total += nvr_ptr->gamePerformanceMeters[i][meter];
	}
	return total;
}

unsigned long getWon(int meter)
{
	return GetPerformanceMeter(meter);
}

unsigned int getHandPay(int meter)
{
	return GetPerformanceMeter(meter);
}

unsigned long getTicketsPay(int meter)
{
	return GetReconciliationMeter(meter);
}

char *getSerialNumber()
{
	GetPrivateProfileString("Keys", "Serial", "~", global_buffer, 256, MACHINE_INI.c_str());
	std::string pre = "";
	char final[272] = {0};
	strncat_s(final, pre.c_str(), pre.length());
	strncat_s(final, global_buffer, 256);
	return final;
}

char *getEDCTypeStr()
{
	char buffer[64] = {0};
	GetPrivateProfileString("Datapack", "Protocol", "", buffer, 64, MACHINE_INI.c_str());
	auto mversion = atoi(buffer);
	
	if (mversion)
		return "Data/EDC: 1 - On.";
	else
		return "Data/EDC: 0 - Off.";
}

unsigned long getReconciliationMeter(unsigned char offset)
{
	return GetReconciliationMeter(offset);
}

void getMemoryStatus(MEMORYSTATUS *memory)
{
	MEMORYSTATUS mem;
	mem.dwTotalPageFile = sizeof(mem);
	
	GlobalMemoryStatus(&mem);
	memory->dwAvailPageFile = (mem.dwAvailPageFile	/ 1024) / 1024;
	memory->dwAvailPhys		= (mem.dwAvailPhys		/ 1024) / 1024;
	memory->dwAvailVirtual	= (mem.dwAvailVirtual	/ 1024) / 1024;
	memory->dwLength		= (mem.dwLength			/ 1024) / 1024;
	memory->dwMemoryLoad	= (mem.dwMemoryLoad		/ 1024) / 1024;
	memory->dwTotalPageFile = (mem.dwTotalPageFile	/ 1024) / 1024;
	memory->dwTotalPhys		= (mem.dwTotalPhys		/ 1024) / 1024;
	memory->dwTotalVirtual	= (mem.dwTotalVirtual	/ 1024) / 1024;
}

unsigned int getNumberOfGames()
{
	char buffer[64] = {0};
	GetPrivateProfileString("Terminal", "NumberGames", "", buffer, 64, MACHINE_INI.c_str());
	unsigned int value = 0;
	utils::FromString<unsigned int>(value, buffer, std::dec);
	return value;
}

char *ErrorCodes[55] = 
{
	"Unknown error",
	"Comms buf critical full",
	"Comms Barcode error",
	"Comms Back office",
	"Remote RN array empty",
	"Critical MEM Corruption",
	"Compensator Data Reset",
	"Reel Position Data Reset",
	"Credit Data Reset",
	"Bank Data Reset",
	"Printer not found",
	"Printer failure",
	"Printer out of paper",
	"Game Data Reset",
	"Max Win Bank Exceeded",
	"Max Credits Exceeded",
	"Max Win Exceeded",
	"Comms Remote Credit",
	"Comms Send Time Out",
	"Comms Send Link Lost",
	"Comms Send Data Invalid",
	"Comms Fail Open Socket1",
	"Comms Fail Open Socket2",
	"Comms Winsock Wrong Vrn",
	"Comms Rng Slow Fill",
	"Comms BO BarCode Fail",
	"NV LRC Removed",
	"NV Stacker full",
	"NV Safe jam",
	"NV Unsafe jam",
	"NV Fraud attempt",
	"NV Software error",
	"NV Note Rejected",
	"Hopper opto fraud",
	"Left hopper opto fail",
	"Right hopper opto fail",
	"Hopper short payout",
	"Coin denomination wrong",
	"Data pac coms failure",
	"LeftHopper Tamper Detect",
	"RightHopper Tamper Detect",
	"Payout Interrupted",
	"Ticket Print Interrupted",
	"Remote Credit Too Large",
	"NV Command Unknown",
	"NV Parameter Count Wrong",
	"NV Parameter Out Of Range",
	"NV Cant Process Command",
	"NV Software",
	"NV SSP Fail",
	"NV Key Not Set",
	"Data Pac Running Slow",
	"NV Recycler Removed",
	"NV Recycler Emptied",
	""
};

char *getErrorMessage(char *str, int code)
{
	if (code >= 55)
		return "Unknown Error";
	
	auto length = strlen(ErrorCodes[code]) + 1;
	strcpy_s(str, length, ErrorCodes[code]);
	return str;
}

int getUtilsRelease()
{
	return MODEL_NUMBER;
}



unsigned char getRecyclerChannel()
{
	return GetRecyclerChannel();
}

unsigned long getMaxHandPayThreshold()
{
	return GetVariableValue(MAX_HANDPAY_THRESHOLD);
}

unsigned int getCabinetType()
{
	return GetCabinetType();
}

unsigned char combined()
{
	return COMBINED;
}

unsigned char hopper()
{
	return HOPPER;
}

unsigned char printer()
{
	return PRINTER;
}

unsigned char getTerminalFormat()
{
	return GetTerminalFormat();
}

unsigned int getUtilsAdd2CreditValue()
{
	return GetUtilsAdd2CreditValue();
}

unsigned long getReserveCredits()
{
	return 0; //return GetReserveCredits();
}

bool isBackOfficeAvilable()
{
	char str[32];
	DWORD result = GetPrivateProfileString("Server", "Server IP", "", str, 32, MACHINE_INI.c_str());
	if (result == 0) // entry is commented out. back office set.
		return false;
	else			 
		return true;
}

unsigned long getWinningGameMeter(const int offset, const int meter)
{
	return nvr_ptr->winningGames[offset][meter];
}

unsigned long getHistoryLength()
{
	return NUMBER_LAST_GAMES;
}

char *getLicense()
{
	char buffer[128] = {0};
	GetPrivateProfileString("Keys", "License", "", buffer, 128, MACHINE_INI.c_str());
	char bufferthevampireslayer[128] = {0};
	strncat_s(bufferthevampireslayer, buffer, 128);
	return bufferthevampireslayer;
}

char *CountryStrings[10] =
{
	"UK CAT B3", "UK CAT C", "EURO", "Czech", "Argentina", "UK-Reserved", 
	"COLM", "Northern Ireland", "UK Bingo", "Spain"
};

char *getCountryCodeStrLiteral(char *str, int code)
{
	if (code >= 10)
		return "Unknown Region";

	auto length = strlen(CountryStrings[code]) + 1;
	strcpy_s(str, length, CountryStrings[code]);
	return str;
}


unsigned int getBankCreditsReservePtr()
{
	//TODO: DEMO / PLAYER POINTS?
	return (unsigned int)(nvr_ptr->bank1 + nvr_ptr->cd1);// + nvr_ptr->reserveCredits1);
}

bool getIsHopperHopping()
{
	bool temp = GetHoppersRunning();
	return temp;
}

bool getUtilRequestBitState(int whichBit)
{
	return (bool)GetUtilRequestBitState(whichBit);
}

unsigned char getSmartCardGroup()
{
	return GetSmartCardGroup();
}

unsigned char getSmartCardSubGroup()
{
	return GetSmartCardSubGroup();
}

unsigned char getUtilsAccessLevel()
{
	return GetUtilsAccessLevel();
}

bool getUtilDoorAccess()
{
	if (GetUtilsAccessLevel() & UTIL_AC_LEVEL_DOOR)
		return true;
	else
		return false;
}

bool getUtilRefillAccess()
{
	auto sc = nvr_main->smartCardStatus;
	if (GetUtilsAccessLevel() & UTIL_AC_LEVEL_KEY) //0x00000010)
		return true;
	else
		return false;
}

bool getUtilSmartCardAccess(int whichBit) 
{
	if (GetUtilsAccessLevel() & whichBit)
		return true;
	else
		return false;
}

int getLastPayoutType()
{
	return GetLastPayoutType();
}

unsigned char getSmartCardPointsRTP()
{
	return GetSmartCardPointsRTP();
}

unsigned long getPartialCollectValue()
{
	return GetPartialCollectValue();
}

unsigned long getCashMatchMeter(unsigned char offset)
{
	return GetCashMatchMeter(offset);
}

unsigned long getRewardPointMeter(unsigned char offset)
{
	return GetRewardPointMeter(offset);
}

unsigned long getDemoPlayMeter(unsigned char offset)
{
	return GetDemoPlayMeter(offset);
}

int getLastGameNo()
{
	return GetLastGameNo();
}

int getGameHistory()
{
	return NUMBER_LAST_GAMES;
}

int getLastWinningGameNo()
{
	return GetLastWinningGameNo();
}

unsigned long getMaxStagePayoutValue()
{
	return GetMaxStagePayoutValue();
}

unsigned int getPromoGame(int game)
{
	return GetPromoGame(game);
}

bool inDemoSession()
{
	return GetDemoCredits()||GetDemoWinBankDeposit();
}

unsigned long getCollectableCredits()
{
	return GetCollectableCredits();
}

unsigned long getCollectableBankDeposit()
{
	return GetCollectableBankDeposit();
}

bool allowCollect()
{
	return GetAllowCollect > 0;
}