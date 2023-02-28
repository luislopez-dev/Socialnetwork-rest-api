using AutoMapper;
using datingApp.Data;
using datingApp.DTOs;
using datingApp.Extensions;
using datingApp.Helpers;
using datingApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Controllers;

public class MessagesController : BaseApiController
{

    private readonly IUnitOfWork _uoWork;
    private readonly IMapper _mapper;

    public MessagesController(IUnitOfWork uoWork, IMapper mapper)
    {
        _uoWork = uoWork;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();
        if (username == createMessageDto.RecipientUsername.ToLower())
        {
            return BadRequest("You can not send messages to yourself");
        }

        var sender = await _uoWork.UserRepository.GetUserByUserNameAsync(username);
        var recipient = await _uoWork.UserRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);
        if (recipient == null)
        {
            return NotFound();
        }

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };
        _uoWork.MessageRepository.AddMessage(message);
        if (await _uoWork.Complete())
        {
            return Ok(_mapper.Map<MessageDto>(message));
        }

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();
        var messages = await _uoWork.MessageRepository.GetMessagesForUser(messageParams);
        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage,
            messages.PageSize, messages.TotalCount, messages.TotalPages));
        return messages;
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();
        var message = await _uoWork.MessageRepository.GetMessage(id);
        if (message.SenderUsername != username && message.RecipientUsername != username) 
            return Unauthorized();
        if (message.SenderUsername == username)
        {
            message.SenderDeleted = true;
        }
        if (message.RecipientUsername == username)
        {
            message.RecipientDeleted = true;
        }
        if (message.SenderDeleted && message.RecipientDeleted)
        {
            _uoWork.MessageRepository.DeleteMessage(message);
        }

        if (await _uoWork.Complete())
        {
            return Ok();
        }

        return BadRequest("Problem deleting message");
        
    }
}