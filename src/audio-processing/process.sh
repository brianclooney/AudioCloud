#!/bin/bash

# Configuration
OUTPUT_DIR="out"
TEMP_DIR="$OUTPUT_DIR/tmp"
DOWNLOAD_DIR="out/raw"
SOURCE_DIR="metadata"

# Default options
FORMAT="zip"  # Default format
FORCE_DOWNLOAD=false
PUBLISH=false
PUBLISH_TARGET=""
KEEP_TEMP=false

# Usage function to display help
usage() {
    cat << EOF
Usage: $0 [options] <date1> <date2> ...

This script downloads, processes, and packages audio files for given dates.

OPTIONS:
  -f    Force the download of audio files even if they already exist.
  -k    Keep temporary files after processing instead of cleaning them up.
  -z    Package processed files into a ZIP archive (default).
  -t    Package processed files into a TAR.GZ archive.
  -p    Publish the packaged files to the specified hostname:port.
  -h    Show this help message and exit.

EXAMPLES:
  $0 -f 2024-03-14                    Force download and package into ZIP (default).
  $0 -kt 2024-03-15                   Keep temp files and package into TAR.GZ.
  $0 -p example.com:8080 2024-03-16   Publish to example.com on port 8080 after processing.

EOF
}

# Check if at least one argument is provided
if [ "$#" -eq 0 ]; then
    usage
    exit 1
fi

# Parse options
while getopts "fztp:kh" opt; do
    case $opt in
      f)
        FORCE_DOWNLOAD=true
        ;;
      z)
        FORMAT="zip"
        ;;
      t)
        FORMAT="tar.gz"
        ;;
      k)
        KEEP_TEMP=true
        ;;
      p)
        PUBLISH=true
        PUBLISH_TARGET=$OPTARG
        ;;
      h)  # Handle help option
        usage
        exit 0
        ;;
      \?)
        usage
        exit 1
        ;;
    esac
done

# Remove processed options from the positional parameters
shift $((OPTIND-1))

# Check if at least one date is provided after the options
if [ "$#" -eq 0 ]; then
    usage
fi

# Check if required directories exist, create if not
mkdir -p "$DOWNLOAD_DIR" "$OUTPUT_DIR" "$TEMP_DIR"

download_audio() {
    local date=$1
    local json_file="$SOURCE_DIR/${date}.json"
    local mp3_output="$DOWNLOAD_DIR/${date}.mp3"
    local checksum_file="$OUTPUT_DIR/${date}.checksum"
    
    # Make sure the json file exists
    if [ ! -f "$json_file" ]; then
        echo "File $json_file does not exists."
        return 1
    fi

    # Check if the checksum file exists and compare the checksums
    if [ -f "$mp3_output" ] && ! $FORCE_DOWNLOAD; then
        echo "File $mp3_output already exists and the JSON file has not changed. Skipping download. Use -f to force download."
        return 0
    fi
        
    echo "Downloading audio file for date: $date"
    local url=$(python -c "import json; print(json.load(open('${json_file}'))['source']['url'])")
    wget -O "$mp3_output" "$url"

    if [ $? -ne 0 ]; then
        echo "Download failed for $date, skipping processing."
        return 1
    fi
}

process_audio() {
    local date=$1
    local json_file="$SOURCE_DIR/${date}.json"
    local mp3_file="$DOWNLOAD_DIR/${date}.mp3"
    local output_dir="$TEMP_DIR/${date}"
    
    echo "Processing audio file for date: $date"

    mkdir -p $output_dir
    python "split_mp3/split_mp3.py" "$mp3_file" "$json_file" "$output_dir"
}

package_files() {
    local date=$1
    local source_dir="$TEMP_DIR/${date}"
    local output_file_base="$OUTPUT_DIR/${date}"

    echo "Packaging files for date: $date"

    if [ -f "${output_file_base}.${FORMAT}" ]; then
        rm "${output_file_base}.${FORMAT}"
    fi
    
    if [ "$FORMAT" == "zip" ]; then
        zip -j -r "${output_file_base}.zip" "$source_dir"
    elif [ "$FORMAT" == "tar.gz" ]; then
        tar -czf "${output_file_base}.tar.gz" -C "$source_dir" .
    fi

    echo "Packaging completed: ${output_file_base}.${FORMAT}"
}

publish_package() {

    if [ -z "$PUBLISH_TARGET" ]; then
        echo "No publish target set, skipping publish."
        return 0
    fi

    local date=$1
    local archive_path="${OUTPUT_DIR}/${date}.${FORMAT}"
    local url="http://$PUBLISH_TARGET/api/playlists"

    # Check if the archive exists
    if [ ! -f "$archive_path" ]; then
        echo "Archive $archive_path does not exist, cannot publish."
        return 1
    fi

    echo "Publishing $archive_path to $url"

    # Perform the POST request with curl
    local response=$(curl -s -o response.txt -w "%{http_code}" -X POST "$url" -H "Content-Type: multipart/form-data" -F "archive=@$archive_path")

    if [ "$response" -lt 200 ] || [ "$response" -ge 400 ]; then
        echo "Publish failed with status: $response"
        cat response.txt
        rm -f response.txt
        return 1
    else
        echo "Publish completed."
        rm -f response.txt
    fi
}

cleanup_temp_files() {
    local date=$1
    if ! $KEEP_TEMP; then
        echo "Cleaning up temporary files for date: $date"
        rm -rf "$TEMP_DIR/$date"
    fi
}

# Main loop to process each date
for date in "$@"; do
    download_audio "$date" && \
    process_audio "$date" && \
    package_files "$date" && \
    publish_package "$date" && \
    cleanup_temp_files "$date"
done
