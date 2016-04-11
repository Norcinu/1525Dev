#include "BoLibHandPay.h"
#include "General.h"

extern unsigned long zero_cdeposit(void);
extern unsigned long add_cdeposit(unsigned long value);
extern unsigned long ZeroReserveCredits();

void setHandPayThreshold(unsigned int value)
{
	SetHandPayThreshold(value);
}

unsigned int getHandPayThreshold()
{
	return GetHandPayThreshold();
}

bool getHandPayActive()
{
	return GetHandPayActive() ? true : false;
}

void sendHandPayToServer(unsigned int paid_out, unsigned int release)
{
	SendHandPay2Server(paid_out, release);
}

bool canPerformHandPay()
{
	auto termType = GetTerminalType();
	auto titoState = GetTiToEnabledState();
	auto country = GetCountry();

	return (termType != PRINTER || titoState == TITO_ENABLED_NotREGISTERED);
}

bool performHandPay()
{
	auto termType = GetTerminalType();
	auto titoState = GetTiToEnabledState();
	auto country = GetCountry();
	
	/*if(GetDemoCredits()||GetDemoWinBankDeposit())
		return false;*/

	if (termType != PRINTER || titoState == TITO_ENABLED_NotREGISTERED)
	{
		if (GetHandPayActive() || country == CC_EURO || titoState == TITO_ENABLED_NotREGISTERED)
		{
			auto totalCredits = GetCollectableBankDeposit() + GetCollectableCredits(); //GetBankDeposit() + GetCredits();
			
			SendHeaderOnly(HANDPAY_CONFIRM, 1);
			AddToPerformanceMeters(HAND_PAY_LT, totalCredits);
			SetMeterPulses(2, 1, totalCredits);
			
			SendHandPay2Server(totalCredits, MODEL_NUMBER);
					
			zero_cdeposit();
			ZeroBankDeposit();
			
			clearCashMatchCredits();

			return true;
		}
	}
	return false;
}

void cancelHandPay()
{
	SendHeaderOnly(HANDPAY_CANCEL, 1);
}

extern unsigned long ZeroCashMatchCredits();
void clearCashMatchCredits()
{
	if (!GetDoorStatus())
		OverRideStagePayouts = 0;

	if(GetCashMatchCredits())   //CashMatch Being Aborted? <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
	{
		unsigned int lValue = GetCashMatchCredits();
		AddToPerformanceMeters(MONEY_OUT_LT,lValue);
		AddToReconciliationMeters(CMATCH_OUT_LT,lValue);
		ZeroCashMatchCredits();
		LockedInCredits = 0;
	}
}
