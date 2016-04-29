#include <Windows.h>
#include <bo.h>
#include <NVR.H>
#include <direct.h>
#include <cstdio>
#include <memory>
#include <string>
#include "BoLibGeneral.h"

#include "dpci_core_api.h"
#include "dpci_sram_api.h"

#include <sys/stat.h>
#include <time.h>
#include <io.h>
#include <fcntl.h>
#include <sys/stat.h>

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);

char chipIdStr[32] = {0};
unsigned char finalChipID[32] = {0};

const std::string MACHINE_INI = "D:\\machine\\machine.ini";

void enableNoteValidator()
{
	EnableNoteValidator();
}

void disableNoteValidator()
{
	DisableNoteValidator();
}

unsigned long getPrinterTicketState()
{
	return Share2;
}

void printTestTicket()
{
	SetUtilRequestBitState(UTIL_PRINT_TICKET_BIT);

	int returnCode = GetCurrentError();
	do
	{
		Sleep(2);
		
		if(returnCode == ERR_SERIAL_PRINTER_NOT_FOUND)
			int a = 0;
		else if(returnCode == ERR_PRINTER_FAILURE)
			int a = 0;
		else if (returnCode == ERR_PRINTER_NO_PAPER)
			int a = 0;

		returnCode = GetCurrentError();
	} while (GetUtilRequestBitState(UTIL_PRINT_TICKET_BIT));
}

char *getBnvStringType(unsigned char bnv)
{
	auto type = GetBnvType();
	if (type == NO_BNV)
		return "NO_BNV";
	else if (type == AUTO_BNV)
		return "AUTO_BNV";
	else if (type == NV9_BNV)
		return "NV9";
	else if (type == MEI_BNV)
		return "MEI";
	else if (type == JCM_BNV)
		return "JCM";
	else if (type == NV11_BNV)
		return "NV11";
	else if (type >= LAST_BNV)
		return "Undefined BNV type.";

	return "";
}

unsigned long useMoneyOutType(int value)
{
	return (!value) ? MONEY_OUT_ST : MONEY_OUT_LT;
}

unsigned long useMoneyInType(int value)
{
	return (!value) ? MONEY_IN_ST : MONEY_IN_LT;
}

unsigned long useRefillType(int value)
{
	return (!value) ? REFILL_L_ST : REFILL_L_LT;
}

unsigned long useVtpMeter(int value)
{
	return (!value) ? GAMEWAGERED_ST : GAMEWAGERED_LT;
}

unsigned long useWonMeter(int value)
{
	return (!value) ? GAMEWON_ST : GAMEWON_LT;
}

unsigned long useHandPayMeter(int value)
{
	return (!value) ? HAND_PAY_ST : HAND_PAY_LT;
}

unsigned long useTicketsMeter(int value)
{
	return (!value) ? TICKET_OUT_ST : TICKET_OUT_LT;
}

unsigned long useStakeInMeter(int meter)
{
	if (!meter)
		return GetReconciliationMeter(COIN_8_ST);
	else
		return GetReconciliationMeter(COIN_8_LT);
}

