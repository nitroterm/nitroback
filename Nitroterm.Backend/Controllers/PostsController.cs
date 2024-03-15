using Microsoft.AspNetCore.Mvc;
using Nitroterm.Backend.Algorithm;
using Nitroterm.Backend.Attributes;
using Nitroterm.Backend.Database;
using Nitroterm.Backend.Database.Models;
using Nitroterm.Backend.Dto;
using Nitroterm.Backend.Utilities;

namespace Nitroterm.Backend.Controllers;

[ApiController]
[Route("/api/v1/posts")]
public class PostsController : ControllerBase
{
    [HttpPost("/api/v1/post")]
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

        return Ok(new ResultDto<PostDto>(new PostDto(post)));
    }

    [HttpGet("{id:guid}")]
    public object GetPost(Guid id)
    {
        using NitrotermDbContext db = new();

        Post? post = db.GetPost(id);
        if (post == null) return NotFound(new ErrorResultDto("not_found", "post not found"));
        
        return Ok(new ResultDto<PostDto>(new PostDto(post)));
    }
}