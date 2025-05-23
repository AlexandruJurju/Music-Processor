﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MusicProcessor.Persistence.Common;

#nullable disable

namespace MusicProcessor.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250329174706_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("MusicProcessor.Domain.Albums.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int>("ArtistId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("artist_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_albums");

                    b.HasIndex("ArtistId")
                        .HasDatabaseName("ix_albums_artist_id");

                    b.ToTable("albums", (string)null);
                });

            modelBuilder.Entity("MusicProcessor.Domain.Artists.Artist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_artists");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_artists_name");

                    b.ToTable("artists", (string)null);
                });

            modelBuilder.Entity("MusicProcessor.Domain.GenreCategories.GenreCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_genre_categories");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_genre_categories_name");

                    b.ToTable("genre_categories", (string)null);
                });

            modelBuilder.Entity("MusicProcessor.Domain.Genres.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<bool>("RemoveFromSongs")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(false)
                        .HasColumnName("remove_from_songs");

                    b.HasKey("Id")
                        .HasName("pk_genres");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_genres_name");

                    b.ToTable("genres", (string)null);
                });

            modelBuilder.Entity("MusicProcessor.Domain.SongsMetadata.SongMetadata", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int?>("AlbumId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("album_id");

                    b.Property<int?>("Date")
                        .HasColumnType("INTEGER")
                        .HasColumnName("date");

                    b.Property<int>("DiscCount")
                        .HasColumnType("INTEGER")
                        .HasColumnName("disc_count");

                    b.Property<int>("DiscNumber")
                        .HasColumnType("INTEGER")
                        .HasColumnName("disc_number");

                    b.Property<int>("Duration")
                        .HasColumnType("INTEGER")
                        .HasColumnName("duration");

                    b.Property<string>("ISRC")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("TEXT")
                        .HasColumnName("isrc");

                    b.Property<int>("MainArtistId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("main_artist_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<int>("TrackNumber")
                        .HasColumnType("INTEGER")
                        .HasColumnName("track_number");

                    b.Property<int>("TracksCount")
                        .HasColumnType("INTEGER")
                        .HasColumnName("tracks_count");

                    b.HasKey("Id")
                        .HasName("pk_songs");

                    b.HasIndex("AlbumId")
                        .HasDatabaseName("ix_songs_album_id");

                    b.HasIndex("MainArtistId")
                        .HasDatabaseName("ix_songs_main_artist_id");

                    b.HasIndex("Name")
                        .HasDatabaseName("ix_songs_name");

                    b.ToTable("songs", (string)null);
                });

            modelBuilder.Entity("genre_genre_category", b =>
                {
                    b.Property<int>("GenreCategoriesId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("genre_categories_id");

                    b.Property<int>("GenresId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("genres_id");

                    b.HasKey("GenreCategoriesId", "GenresId")
                        .HasName("pk_genre_genre_category");

                    b.HasIndex("GenresId")
                        .HasDatabaseName("ix_genre_genre_category_genres_id");

                    b.ToTable("genre_genre_category", (string)null);
                });

            modelBuilder.Entity("song_artists", b =>
                {
                    b.Property<int>("ArtistsId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("artists_id");

                    b.Property<int>("SongsId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("songs_id");

                    b.HasKey("ArtistsId", "SongsId")
                        .HasName("pk_song_artists");

                    b.HasIndex("SongsId")
                        .HasDatabaseName("ix_song_artists_songs_id");

                    b.ToTable("song_artists", (string)null);
                });

            modelBuilder.Entity("song_genres", b =>
                {
                    b.Property<int>("GenresId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("genres_id");

                    b.Property<int>("SongsId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("songs_id");

                    b.HasKey("GenresId", "SongsId")
                        .HasName("pk_song_genres");

                    b.HasIndex("SongsId")
                        .HasDatabaseName("ix_song_genres_songs_id");

                    b.ToTable("song_genres", (string)null);
                });

            modelBuilder.Entity("MusicProcessor.Domain.Albums.Album", b =>
                {
                    b.HasOne("MusicProcessor.Domain.Artists.Artist", "Artist")
                        .WithMany()
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_albums_artists_artist_id");

                    b.Navigation("Artist");
                });

            modelBuilder.Entity("MusicProcessor.Domain.SongsMetadata.SongMetadata", b =>
                {
                    b.HasOne("MusicProcessor.Domain.Albums.Album", "Album")
                        .WithMany("Songs")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_songs_albums_album_id");

                    b.HasOne("MusicProcessor.Domain.Artists.Artist", "MainArtist")
                        .WithMany()
                        .HasForeignKey("MainArtistId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_songs_artists_main_artist_id");

                    b.OwnsOne("MusicProcessor.Domain.SongsMetadata.SpotifyInfo", "SpotifyInfo", b1 =>
                        {
                            b1.Property<int>("SongMetadataId")
                                .HasColumnType("INTEGER")
                                .HasColumnName("id");

                            b1.Property<string>("SpotifyAlbumId")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("TEXT")
                                .HasColumnName("spotify_info_spotify_album_id");

                            b1.Property<string>("SpotifyArtistId")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("TEXT")
                                .HasColumnName("spotify_info_spotify_artist_id");

                            b1.Property<string>("SpotifyCoverUrl")
                                .IsRequired()
                                .HasMaxLength(500)
                                .HasColumnType("TEXT")
                                .HasColumnName("spotify_info_spotify_cover_url");

                            b1.Property<string>("SpotifySongId")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("TEXT")
                                .HasColumnName("spotify_info_spotify_song_id");

                            b1.Property<string>("SpotifySongUrl")
                                .IsRequired()
                                .HasMaxLength(500)
                                .HasColumnType("TEXT")
                                .HasColumnName("spotify_info_spotify_song_url");

                            b1.HasKey("SongMetadataId");

                            b1.ToTable("songs");

                            b1.WithOwner()
                                .HasForeignKey("SongMetadataId")
                                .HasConstraintName("fk_songs_songs_id");
                        });

                    b.Navigation("Album");

                    b.Navigation("MainArtist");

                    b.Navigation("SpotifyInfo");
                });

            modelBuilder.Entity("genre_genre_category", b =>
                {
                    b.HasOne("MusicProcessor.Domain.GenreCategories.GenreCategory", null)
                        .WithMany()
                        .HasForeignKey("GenreCategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_genre_genre_category_genre_categories_genre_categories_id");

                    b.HasOne("MusicProcessor.Domain.Genres.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_genre_genre_category_genres_genres_id");
                });

            modelBuilder.Entity("song_artists", b =>
                {
                    b.HasOne("MusicProcessor.Domain.Artists.Artist", null)
                        .WithMany()
                        .HasForeignKey("ArtistsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_song_artists_artists_artists_id");

                    b.HasOne("MusicProcessor.Domain.SongsMetadata.SongMetadata", null)
                        .WithMany()
                        .HasForeignKey("SongsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_song_artists_songs_songs_id");
                });

            modelBuilder.Entity("song_genres", b =>
                {
                    b.HasOne("MusicProcessor.Domain.Genres.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_song_genres_genres_genres_id");

                    b.HasOne("MusicProcessor.Domain.SongsMetadata.SongMetadata", null)
                        .WithMany()
                        .HasForeignKey("SongsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_song_genres_songs_songs_id");
                });

            modelBuilder.Entity("MusicProcessor.Domain.Albums.Album", b =>
                {
                    b.Navigation("Songs");
                });
#pragma warning restore 612, 618
        }
    }
}
