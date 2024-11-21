Audio Processing Scripts
========================

This directory contains scripts to automate the download, processing, and 
packaging of audio files, including splitting tracks based on metadata in JSON files.

## Setup

### 1. Install System Dependencies

Ensure Python 3.8+ and ffmpeg are installed.

**For Ubuntu/Debian:**
```bash
sudo apt-get install ffmpeg
```

**For macOS:**
```bash
brew install ffmpeg
```

### 2. Create and Activate a Virtual Environment (Optional)

```bash
python3 -m venv venv
source venv/bin/activate  # Linux/macOS
venv\Scripts\activate     # Windows
```

### 3. Install Python Dependencies

```bash
pip install -r requirements.txt
```

## Usage

### 1. Prepare JSON Metadata Files

Place JSON metadata files inside the `metadata/` directory.

Example structure for a JSON file:

```json
{
  "title": "Sample Playlist",
  "dateRecorded": "2024-03-14",
  "tracks": [
    { "title": "Track 1", "startTime": "00:00:00.000", "endTime": "00:05:00.000" },
    { "title": "Track 2", "startTime": "00:05:00.000", "endTime": "00:10:00.000" }
  ]
}
```

### 2. Run the Process Script

Execute the `process.sh` script to download, process, and package audio files.

Example:
    cd src/audio_processing
    ./process.sh -f 2024-03-14

Options:
  -f    Force the download of audio files even if they already exist.
  -k    Keep temporary files after processing.
  -z    Package processed files into a ZIP archive (default).
  -t    Package processed files into a TAR.GZ archive.
  -p    Publish the packaged files to the specified hostname:port.

### 3. Verify Output

Processed files will be saved in the `output/` directory. Temporary files 
will be stored in `output/temp/` unless removed.

### 4. Publishing Processed Files (Optional)

Use the `-p` option to publish processed files to an API endpoint.

Example:
    ./process.sh -p example.com:8080 2024-03-14

Dependencies
- pydub: Used for audio manipulation.
- mutagen: Used for reading audio metadata.

Additional Notes

Ensure that ffmpeg is correctly installed and available in your system’s PATH. 
The Python dependencies are listed in `requirements.txt`.
