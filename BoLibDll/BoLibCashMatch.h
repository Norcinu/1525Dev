#define DLLEXPORT extern "C" __declspec(dllexport)

DLLEXPORT unsigned long addToCashMatchCredits(unsigned long value);
DLLEXPORT void          addCashMatchEventTotalValue(unsigned int value);
DLLEXPORT void          addCashMatchEventTotalCount(unsigned int value);
DLLEXPORT unsigned long getCashMatchCredits(void);
DLLEXPORT unsigned int  getCashMatchEventMaxTotalValue(void);
DLLEXPORT unsigned int  getCashMatchEventMaxValue(void);
DLLEXPORT unsigned int  getCashMatchEventMaxTotalCount(void);
DLLEXPORT unsigned int  getCashMatchEventTotalValue(void);
DLLEXPORT unsigned int  getCashMatchEventTotalCount(void);

DLLEXPORT unsigned int  getDemoPlayEventMaxTotalValue(void);
DLLEXPORT unsigned int  getDemoPlayEventMaxValue(void);
DLLEXPORT unsigned int  getDemoPlayEventMaxTotalCount(void);
DLLEXPORT unsigned int  getDemoPlayEventTotalValue(void);
DLLEXPORT unsigned int  getDemoPlayEventTotalCount(void);

DLLEXPORT void          setCashMatchEventMaxTotalValue(unsigned int Value);
DLLEXPORT void          setCashMatchEventMaxValue(unsigned int Value);
DLLEXPORT void          setCashMatchEventMaxTotalCount(unsigned int Value);
DLLEXPORT void          setDemoPlayEventMaxTotalValue(unsigned int Value);
DLLEXPORT void          setDemoPlayEventMaxValue(unsigned int Value);
DLLEXPORT void          setDemoPlayEventMaxTotalCount(unsigned int Value);

DLLEXPORT unsigned long subCashMatchCredits(unsigned long value);

//Maximum limits
DLLEXPORT unsigned int getCashMatchMax();
DLLEXPORT unsigned int getCashMatchEventMax();
DLLEXPORT unsigned int getCashMatchValueMax();
DLLEXPORT unsigned int getDemoCreditsMax();
DLLEXPORT unsigned int getDemoEventsMax();
DLLEXPORT unsigned int getDemoGamesMax();
