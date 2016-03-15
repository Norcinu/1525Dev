#include "AudioSample.h"
#include "AudioManager.h"



AudioSample::AudioSample()
{
	m_secondaryBuffer = 0;
}

AudioSample::~AudioSample()
{
	if(m_secondaryBuffer)
	{
		m_secondaryBuffer->Release();
		m_secondaryBuffer = 0;
	}
}

void AudioSample::Play(bool loop)
{
#ifdef FAST_PLAY
	return;
#endif
	if(FAILED(m_secondaryBuffer->SetCurrentPosition(0)))
	{
		char buffer[256];
		sprintf_s(buffer, "\"%s\" Could Not Reset Sound Position", m_name.c_str());
	}

	if(FAILED(m_secondaryBuffer->SetVolume((TheAudioManager::Instance()->GetVolume() - 100) * 35)))
	{
		char buffer[256];
		sprintf_s(buffer, "\"%s\" Could Not Set Volume", m_name.c_str());
	}
	
	if(loop)
	{
		if(FAILED(m_secondaryBuffer->Play(0, 0, DSBPLAY_LOOPING)))
		{
			char buffer[256];
			sprintf_s(buffer, "Could Not Play \"%s\"", m_name.c_str());
		}
	}
	else
	{
		if(FAILED(m_secondaryBuffer->Play(0, 0, 0)))
		{
			char buffer[256];
			sprintf_s(buffer, "Could Not Play \"%s\"", m_name.c_str());
		}
	}
}

void AudioSample::Stop()
{
	if(FAILED(m_secondaryBuffer->Stop()))
	{
		char buffer[256];
		sprintf_s(buffer, "\"%s\" Could Not Stop", m_name.c_str());
	}
}

bool AudioSample::IsPlaying() const
{
	DWORD status;
	m_secondaryBuffer->GetStatus(&status);

	if(status &= DSBSTATUS_PLAYING)
	{
		return true;
	}
	return false;
}

