#include "AudioManager.h"
#include "bo.h"
#include "Defines.h"


void InitDirectSound()
{
	AUDIO->Initialize(::GetDesktopWindow());
}

void addSound(const char* filename)
{
	AUDIO->LoadAudio(filename);
}

void loadAndPlayFile(const char* filename)
{
	if (AUDIO->GetDirectSound() == nullptr) {
		AUDIO->Initialize(::GetDesktopWindow());
	}
	
	AUDIO->SetVolume(GetLocalMasterVolume());
	
	if (AUDIO->LoadAudio(filename)) {
		AUDIO->GetAudioSample(std::string(filename))->Play(false);
		::Sleep(20);
	}
}

void justPlaySound(const char* filename)
{
	if (AUDIO->GetDirectSound() == nullptr) 
		return;

	AUDIO->SetVolume(GetLocalMasterVolume());

	//if (AUDIO->GetAudioSamples().size() > 0) {
		auto sample = AUDIO->GetAudioSample(std::string(filename));
		if (sample) {
			sample->Play(false);
			::Sleep(20);
		} else {
			loadAndPlayFile(filename);
		}
	//}
}

void clearSoundResources()
{
	TheAudioManager::Instance()->CleanUp();
}

AudioManager::AudioManager()
{
	m_directSound = 0;
	m_primaryBuffer = 0;
	m_volume = 25;
}

AudioManager::~AudioManager()
{
	CleanUp();
}

void AudioManager::CleanUp()
{
	for(AudioSamples::iterator it = m_audioSamples.begin(); it != m_audioSamples.end(); ++it)
	{
		delete it->second;
	}

	m_audioSamples.clear();

	if(m_primaryBuffer)
	{
		m_primaryBuffer->Release();
		m_primaryBuffer = 0;
	}

	if(m_directSound)
	{
		m_directSound->Release();
		m_directSound = 0;
	}
}

void AudioManager::SetVolume(unsigned int volume)
{
	m_volume = volume;
}

void AudioManager::StopAllSounds()
{
	for(AudioSamples::iterator it = m_audioSamples.begin(); it != m_audioSamples.end(); ++it)
	{
		it->second->Stop();
	}

	m_primaryBuffer->Stop();
}

AudioSample* AudioManager::GetAudioSample(const std::string& name)
{
	AudioSamples::iterator it = m_audioSamples.find(name);

	if(it != m_audioSamples.end())
	{
		return it->second;
	}

	char buffer[256];
	sprintf_s(buffer, "The AudioSample \"%s\" Cannot Be Found In The AudioManager", name.c_str());
	return 0;
}

bool AudioManager::Initialize(HWND hwnd)
{
	if(FAILED(DirectSoundCreate8(NULL, &m_directSound, NULL)))
	{
		return false;
	}
	
	
	if(FAILED(m_directSound->SetCooperativeLevel(hwnd, DSSCL_PRIORITY)))
	{
		return false;
	}
	
	DSBUFFERDESC buffer;
	buffer.dwBufferBytes = 0;
	buffer.dwFlags = DSBCAPS_PRIMARYBUFFER | DSBCAPS_GLOBALFOCUS;
	buffer.dwReserved = 0;
	buffer.dwSize = sizeof(DSBUFFERDESC);
	buffer.guid3DAlgorithm = GUID_NULL;
	buffer.lpwfxFormat = NULL;

	if(FAILED(m_directSound->CreateSoundBuffer(&buffer, &m_primaryBuffer, NULL)))
	{
		return false;
	}
	
	WAVEFORMATEX waveFormat;
	waveFormat.cbSize = 0;
	waveFormat.nChannels = 2;
	waveFormat.nSamplesPerSec = 44100;
	waveFormat.wBitsPerSample = 16;
	waveFormat.wFormatTag = WAVE_FORMAT_PCM;
	waveFormat.nBlockAlign = (waveFormat.wBitsPerSample / 8) * waveFormat.nChannels;
	waveFormat.nAvgBytesPerSec = waveFormat.nSamplesPerSec * waveFormat.nBlockAlign;

	if(FAILED(m_primaryBuffer->SetFormat(&waveFormat)))
	{
		return false;
	}

	return true;
}

bool AudioManager::LoadAudio(const char* fname)
{
	std::string name(fname);

	auto audio = new AudioSample;
	if (!audio->Load(name)) {
		return false;
	}
	
	m_audioSamples[name] = audio;

	return true;
}

bool AudioManager::LoadAudio()
{
	return true;
}

IDirectSound8* AudioManager::GetDirectSound() const
{
	if(m_directSound)
	{
		return m_directSound;
	}

	return 0;
}

unsigned int AudioManager::GetVolume() const
{
	return m_volume;
}

void AudioManager::SetServerBasedGame(unsigned char type)
{
	if(type != ServerBasedGame)
	{
		ServerBasedGame = type;
	}
}

void AudioManager::CheckPerformanceVolumeChanged(void)
{
	if(ServerBasedGame)
	{
		if(GetRemoteMasterVolume()!=SavedMasterVolume)
		{
			SavedMasterVolume = GetRemoteMasterVolume();		//GV: save volume to check if changed during game play
			SetVolume(SavedMasterVolume);
		}
	}
	else
	{
		if(GetLocalMasterVolume()!= SavedMasterVolume)
		{
			SavedMasterVolume = GetLocalMasterVolume();		//GV: save volume to check if changed during game play
			SetVolume(SavedMasterVolume);
		}
	}
}

AudioSamples AudioManager::GetAudioSamples() const 
{ 
	return m_audioSamples; 
}
