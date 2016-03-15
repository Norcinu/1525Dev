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
	
	if (termType != PRINTER || titoState == TITO_ENABLED_NotREGISTERED)
	{
		if (GetHandPayActive() || country == CC_EURO || titoState == TITO_ENABLED_NotREGISTERED)
		{
			auto totalCredits = GetBankDeposit() + GetCredits();
			
			SendHeaderOnly(HANDPAY_CONFIRM, 1);
			AddToPerformanceMeters(HAND_PAY_LT, totalCredits);
			SetMeterPulses(2, 1, totalCredits);
			
			SendHandPay2Server(totalCredits, MODEL_NUMBER);
			
			/*if (GetInTournamentPlay())
			{
				AddToTPlayLog(totalCredits, TPLAY_SESSION_HAND_PAID);
				ClearTPlaySessionActive();
			}*/
			
			zero_cdeposit();
			ZeroBankDeposit();
			
			return true;
		}
	}
	return false;
}

void cancelHandPay()
{
	SendHeaderOnly(HANDPAY_CANCEL, 1);
}