char *GetUniquePcbID(char TYPE)
{
	unsigned char chipId[DPCI_IDPROM_ID_SIZE];
	unsigned char retries = 5;
		
	unsigned long PromId1 = 0;
	unsigned long PromId2 = 0;

	for (int i = 0; i < DPCI_IDPROM_ID_SIZE; i++)
		chipId[i]=0;
	
	do
	{
		if (dpci_idprom_readid(chipId) == 0)
			break;  //success

		Sleep(100);
	} while (--retries > 0);
	
	for (char a = 0; a < DPCI_IDPROM_ID_SIZE / 2; a++)
	{
		PromId1 += chipId[a];
		if(a < ((DPCI_IDPROM_ID_SIZE / 2) - 1))
			PromId1 = (PromId1 << 8);
	}
	
	for (char a = DPCI_IDPROM_ID_SIZE / 2; a < DPCI_IDPROM_ID_SIZE; a++)
	{
		PromId2 += chipId[a];
		if(a < (DPCI_IDPROM_ID_SIZE - 1))
			PromId2 = (PromId2 << 8);
	}

	if (PromId1 && PromId2)
	{
		if (TYPE)
			sprintf_s(chipIdStr, "%.10lu%.10lu", PromId1, PromId2);	//decimal for encryption
		else
			sprintf_s(chipIdStr, "%.8X-%.8X", PromId1, PromId2);		//hex for display
		
		for (int i = 0; i < 32; i++)
			finalChipID[i] = chipIdStr[i];
		
		return (char *)finalChipID;
	}
	
	sprintf_s(chipIdStr, "Unavailable");
	return chipIdStr;
}
//
int CanChangeDrive(int *drive)
{
	for (auto test_drive = 1; test_drive <= 26; test_drive++)
	{
		if (!_chdrive(test_drive))
		{
			if (test_drive + 'A' - 1 > 'D')
			{
				*drive = test_drive;
				return 0;
			}
		}
	}
	return -1;
}

void setFileAction()
{
	SetFileAction();
}

void clearFileAction()
{
	ClearFileAction();
}

void doReadTicketFile()
{
	char buf[256];
	struct tm *clock;				// create a time structure
	struct stat attrib;				// create a file attribute structure
	int ypos=145;
	int xpos=100;
	int cnt=0;
	int file;
	int StoredCsum,LiveCsum,value;
	unsigned int RSTicketFaceValue = 0;
	unsigned int RSTicketModelNo = 0;
	unsigned int RSTicketNumber;
	unsigned int RSTicketDuplicateNumber = 0;
	unsigned int RSPrintProgress;
	unsigned int RSPrinterStatus;	
	char RSTicketBarCode[32];

	unsigned int StartPrintBankValue = 0;
	unsigned int StartPrintPartCollectValue = 0;
	unsigned int StartPrintCreditValue = 0;

	//====================================
	//	Read data from file

	SetFileAction();

	file = _open("d:\\machine\\game_data\\ticket.dat",FILE_WRITE, _S_IREAD | _S_IWRITE);

	if(file == NULL)
	{
		ClearFileAction();
		return;
	}

	if(_read(file, &value, 4) > 0) 
		RSTicketFaceValue = value;
	LiveCsum = value;

	if(_read(file, &value, 4) > 0) 
		RSTicketModelNo = value; //how many times ram reset
	LiveCsum += value;

	if(_read(file, &value, 4) > 0) 
		RSTicketNumber = value; 
	LiveCsum += value;

	for(char Loop = 0; Loop<32; Loop++)
	{	
		if(_read(file, &value, 4) > 0) 
		{
			RSTicketBarCode[Loop] = value;
			LiveCsum += value;
		}
	}

	if(_read(file, &value, 4) > 0) 
		RSTicketDuplicateNumber = value; 
	LiveCsum += value;

	if(_read(file, &value, 4) > 0) 
		RSPrintProgress = value; 
	LiveCsum += value;

	if(_read(file, &value, 4) > 0) 
		RSPrinterStatus = value; 
	LiveCsum += value;

	if(_read(file, &value, 4) > 0) 
		StartPrintBankValue = value; 
	LiveCsum += value;

	if(_read(file, &value, 4) > 0) 
		StartPrintPartCollectValue = value; 
	LiveCsum += value;

	if(_read(file, &value, 4) > 0) 
		StartPrintCreditValue = value; 
	LiveCsum += value;

	if(_read(file, &value, 4) > 0) 
		StoredCsum = value; //check sum

	if(LiveCsum != StoredCsum)
	{
		_close(file);
		ClearFileAction();
		return;
	}
	_close(file);
	ClearFileAction();

	stat("d:\\machine\\game_data\\ticket.dat", &attrib);		// get the attributes of afile.txt
	clock = gmtime(&(attrib.st_mtime));							// Get the last modified time and put it into the time structure

	buf[0]=0;

	if(RSPrinterStatus)
	{
		//ClearPrinterStatus();
		//SetUpdatePrinterStatus();
	}
	
}

