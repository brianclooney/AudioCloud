
using AudioCloud.Shared.DTOs;

namespace AudioCloud.Client.Web.Models
{
    public enum PlaybackState
    {
        Playing,
        Complete,
        Unknown
    }

    public class RecordingsState
    {
        private TrackFilter _trackFilter = new TrackFilter();
        public TrackFilter Filter
        {
            get => _trackFilter;
            set
            {
                if (_trackFilter != value)
                {
                    _trackFilter = value;
                    OnTrackFilterChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private TrackResponseDto? _selectedTrack;
        public TrackResponseDto? SelectedTrack 
        { 
            get => _selectedTrack; 
            set
            {
                if (_selectedTrack != value) 
                {
                    _selectedTrack = value;
                    OnSelectedTrackChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private PlaybackState _playbackState = PlaybackState.Unknown;
        public PlaybackState PlaybackState
        {
            get => _playbackState;
            set
            {
                if (_playbackState != value)
                {
                    _playbackState = value;
                    OnPlaybackStateHasChange?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler? OnTrackFilterChanged;
        public event EventHandler? OnSelectedTrackChanged;
        public event EventHandler? OnPlaybackStateHasChange;
    }
}
