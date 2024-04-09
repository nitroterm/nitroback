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

        if (string.IsNullOrWhiteSpace(postDto.Contents))
        {
            return BadRequest(new ErrorResultDto("invalid_format",
                "message format is invalid"));
        }

        if (postDto.Contents.Length > 4000)
        {
            return BadRequest(new ErrorResultDto("too_large",
                "message length must be less than 4000 characters"));
        }

        Post post = new()
        {
            PublicIdentifier = Guid.NewGuid(),
            Message = postDto.Contents,
            Sender = user
        };
        post.NitroLevel = AlgorithmManager.DeduceInitialNitroLevelForPost(post, user);

        db.Posts.Update(post);
        db.SaveChanges();

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

        if (!string.IsNullOrWhiteSpace(dto.Contents))
        {
            post.Message = dto.Contents;
            // TODO: mark post as edited
        }

        db.Update(post);
        db.SaveChanges();
        
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