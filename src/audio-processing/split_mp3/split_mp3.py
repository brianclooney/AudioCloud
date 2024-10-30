import argparse
import math
import os
import re
import sys
import json
import datetime
from pydub import AudioSegment
import mutagen


def time_to_milliseconds(time_str):
    time_obj = datetime.datetime.strptime(time_str, "%H:%M:%S.%f")
    return (time_obj.hour * 3600 + time_obj.minute * 60 + time_obj.second) * 1000 + time_obj.microsecond // 1000


def milliseconds_to_time_str(milliseconds):
    seconds, milliseconds = divmod(milliseconds, 1000)
    minutes, seconds = divmod(seconds, 60)
    hours, minutes = divmod(minutes, 60)
    return "{:02d}:{:02d}:{:06.3f}".format(hours, minutes, seconds + milliseconds / 1000)


def milliseconds_to_track_duration_str(milliseconds):
    seconds, milliseconds = divmod(milliseconds, 1000)
    if milliseconds > 499:
        seconds += 1
    minutes, seconds = divmod(seconds, 60)
    return "{:02d}:{:02d}".format(minutes, seconds)


def get_bitrate(input_file):
    audio_info = mutagen.File(input_file, easy=True)
    return audio_info.info.bitrate // 1000


def create_filename(index, title, ext):
    title = title.lower()
    title = title.replace('&', 'and')
    title = title.replace('-', ' ')
    title = re.sub(r'[^\w\s]', '', title)
    title = title.replace(' ', '_')

    filename = f"{index:02}_{title}.{ext}"
    return filename


def normalize_audio(input_file):
    audio = AudioSegment.from_mp3(input_file)
    # Normalizing to -20 dBFS
    normalized_audio = match_target_amplitude(audio, -20.0)
    return normalized_audio


def match_target_amplitude(sound, target_dBFS):
    change_in_dBFS = target_dBFS - sound.dBFS
    return sound.apply_gain(change_in_dBFS)


def create_manifest_and_split_audio(input_file, split_info_file, output_dir):

    manifest_filename = f"{output_dir}/manifest.json"
    audio = normalize_audio(input_file)

    file_size = os.path.getsize(input_file)
    byte_rate = file_size / (audio.duration_seconds * 1000)

    with open(split_info_file, 'r') as f:
        split_info = json.load(f)

    manifest = {
        'title': split_info['title'],
        'dateRecorded': split_info['dateRecorded'], 
        'tracks': []
    }
    
    playlist = []
    segments = split_info['tracks']

    for i, segment in enumerate(segments):

        print(f"  processing segment {i+1} of {len(segments)}: {segment['title']}")

        start_time = time_to_milliseconds(segment['startTime'])

        if 'endTime' in segment:
            end_time = time_to_milliseconds(segment['endTime'])
        elif i < len(segments) - 1:
            end_time = time_to_milliseconds(segments[i + 1]['startTime'])
        else:
            end_time = len(audio)

        start_byte = int(start_time * byte_rate)
        end_byte = int(end_time * byte_rate)

        if output_dir:
            output_filename = segment.get('outputFile', create_filename(i+1, segment['title'], 'mp3'))
            output_file = os.path.join(output_dir, output_filename)
            track = audio[start_time:end_time]
            track.export(output_file, format="mp3", bitrate="128k")
            playlist.append({'title': segment['title'], 'mp3': output_file})

        track_info = {
            'index': i + 1,
            'file': output_filename,
            'title': segment['title'],
            'duration': math.ceil(len(track)/1000)
            # 'start_byte': start_byte,
            # 'end_byte': end_byte
        }

        manifest['tracks'].append(track_info)
        
    if manifest_filename:
        with open(manifest_filename, 'w') as f:
            json.dump(manifest, f, indent=2)

    # if playlist_filename and output_dir:
    #     with open(playlist_filename, 'w') as f:
    #         json.dump(playlist, f, indent=2)


if __name__ == "__main__":

    if sys.version_info < (3, 6):
        sys.exit("  Error: This script requires Python 3.6 or later.\n"
                 "  You are currently running Python {}.{}\n"
                 "  Please make sure to activate the correct Python environment by running 'source .venv/bin/activate' before executing this script."
                 .format(sys.version_info.major, sys.version_info.minor))

    parser = argparse.ArgumentParser(description="Split an MP3 file based on a split info file.")
    parser.add_argument("input_file", help="Path to the input MP3 file.")
    parser.add_argument("split_info_file", help="Path to the split info JSON file.")
    parser.add_argument("output_dir", help="Directory to output the split MP3 files.")
    
    args = parser.parse_args()

    create_manifest_and_split_audio(
        args.input_file, 
        args.split_info_file, 
        args.output_dir)
