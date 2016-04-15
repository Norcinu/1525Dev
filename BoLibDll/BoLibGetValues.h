#define DLLEXPORT extern "C" __declspec(dllexport)

#include <Windows.h>

struct GamesInfo;
struct SpanishRegional;

typedef int BOOL;

DLLEXPORT int			getMaxCredits();
DLLEXPORT int			getMaxBank();
DLLEXPORT int			getTargetPercentage();
DLLEXPORT bool			isDualBank();
DLLEXPORT int			getError();
DLLEXPORT const char	*getErrorText();
DLLEXPORT int			getDoorStatus();
DLLEXPORT int			refillKeyStatus();
DLLEXPORT int			getCredit();
DLLEXPORT int			getBank();		    
DLLEXPORT int			addCredit(int pennies);
DLLEXPORT int			getCountryCode();
DLLEXPORT char			*getCountryCodeStr();
DLLEXPORT const char	*getLastGame(int index);
DLLEXPORT unsigned long	getWinningGame(int index);
DLLEXPORT unsigned long	getPerformanceMeter(unsigned char Offset);
DLLEXPORT unsigned long	getGamePerformanceMeter(unsigned int Offset, unsigned int MeterType);
DLLEXPORT unsigned int	getLocalMasterVolume();
DLLEXPORT unsigned long	getGameModel(int index);
DLLEXPORT unsigned int  getGameTime(int index);
DLLEXPORT unsigned int	getGameDate(int index);
DLLEXPORT unsigned int  getGameWager(int index);
DLLEXPORT unsigned int  getGameCreditLevel(int index);
DLLEXPORT unsigned int	getSwitchStatus(unsigned char offset, unsigned char mask);
DLLEXPORT unsigned int	getLastNote(int index);
DLLEXPORT unsigned int	*getLastNotes();
DLLEXPORT unsigned int	getHopperFloatLevel(unsigned char hopper);
DLLEXPORT unsigned int	getHopperDivertLevel(unsigned char hopper);
DLLEXPORT unsigned char	getHopperDumpSwitchActive();
DLLEXPORT unsigned char	getHopperDumpSwitch();
DLLEXPORT unsigned int	getRequestEmptyLeftHopper();
DLLEXPORT unsigned int	getRequestEmptyRightHopper();
DLLEXPORT unsigned char	getBnvType();
DLLEXPORT unsigned int	getRecyclerFloatValue();
DLLEXPORT signed int	getRefillCtr(unsigned char hopper);
DLLEXPORT unsigned char getLeftHopper();
DLLEXPORT unsigned char	getMiddleHopper();
DLLEXPORT unsigned char	getRightHopper();
DLLEXPORT unsigned int	getMinPayoutValue();
DLLEXPORT unsigned long	getCashIn(int meter);
DLLEXPORT unsigned long	getCashOut(int meter);
DLLEXPORT unsigned long	getNotesIn(int meter);
DLLEXPORT unsigned long	getNotesOut(int meter);
DLLEXPORT unsigned long	getRefillValue(int meter);
DLLEXPORT unsigned long	getVtp(int meter);
DLLEXPORT unsigned long	getWon(int meter);
DLLEXPORT unsigned int	getHandPay(int meter);
DLLEXPORT unsigned long	getTicketsPay(int meter);
DLLEXPORT char			*getSerialNumber();
DLLEXPORT char			*getEDCTypeStr();
DLLEXPORT unsigned long	getReconciliationMeter(unsigned char offset);
DLLEXPORT void			getMemoryStatus(MEMORYSTATUS *memory);
DLLEXPORT unsigned int	getNumberOfGames();
DLLEXPORT unsigned int	getBoLibVersion();
//DLLEXPORT void			getRegionalValues(int index, SpanishRegional *region);
DLLEXPORT char 			*getErrorMessage(char *str, int code);
DLLEXPORT int			getUtilsRelease();
//DLLEXPORT unsigned long getTPlayMeter(unsigned char offset);
DLLEXPORT int			getUkCountryCodeB3();
DLLEXPORT int			getUkCountryCodeC();
DLLEXPORT int			getSpainCountryCode();
DLLEXPORT unsigned char getRecyclerChannel();
DLLEXPORT unsigned long getMaxHandPayThreshold();
DLLEXPORT unsigned int	getCabinetType();
DLLEXPORT unsigned char combined();
DLLEXPORT unsigned char hopper();
DLLEXPORT unsigned char printer();
DLLEXPORT unsigned int  getTerminalType();
DLLEXPORT unsigned char getTerminalFormat();
DLLEXPORT unsigned int	getUtilsAdd2CreditValue();
DLLEXPORT unsigned long getLastGameModel(int index);
DLLEXPORT unsigned long getReserveCredits();
DLLEXPORT bool			isBackOfficeAvilable();
DLLEXPORT unsigned long getWinningGameMeter(int offset, int meter);
DLLEXPORT unsigned long getHistoryLength();
DLLEXPORT char			*getLicense();
DLLEXPORT char			*getCountryCodeStrLiteral(char *str, int code);
//DLLEXPORT unsigned long getEspRegionalVariableValue(int ValueIndex);
DLLEXPORT unsigned int	getBankCreditsReservePtr();
DLLEXPORT bool			getIsHopperHopping();
DLLEXPORT bool			getUtilRequestBitState(int whichBit);
DLLEXPORT unsigned char getSmartCardGroup();
DLLEXPORT unsigned char getSmartCardSubGroup();
DLLEXPORT unsigned char getUtilsAccessLevel();
DLLEXPORT bool			getUtilDoorAccess();
DLLEXPORT bool			getUtilRefillAccess();
DLLEXPORT bool			getUtilSmartCardAccess(int whichBit);
DLLEXPORT int			getLastPayoutType();
DLLEXPORT unsigned char getSmartCardPointsRTP();
DLLEXPORT unsigned long getPartialCollectValue();
DLLEXPORT unsigned long getCashMatchMeter(unsigned char offset);
DLLEXPORT unsigned long getRewardPointMeter(unsigned char offset);
DLLEXPORT unsigned long getDemoPlayMeter(unsigned char offset);

DLLEXPORT int			getLastGameNo();
DLLEXPORT int			getGameHistory();
DLLEXPORT int			getLastWinningGameNo();

DLLEXPORT unsigned long getMaxStagePayoutValue();

DLLEXPORT unsigned int	getPromoGame(int game);
DLLEXPORT bool			inDemoSession();
DLLEXPORT unsigned long getCollectableCredits();
DLLEXPORT unsigned long getCollectableBankDeposit();
DLLEXPORT bool			allowCollect();
DLLEXPORT bool			getRequestHopperPayout();


