#include "General.h"
#include "BoLibSetValues.h"
#include "BoLibGetValues.h" // errr

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);

CRITICAL_SECTION critical;

int setEnvironment()
{
	InitializeCriticalSectionAndSpinCount(&critical, 0x00000400);
	return SetEnvironment(RELEASEx);
}

void closeSharedMemory()
{
	DeleteCriticalSection(&critical);
	CloseSharedMemory();
}

void clearBankAndCredit()
{
	ZeroBankDeposit();
	zero_cdeposit();
}

int setCountryCode(int countryCode)
{
	nvr_main->countryCode = countryCode;
	return GetCountry();
}

int clearError()
{
	ClearCriticalError(GetCurrentError());
	return GetCurrentError();
}

void transferBankToCredit()
{
	TransferFromBankToCredits();
}

void setTargetPercentage(int Percentage)
{
	SetTargetPercentage(Percentage);
}

void setLocalMasterVolume(unsigned int val)
{
	SetLocalMasterVolume(val);
}

void setLampStatus(unsigned char offset, unsigned char mask, unsigned char state)
{
	SetLampStatus(offset, mask, state);
}

void setHopperFloatLevel(unsigned char hopper, unsigned int value)
{
	SetHopperFloatLevel(hopper, value);
}

void setRequestEmptyHopper(int hopper)
{
	SetUtilRequestBitState(hopper);
}

void setCriticalError(int code)
{
	if (!GetCurrentError())
		SetCriticalError(code);
}

void clearShortTermMeters()
{
	ClearShortTermMeters();
}

void setHopperDivertLevel(unsigned char hopper, unsigned int value)
{
	SetHopperDivertLevel(hopper, value);
}

void shellSendEmptyRecycler()
{
	Share2 |= UTIL_EMPTY_RECYCLER_BIT; 
}

void setTerminalType(unsigned char type)
{
	SetTerminalType(type);
}

void setPrinterType(unsigned char type)
{
	SetPrinterType(type);
}

void setRecyclerChannel(unsigned char value)
{
	SetRecyclerChannel(value);
}

void setBnvType(unsigned char value)
{
	SetBnvType(value);
}

void setRebootRequired()
{
	SetRebootRequired();
}


void setUtilsAdd2CreditValue(unsigned int value)
{
    SetUtilsAdd2CreditValue(value);
}

void enableUtilsCoinBit()
{
	nvr_main->share_2 |= 0x80;
}

void disableUtilsCoinBit()
{
	nvr_main->share_2 &= ~0x80;
}

void setUtilRequestBitState(int bit)
{
	SetUtilRequestBitState(bit);
}

void clearUtilRequestBitState(int bit)
{
	ClearUtilRequestBitState(bit);
}

void clearBankCreditReserve()
{
	nvr_ptr->bank1 = 0;
	nvr_ptr->bank2 = 0;
	nvr_ptr->bank3 = 0;

	nvr_ptr->cd1 = 0;
	nvr_ptr->cd2 = 0;
	nvr_ptr->cd3 = 0;

	//TODO: clear player points?
	/*nvr_ptr->reserveCredits1 = 0;
	nvr_ptr->reserveCredits2 = 0;
	nvr_ptr->reserveCredits3 = 0;*/
}

void setSmartCardPointsRTP(unsigned char value)
{
	SetSmartCardPointsRTP(value);
}

void clearPartialCollectValue()
{
	ClearPartialCollectValue();
}
