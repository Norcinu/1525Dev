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

