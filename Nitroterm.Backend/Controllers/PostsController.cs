﻿using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nitroterm.Backend.Algorithm;
using Nitroterm.Backend.Attributes;
using Nitroterm.Backend.Database;
using Nitroterm.Backend.Database.Models;
using Nitroterm.Backend.Database.Models.WebSockets;
using Nitroterm.Backend.Dto;
using Nitroterm.Backend.Services;
using Nitroterm.Backend.Utilities;

namespace Nitroterm.Backend.Controllers;

[ApiController]
[Route("/api/nitroterm/v1/posts")]
public class PostsController : ControllerBase
{
    private IEventService _wsController;
    public PostsController(IEventService wsController)
    {
        _wsController = wsController;
    }
    
    /// <summary>
    /// Create a post
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(ResultDto<PostDto?>), 200)]
    [ProducesResponseType(typeof(ErrorResultDto), 401)]
    [ProducesResponseType(typeof(ErrorResultDto), 400)]
    [HttpPost("/api/nitroterm/v1/post")]
    [Authorize]
    public object CreatePost([FromBody] PostCreationDto postDto)
    {
        using NitrotermDbContext db = new();

        User? user = this.GetUser();
        if (user == null) return Forbid();

        if (!Utilities.Utilities.CheckUserContent(postDto.Contents))
        {
            return BadRequest(new ErrorResultDto("invalid_format",
                "message format is invalid : must be less than 4000 characters"));
        }

        Post post = new()
        {
            PublicIdentifier = Guid.NewGuid(),
            Message = postDto.Contents,
            Sender = user,
            CreationTimestamp = DateTime.Now
        };
        post.NitroLevel = AlgorithmManager.DeduceInitialNitroLevelForPost(post, user);

        db.Posts.Update(post);
        db.SaveChanges();
        
        // Send notification to all mentioned users
        string[] mentions = Utilities.Utilities.ParseMentions(post.Message);
        foreach (string mention in mentions)
        {
            User? mentionedUser = db.Users.FirstOrDefault(user => user.Username == mention);
            if (mentionedUser == null) continue;

            mentionedUser.SendNotification(db, new Notification
            {
                Title = $"{post.Sender.DisplayName}",
                Body = $"{post.Sender.DisplayName} mentioned you in their post"
            }, new Dictionary<string, string>
            {
                {"type", "post"},
                {"post", post.PublicIdentifier.ToString()}
            });
        }
        
        return Ok(new ResultDto<PostDto?>(new PostDto(post)));
    }

    /// <summary>
    /// Get a post
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(ResultDto<PostDto?>), 200)]
    [ProducesResponseType(typeof(ErrorResultDto), 401)]
    [ProducesResponseType(typeof(ErrorResultDto), 404)]
    [HttpGet("{id:guid}")]
    public object GetPost(Guid id)
    {
        using NitrotermDbContext db = new();

        Post? post = db.GetPost(id);
        if (post == null) return NotFound(new ErrorResultDto("not_found", "post not found"));

        return Ok(new ResultDto<PostDto?>(new PostDto(post)));
    }

    /// <summary>
    /// Update a post
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(ResultDto<PostDto?>), 200)]
    [ProducesResponseType(typeof(ErrorResultDto), 404)]
    [ProducesResponseType(typeof(ErrorResultDto), 401)]
    [HttpPut("{id:guid}")]
    [Authorize]
    public object Put(Guid id, [FromBody] PostCreationDto dto)
    {
        using NitrotermDbContext db = new();
        User user = this.GetUser()!;
        
        Post? post = db.GetPost(id);
        if (post == null) return NotFound(new ErrorResultDto("not_found", "post not found"));
        if (post.Sender.Id != user.Id) return NotFound(new ErrorResultDto("unauthorized", "this post doesn't belong to you"));
        
        if (!Utilities.Utilities.CheckUserContent(dto.Contents))
        {
            return BadRequest(new ErrorResultDto("invalid_format",
                "message format is invalid : must be less than 4000 characters"));
        }
        
        post.Message = dto.Contents;
        post.LastEditionTimestamp = DateTime.Now;
        post.Edited = true;

        db.Update(post);
        db.SaveChanges();
        
        return new ResultDto<PostDto?>(new PostDto(post));
    }

    /// <summary>
    /// Nitronize (upvote) a post
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(ResultDto<PostDto?>), 200)]
    [ProducesResponseType(typeof(ErrorResultDto), 404)]
    [ProducesResponseType(typeof(ErrorResultDto), 401)]
    [ProducesResponseType(typeof(ErrorResultDto), 400)]
    [HttpPost("{id:guid}/nitronize")]
    [Authorize]
    public object NitronizePost(Guid id)
    {
        using NitrotermDbContext db = new();
        User user = this.GetUser()!;
        
        Post? post = db.GetPost(id);
        if (post == null) return NotFound(new ErrorResultDto("not_found", "post not found"));

        if (!db.InteractWithPost(user, post, UserToPostInteractionType.Nitro))
        {
            if (!db.InteractWithPost(user, post, UserToPostInteractionType.None))
                return BadRequest(new ErrorResultDto("duplicate_interaction", "interaction is duplicate"));
        }

        WebSocketEvent<PostDto> wsEvent = new("post_update", new PostDto(post));
        _wsController.SendEvent(wsEvent);
        
        return new ResultDto<PostDto?>(new PostDto(post));
    }

    /// <summary>
    /// Dynamite (downvote) a post
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(ResultDto<PostDto?>), 200)]
    [ProducesResponseType(typeof(ErrorResultDto), 404)]
    [ProducesResponseType(typeof(ErrorResultDto), 401)]
    [ProducesResponseType(typeof(ErrorResultDto), 400)]
    [HttpPost("{id:guid}/dynamite")]
    [Authorize]
    public object DynamitePost(Guid id)
    {
        using NitrotermDbContext db = new();
        User user = this.GetUser()!;
        
        Post? post = db.GetPost(id);
        if (post == null) return NotFound(new ErrorResultDto("not_found", "post not found"));

        if (!db.InteractWithPost(user, post, UserToPostInteractionType.Dynamite))
        {
            if (!db.InteractWithPost(user, post, UserToPostInteractionType.None))
                return BadRequest(new ErrorResultDto("duplicate_interaction", "interaction is duplicate"));
        }

        WebSocketEvent<PostDto> wsEvent = new("post_update", new PostDto(post));
        _wsController.SendEvent(wsEvent);
        
        return new ResultDto<PostDto?>(new PostDto(post));
    }

    /// <summary>
    /// Delete a post
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(ResultDto<PostDto?>), 200)]
    [ProducesResponseType(typeof(ErrorResultDto), 404)]
    [ProducesResponseType(typeof(ErrorResultDto), 401)]
    [ProducesResponseType(typeof(ErrorResultDto), 400)]
    [HttpDelete("{id:guid}")]
    [Authorize]
    public object Delete(Guid id)
    {
        using NitrotermDbContext db = new();
        User user = this.GetUser()!;
        
        Post? post = db.GetPost(id);
        if (post == null) return NotFound(new ErrorResultDto("not_found", "post not found"));
        if (post.Sender.Id != user.Id) return NotFound(new ErrorResultDto("unauthorized", "this post doesn't belong to you"));

        db.Remove(post);
        db.SaveChanges();
        
        return new ResultDto<PostDto?>(new PostDto(post));
    }
}