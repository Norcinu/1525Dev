#ifndef AUDIOMANAGER_H
#define AUDIOMANAGER_H

#define DIRECTSOUND_VERSION 0x0900

#include <map>
#include <string>
#include <Windows.h>
#include "AudioSample.h"
#include "NonCopyable.h"
#include "Singleton.h"
#include "../General.h"

#define DLLEXPORT extern "C" __declspec(dllexport)

DLLEXPORT void InitDirectSound();
DLLEXPORT void addSound(const char* filename);
DLLEXPORT void loadAndPlayFile(const char* filename);
DLLEXPORT void clearSoundResources();
DLLEXPORT void justPlaySound(const char* filename);

typedef std::map<std::string, AudioSample*> AudioSamples;

class AudioManager : public NonCopyable
{
private:
	AudioManager();
	friend class Singleton<AudioManager>;
	
public:
	~AudioManager();

	void SetVolume(unsigned int volume);
	void StopAllSounds();
	AudioSample* GetAudioSample(const std::string& name);
	bool Initialize(HWND hwnd);
	bool LoadAudio();
	bool LoadAudio(const char* fname);
	IDirectSound8* GetDirectSound() const;
	unsigned int GetVolume() const;

	void CheckPerformanceVolumeChanged(void);
	void SetServerBasedGame(unsigned char type);
	
	void CleanUp();
	AudioSamples GetAudioSamples() const;
	
private:
	AudioSamples m_audioSamples;
	IDirectSound8* m_directSound;
	IDirectSoundBuffer* m_primaryBuffer;
	unsigned int m_volume;
	unsigned int SavedMasterVolume;
	int	ServerBasedGame;
};

typedef Singleton<AudioManager> TheAudioManager;

#define AUDIO TheAudioManager::Instance()

#endif AUDIOMANAGER_H 