using Mrp.Models;

namespace Mrp.Data.Interfaces;

public interface IMediaRepository
{
    MediaEntry Create(MediaEntry entry);
    MediaEntry? GetById(int id);
    List<MediaEntry> GetAll();
    bool Update(MediaEntry entry);
    bool Delete(int id);
}