using datingApp.Data;
using datingApp.DTOs;
using datingApp.Helpers;

namespace datingApp.Interfaces;

public interface IMessageRepository
{
    public void AddMessage(Message message);
    public void DeleteMessage(Message message);
    public Task<Message> GetMessage(int id);
    public Task<PagedList<MessageDto>> GetMessagesForUser();
    public Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId);
    public Task<bool> SaveAllAsync();
}