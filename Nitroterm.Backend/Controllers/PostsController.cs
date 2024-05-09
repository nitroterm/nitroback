using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nitroterm.Backend.Algorithm;
using Nitroterm.Backend.Attributes;
using Nitroterm.Backend.Database;
using Nitroterm.Backend.Database.Models;
using Nitroterm.Backend.Dto;
using Nitroterm.Backend.Utilities;

namespace Nitroterm.Backend.Controllers;

[ApiController]
[Route("/api/nitroterm/v1/posts")]
public class PostsController : ControllerBase
{
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

    [HttpGet("{id:guid}")]
    public object GetPost(Guid id)
    {
        using NitrotermDbContext db = new();

        Post? post = db.GetPost(id);
        if (post == null) return NotFound(new ErrorResultDto("not_found", "post not found"));

        return Ok(new ResultDto<PostDto?>(new PostDto(post)));
    }

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

    [HttpPost("{id:guid}/nitronize")]
    [Authorize]
    public object NitronizePost(Guid id)
    {
        using NitrotermDbContext db = new();
        User user = this.GetUser()!;
        
        Post? post = db.GetPost(id);
        if (post == null) return NotFound(new ErrorResultDto("not_found", "post not found"));

        if (!db.InteractWithPost(user, post, UserToPostInteractionType.Nitro))
            return new ErrorResultDto("duplicate_interaction", "interaction is duplicate");
        
        return new ResultDto<PostDto?>(new PostDto(post));
    }

    [HttpPost("{id:guid}/dynamite")]
    [Authorize]
    public object DynamitePost(Guid id)
    {
        using NitrotermDbContext db = new();
        User user = this.GetUser()!;
        
        Post? post = db.GetPost(id);
        if (post == null) return NotFound(new ErrorResultDto("not_found", "post not found"));

        if (!db.InteractWithPost(user, post, UserToPostInteractionType.Dynamite))
            return new ErrorResultDto("duplicate_interaction", "interaction is duplicate");
        
        return new ResultDto<PostDto?>(new PostDto(post));
    }

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