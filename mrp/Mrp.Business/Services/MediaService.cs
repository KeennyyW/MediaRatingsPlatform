using System;
using Mrp.Data.Interfaces;
using Mrp.Models;

namespace Mrp.Business.Services;

public class MediaService
{
    private readonly IMediaRepository _repository;

    public MediaService(IMediaRepository repository)
    {
        _repository = repository;
    }

    public MediaEntry Create(MediaEntry entry, int userId)
    {
        Validate(entry);
        entry.CreatorId = userId;
        return _repository.Create(entry);
    }

    public MediaEntry GetById(int id)
    {
        MediaEntry? entry = _repository.GetById(id);

        if (entry == null)
        {
            throw new KeyNotFoundException("Media not found");
        
        }
        return entry;
    }

    public List<MediaEntry> GetAll()
    {
        return _repository.GetAll();
    }

    public MediaEntry Update(int id, MediaEntry newData, int userId)
    {
        Validate(newData);

        MediaEntry oldData = GetById(id);
        if (oldData.CreatorId != userId)
        {
            throw new UnauthorizedAccessException("Not your media entry");
        }

        newData.Id = id;
        _repository.Update(newData);
        newData.CreatorId = userId;
        return newData;
    }

    public void Delete(int id, int userId)
    {

        MediaEntry? entry = _repository.GetById(id);
        if (entry == null)
        {
            throw new KeyNotFoundException("Media entry not found");
        }

        if (entry.CreatorId != userId)
        {
            throw new UnauthorizedAccessException("Not your media entry");
        }

        _repository.Delete(id);
        return;
    }


    private void Validate(MediaEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.Title))
        {
            throw new ArgumentException("Title is required");
        }
        if (entry.MediaType != "movie" && entry.MediaType != "series" && entry.MediaType != "game")
        {
            throw new ArgumentException("MediaType must be movie, series or game");
        }
        if (entry.ReleaseYear < 0 || entry.ReleaseYear > DateTime.Now.Year + 5)
        {
            throw new ArgumentException("Release Year is invalid");
        }
        if (entry.AgeRestriction != 0 && entry.AgeRestriction != 6 && entry.AgeRestriction != 12
            && entry.AgeRestriction != 16 && entry.AgeRestriction != 18)
        {
            throw new ArgumentException("AgeRestriction must be 0, 6, 12, 16 or 18");
        }
    }
}
