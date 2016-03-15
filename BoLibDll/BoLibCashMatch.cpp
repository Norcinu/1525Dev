#include "BoLibCashMatch.h"
#include "General.h"

unsigned long addToCashMatchCredits(unsigned long value)
{
	return AddToCashMatchCredits(value);
}

void addCashMatchEventTotalValue(unsigned int value)
{
	AddCashMatchEventTotalValue(value);
}

void addCashMatchEventTotalCount(unsigned int value)
{
	AddCashMatchEventTotalCount(value);
}

unsigned long getCashMatchCredits(void)
{
	return GetCashMatchCredits();
}

unsigned int getCashMatchEventMaxTotalValue(void)
{
	return GetCashMatchEventMaxTotalValue();
}

unsigned int getCashMatchEventMaxValue(void)
{
	return GetCashMatchEventMaxValue();
}

unsigned int getCashMatchEventMaxTotalCount(void)
{
	return GetCashMatchEventMaxTotalCount();
}

unsigned int  getCashMatchEventTotalValue(void)
{
	return GetCashMatchEventTotalValue();
}

unsigned int  getCashMatchEventTotalCount(void)
{
	return GetCashMatchEventTotalCount();
}

void setCashMatchEventMaxTotalValue(unsigned int Value)
{
	SetCashMatchEventMaxTotalValue(Value);
}

void setCashMatchEventMaxValue(unsigned int Value)
{
	SetCashMatchEventMaxValue(Value);
}

void setCashMatchEventMaxTotalCount(unsigned int Value)
{
	SetCashMatchEventMaxTotalCount(Value);
}

void setDemoPlayEventMaxTotalValue(unsigned int Value)
{
	SetDemoPlayEventMaxTotalValue(Value);
}

void setDemoPlayEventMaxValue(unsigned int Value)
{
	SetDemoPlayEventMaxValue(Value);
}

void setDemoPlayEventMaxTotalCount(unsigned int Value)
{
	SetDemoPlayEventMaxTotalCount(Value);
}

unsigned long subCashMatchCredits(unsigned long value)
{
	return SubCashMatchCredits(value);
}

unsigned int getCashMatchMax()
{
	return MAX_CASH_MATCH;
}

unsigned int getCashMatchEventMax()
{
	return MAX_CASH_MATCH_EVENT;
}

unsigned int getCashMatchValueMax()
{
	return MAX_CASH_MATCH_VALUE;
}

unsigned int getDemoCreditsMax()
{
	return MAX_DEMO_CREDITS;
}

unsigned int getDemoEventsMax()
{
	return MAX_DEMO_EVENTS;
}

unsigned int getDemoGamesMax()
{
	return MAX_DEMO_GAMES;
}

unsigned int getDemoPlayEventMaxTotalValue()
{
	return GetDemoPlayEventMaxTotalValue(); 
}

unsigned int getDemoPlayEventMaxValue()
{
	return GetDemoPlayEventMaxValue();
}

unsigned int getDemoPlayEventMaxTotalCount()
{
	return GetDemoPlayEventMaxTotalCount();
}

unsigned int getDemoPlayEventTotalValue()
{
	return GetDemoPlayEventTotalValue();
}

unsigned int getDemoPlayEventTotalCount()
{
	return GetDemoPlayEventTotalCount();
}

