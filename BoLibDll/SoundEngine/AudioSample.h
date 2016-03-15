#ifndef AUDIOSAMPLE_H
#define AUDIOSAMPLE_H

//------AudioSample------//
/*AudioSample can hold a file of .wav
format, from here it can be played, looped
and stopped.*/
//----------------------//

#include <dsound.h>
#include <string>
//#include "FileRead.h"
////#include "GenericWavData.h"
/**
#define MAX_NO_SOUNDS  END_WAV
#define TITLE_SIZE     45
#define SINGLE			0
#define LOOPING			1
#define MAX_NO_PATHS   40
#define REPEAT_INFINITE DMUS_SEG_REPEAT_INFINITE
#define MIN_PERFORMANCE_VOLUME    500
#define MAX_PERFORMANCE_VOLUME   2500  //needs to be negative
#define PERFORMANCE_VOLUME_INCREMENTS ((MIN_PERFORMANCE_VOLUME-(0-MAX_PERFORMANCE_VOLUME))/100)
***/


struct WaveHeader
{
	char m_chunkId[4];
	unsigned long m_chunkSize;
	char m_format[4];
	char m_subChunkId[4];
	unsigned long m_subChunkSize;
	unsigned short m_audioFormat;
	unsigned short m_numChannels;
	unsigned long m_sampleRate;
	unsigned long m_bytesPerSecond;
	unsigned short m_blockAlign;
	unsigned short m_bitsPerSample;
	char m_dataChunkId[4];
	unsigned long m_dataSize;
};

class AudioSample
{
public:
	AudioSample();
	~AudioSample();
	void Play(bool loop);
	void Stop();
	bool IsPlaying() const;
	bool Load(const std::string& name);//, const std::string& filename);
public:
	//WCHAR  WavList[MAX_NO_SOUNDS][TITLE_SIZE];

private:
	IDirectSoundBuffer8* m_secondaryBuffer;
	std::string m_name;
};

#endif AUDIOSAMPLE_H