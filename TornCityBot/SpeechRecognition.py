import speech_recognition as sr
import os

audio_file = sr.AudioFile(os.getcwd()+"\\audio.wav");
#audio_file = sr.AudioFile("C:\\Users\\chook\\source\\repos\\BlackHawkCH91\\TornCityBot\\TornCityBot\\bin\\Debug\\net6.0\\bin\\Debug\\net6.0\audio.wav");
rec = sr.Recognizer()
with audio_file as af:
    audio = rec.record(af)

output = rec.recognize_google(audio);
print(output)