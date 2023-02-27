using datingApp.Data;
using datingApp.DTOs;
using datingApp.Helpers;

namespace datingApp.Interfaces;

public interface IMessageRepository
{
    public void AddMessage(Message message);
    public void DeleteMessage(Message message);
    public Task<Message> GetMessage(int id);
    public Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
    public Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName);
}