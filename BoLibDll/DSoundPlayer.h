#ifndef DSOUND_PLAYER_H
#define DSOUND_PLAYER_H

#define DLLEXPORT extern "C" __declspec(dllexport)

#include "General.h"

typedef struct pcm_sound_typ
{
	LPDIRECTSOUNDBUFFER dsbuffer;   // the ds buffer containing the sound
	int state;                      // state of the sound
	int rate;                       // playback rate
	int size;                       // size of sound
	int id;                         // id number of the sound
} pcm_sound, *pcm_sound_ptr;

#define MAX_SOUNDS 64

#define SOUND_NULL     0
#define SOUND_LOADED   1
#define SOUND_PLAYING  2
#define SOUND_STOPPED  3

#define	CLOSING			1
#define	CLOSED			2

#define DSVOLUME_TO_DB(volume) ((DWORD)(-30*(100 - volume)))

int DeleteAllSounds();
int DeleteSound(int id);

DLLEXPORT int DirectSoundInit();
DLLEXPORT int DirectSoundShutdown();
DLLEXPORT int LoadWavFile(char *filename, int control_flags);

int PlaySound(int id, int flags, int vol, int rate, int pan);
int Replicate_Sound(int source_id);
int SetSoundFreq(int id, int freq);
int SetSoundPan(int id, int pan);
int SetSoundVolume(int id, int vol);
int StatusSound(int id);
int StopAllSounds();
int StopSound(int id);

DLLEXPORT void DoPlaySound();

#endif
