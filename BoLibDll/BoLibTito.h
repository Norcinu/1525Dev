#define DLLEXPORT extern "C" __declspec(dllexport)

DLLEXPORT unsigned int	getTitoStateValue();
DLLEXPORT bool			getTitoEnabledState();
DLLEXPORT unsigned int  getTitoProcessInState(void);
DLLEXPORT unsigned int  getTitoHost(void);
DLLEXPORT unsigned int  getTitoTicketPresented(void);
DLLEXPORT void			setTitoState(int state);
