using Microsoft.AspNetCore.Mvc;
using Nitroterm.Backend.Algorithm;
using Nitroterm.Backend.Attributes;
using Nitroterm.Backend.Database.Models;
using Nitroterm.Backend.Dto;
using Nitroterm.Backend.Utilities;

namespace Nitroterm.Backend.Controllers;

[ApiController]
[Route("/api/nitroterm/v1/feed")]
public class FeedController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public object Get()
    {
        return new ResultDto<PostDto[]?>(AlgorithmManager.DeduceBestPostsForUser(this.GetUser()!, 15)
            .Select(post => new PostDto(post)).ToArray());
    }
    
    [HttpGet("mine")]
    [Authorize]
    public object GetMine()
    {
        return new ResultDto<PostDto[]?>(AlgorithmManager.DeduceBestPostsFromUser(this.GetUser()!, 15)
            .Select(post => new PostDto(post)).ToArray());
    }
}