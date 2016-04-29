#include <Windows.h>
#include <bo.h>
#include <NVR.H>
#include "BoLibTito.h"
#include "..\1524LibSource\TicketData.h"

unsigned int getTitoStateValue()
{
	return GetTiToEnabledState();
}

bool getTitoEnabledState()
{
	return (GetTiToEnabledState()) ? true : false;
}

unsigned int getTitoProcessInState()
{
	return GetTiToProcessInState();
}

unsigned int getTitoHost()
{
	return GetTiToHost();
}

unsigned int getTitoTicketPresented()
{
	return GetTiToTicketPresented();
}

void setTitoState(int state)
{
	SetTiToEnabledState(state);
}

void clearPrinterStatus(void)
{
	ClearPrinterStatus();
	SetUpdatePrinterStatus();
}