bool AudioSample::Load(const std::string& name)//, const std::string& filename)
{
	m_name = name;
	const std::string filename = m_name;
	FILE* filePtr = 0;
	int error = fopen_s(&filePtr, filename.c_str(), "rb");

	if(error)
	{
		char buffer[256];
		sprintf_s(buffer, "Could Not Open Audio File %s", filename.c_str());
		return false;
	}

	WaveHeader waveHeader;
	unsigned int count = fread(&waveHeader, sizeof(WaveHeader), 1, filePtr);

	if(count != 1)
	{
		char buffer[256];
		sprintf_s(buffer, "Could Not Read WaveHeader From %s", filename.c_str());
		return false;
	}

	if((waveHeader.m_chunkId[0] != 'R') || (waveHeader.m_chunkId[1] != 'I') ||
		(waveHeader.m_chunkId[2] != 'F') || (waveHeader.m_chunkId[3] != 'F'))
	{
		char buffer[256];
		sprintf_s(buffer, "The Chunk ID Of %s Is Not In The RIFF Format", filename.c_str());
		return false;
	}

	if((waveHeader.m_format[0] != 'W') || (waveHeader.m_format[1] != 'A') ||
		(waveHeader.m_format[2] != 'V') || (waveHeader.m_format[3] != 'E'))
	{
		char buffer[256];
		sprintf_s(buffer, "%s Is Not Of WAVE Format", filename.c_str());
		return false;
	}

	if((waveHeader.m_subChunkId[0] != 'f') || (waveHeader.m_subChunkId[1] != 'm') ||
		(waveHeader.m_subChunkId[2] != 't') || (waveHeader.m_subChunkId[3] != ' '))
	{
		char buffer[256];
		sprintf_s(buffer, "The SubChunk ID Of %s Is Not 'fmt'", filename.c_str());
		return false;
	}

	if(waveHeader.m_audioFormat != WAVE_FORMAT_PCM)
	{
		char buffer[256];
		sprintf_s(buffer, "%s Is Not Of WAVE_FORMAT_PCM Format", filename.c_str());
		return false;
	}

	if(waveHeader.m_numChannels != 2 && waveHeader.m_numChannels != 1)
	{
		char buffer[256];
		sprintf_s(buffer, "%s Was Not Recorded In Mono Or Stereo Format", filename.c_str());
		return false;
	}

	if(waveHeader.m_sampleRate != 44100)
	{
		char buffer[256];
		sprintf_s(buffer, "%s Was Not Recorded At A Sample Rate Of 44.1 KHz Please Convert!",
			filename.c_str());
		return false;
	}

	if(waveHeader.m_bitsPerSample != 16)
	{
		char buffer[256];
		sprintf_s(buffer, "%s Was Not Recorded In 16 Bit Format", filename.c_str());
		return false;
	}

	if((waveHeader.m_dataChunkId[0] != 'd') || (waveHeader.m_dataChunkId[1] != 'a') ||
		(waveHeader.m_dataChunkId[2] != 't') || (waveHeader.m_dataChunkId[3] != 'a'))
	{
		char buffer[256];
		sprintf_s(buffer, "The DataChunk Header Of %s Is Not 'data'", filename.c_str());
		return false;
	}

	WAVEFORMATEX waveFormat;
	waveFormat.cbSize = 0;
	waveFormat.nChannels = waveHeader.m_numChannels;
	waveFormat.nSamplesPerSec = 44100;
	waveFormat.wBitsPerSample = 16;
	waveFormat.wFormatTag = WAVE_FORMAT_PCM;
	waveFormat.nBlockAlign = (waveFormat.wBitsPerSample / 8) * waveFormat.nChannels;
	waveFormat.nAvgBytesPerSec = waveFormat.nSamplesPerSec * waveFormat.nBlockAlign;

	DSBUFFERDESC buffer;
	buffer.dwBufferBytes = waveHeader.m_dataSize;
	buffer.dwFlags = DSBCAPS_CTRLVOLUME | DSBCAPS_GLOBALFOCUS;
	buffer.dwReserved = 0;
	buffer.dwSize = sizeof(DSBUFFERDESC);
	buffer.guid3DAlgorithm = GUID_NULL;
	buffer.lpwfxFormat = &waveFormat;

	IDirectSoundBuffer* tempBuffer;

	if(FAILED(TheAudioManager::Instance()->GetDirectSound()->CreateSoundBuffer(&buffer, 
		&tempBuffer, NULL)))
	{
		char buffer[256];
		sprintf_s(buffer, "Creating Temporary Buffer For %s Failed", filename.c_str());
		return false;
	}

	if(FAILED(tempBuffer->QueryInterface(IID_IDirectSoundBuffer8, (void**)&m_secondaryBuffer)))
	{
		char buffer[256];
		sprintf_s(buffer, "Creating Secondary Buffer For %s Failed", filename.c_str());
		return false;
	}

	tempBuffer->Release();
	tempBuffer = 0;

	fseek(filePtr, sizeof(WaveHeader), SEEK_SET);
	unsigned char* waveData = new unsigned char[waveHeader.m_dataSize];

	if(!waveData)
	{
		char buffer[256];
		sprintf_s(buffer, "%s Has No Wave Data", filename.c_str());
		return false;
	}

	count = fread(waveData, 1, waveHeader.m_dataSize, filePtr);

	if(count != waveHeader.m_dataSize)
	{
		char buffer[256];
		sprintf_s(buffer, "Reading In Wave Data From %s Failed", filename.c_str());
		return false;
	}
	
	error = fclose(filePtr);

	if(error)
	{
		char buffer[256];
		sprintf_s(buffer, "Closing %s Failed", filename.c_str());
		return false;
	}

	unsigned char* bufferPtr;
	unsigned long bufferSize;

	if(FAILED(m_secondaryBuffer->Lock(0, waveHeader.m_dataSize, (void**)&bufferPtr,
		(DWORD*)&bufferSize, NULL, 0, 0)))
	{
		char buffer[256];
		sprintf_s(buffer, "Locking %s Secondary Buffer Failed", filename.c_str());
		return false;
	}

	memcpy(bufferPtr, waveData, waveHeader.m_dataSize);

	if(FAILED(m_secondaryBuffer->Unlock((void*)bufferPtr, bufferSize, NULL, 0)))
	{
		char buffer[256];
		sprintf_s(buffer, "Unlocking %s Secondary Buffer Failed", filename.c_str());
		return false;
	}

	delete [] waveData;
	waveData = 0;

	return true;
}