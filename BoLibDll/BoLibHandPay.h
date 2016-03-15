#define DLLEXPORT extern "C" __declspec(dllexport)

DLLEXPORT void setHandPayThreshold(unsigned int value);
DLLEXPORT unsigned int getHandPayThreshold();
DLLEXPORT bool getHandPayActive();
DLLEXPORT void sendHandPayToServer(unsigned int paid_out, unsigned int release);
//DLLEXPORT void addHandPayToEDC(unsigned int value);
DLLEXPORT bool performHandPay();
DLLEXPORT void cancelHandPay();
DLLEXPORT bool canPerformHandPay();
