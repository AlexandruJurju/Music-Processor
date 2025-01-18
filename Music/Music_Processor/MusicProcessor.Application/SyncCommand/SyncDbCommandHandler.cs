using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.Factories;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.SyncCommand;

public sealed class SyncDbCommandHandler : IRequestHandler<SyncDbCommand>
{
    private readonly IArtistRepository _artistRepository;
    private readonly IFileService _fileService;
    private readonly IGenreRepository _genreRepository;
    private readonly MetadataHandlerFactory _metadataHandlerFactory;
    private readonly ISpotDLMetadataLoader _spotDlMetadataLoader;
    private readonly ISongRepository _songRepository;
    private readonly IStyleRepository _styleRepository;

    private Dictionary<string, DateTime?> _existingSongs = new();
    private List<Artist> _existingArtists = new();
    private List<Genre> _existingGenres = new();
    private List<Style> _existingStyles = new();
    private Dictionary<string, Song> _spotDLMetadata = new();

    public SyncDbCommandHandler(
        IFileService fileService,
        MetadataHandlerFactory metadataHandlerFactory,
        ISongRepository songRepository,
        IGenreRepository genreRepository,
        IStyleRepository styleRepository,
        IArtistRepository artistRepository,
        ISpotDLMetadataLoader spotDlMetadataLoader)
    {
        _fileService = fileService;
        _metadataHandlerFactory = metadataHandlerFactory;
        _songRepository = songRepository;
        _genreRepository = genreRepository;
        _styleRepository = styleRepository;
        _artistRepository = artistRepository;
        _spotDlMetadataLoader = spotDlMetadataLoader;
    }

    public async Task Handle(SyncDbCommand request, CancellationToken cancellationToken)
    {
        var songsToAdd = new List<Song>();
        var playlistSongs = _fileService.GetAllAudioFilesInFolder(request.PlaylistPath);

        await LoadExistingData(request);

        foreach (var songFile in playlistSongs)
        {
            await ProcessSongFile(songFile, songsToAdd);

            // Add the songs in batches
            if (songsToAdd.Count >= 100)
            {
                await _songRepository.AddRangeAsync(songsToAdd);
                songsToAdd.Clear();
            }
        }

        if (songsToAdd.Any())
        {
            await _songRepository.AddRangeAsync(songsToAdd);
        }
    }

    private async Task LoadExistingData(SyncDbCommand request)
    {
        _existingSongs = _songRepository.GetAll()
            .ToDictionary(
                s => s.Title,
                s => s.DateModified
            );
        _spotDLMetadata = await _spotDlMetadataLoader.LoadSpotDLMetadataAsync(request.PlaylistPath);
        _existingArtists = await _artistRepository.GetAllAsync();
        _existingGenres = await _genreRepository.GetAllAsync();
        _existingStyles = await _styleRepository.GetAllAsync();
    }

    private async Task ProcessSongFile(string songFile, List<Song> songsToAdd)
    {
        var handler = _metadataHandlerFactory.GetHandler(songFile);
        var metadata = handler.ExtractMetadata(songFile);

        // Get the file's last modification time
        var fileLastModified = File.GetLastWriteTimeUtc(songFile);

        if (_existingSongs.TryGetValue(metadata.Title, out var lastDbUpdate))
        {
            // Skip if the file hasn't been modified since last DB update
            if (lastDbUpdate.HasValue && fileLastModified <= lastDbUpdate.Value)
            {
                return;
            }
        }

        var song = new Song(
            metadata.Title,
            metadata.Album,
            metadata.Year,
            metadata.Comment,
            metadata.TrackNumber,
            metadata.Duration,
            metadata.FileType
        );

        _existingSongs.Add(song.Title, DateTime.UtcNow);

        song.Artists = ProcessArtists(metadata.Artists);
        song.Genres = ProcessGenres(metadata.Genres);
        song.Styles = ProcessStyles(metadata.Styles);

        songsToAdd.Add(song);
    }

    private ICollection<Artist> ProcessArtists(IEnumerable<Artist> newArtists)
    {
        var processedArtists = new List<Artist>();

        foreach (var artist in newArtists)
        {
            var existingArtist = _existingArtists.FirstOrDefault(a => string.Equals(a.Name, artist.Name, StringComparison.OrdinalIgnoreCase));

            if (existingArtist != null)
            {
                processedArtists.Add(existingArtist);
            }
            else
            {
                var newArtist = new Artist { Name = artist.Name };
                _existingArtists.Add(newArtist);
                processedArtists.Add(newArtist);
            }
        }

        return processedArtists;
    }

    private ICollection<Genre> ProcessGenres(IEnumerable<Genre> newGenres)
    {
        var processedGenres = new List<Genre>();

        foreach (var genre in newGenres)
        {
            var existingGenre = _existingGenres.FirstOrDefault(g => string.Equals(g.Name, genre.Name, StringComparison.OrdinalIgnoreCase));

            if (existingGenre != null)
            {
                processedGenres.Add(existingGenre);
            }
            else
            {
                var newGenre = new Genre { Name = genre.Name };
                _existingGenres.Add(newGenre);
                processedGenres.Add(newGenre);
            }
        }

        return processedGenres;
    }

    private ICollection<Style> ProcessStyles(IEnumerable<Style> newStyles)
    {
        var processedStyles = new List<Style>();

        foreach (var style in newStyles)
        {
            var existingStyle = _existingStyles.FirstOrDefault(s => string.Equals(s.Name, style.Name, StringComparison.OrdinalIgnoreCase));

            if (existingStyle != null)
            {
                processedStyles.Add(existingStyle);
            }
            else
            {
                var newStyle = new Style { Name = style.Name };
                _existingStyles.Add(newStyle);
                processedStyles.Add(newStyle);
            }
        }

        return processedStyles;
    }
}