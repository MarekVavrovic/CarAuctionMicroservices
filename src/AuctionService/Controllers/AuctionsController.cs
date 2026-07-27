
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AuctionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController(IAuctionRepository repo, IMapper mapper, IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions(string? date)
    {
        return await repo.GetAuctionAsync(date);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
    {
        var auction = await repo.GetAuctionByIdAsync(id);

        if (auction == null)return NotFound();
        
        return auction;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction([FromBody] CreateAuctionDto auctionDto)
    {
        var auction = mapper.Map<Auction>(auctionDto);
        auction.Seller = User.Identity?.Name ?? throw new InvalidOperationException("User not found");

        repo.AddAuction(auction);

        //CREATE CONSUMER
        var newAuction = mapper.Map<AuctionDto>(auction);
        
        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));

        var result = await repo.SaveChangesAsync();

        if (!result)
        {
            return BadRequest("Failed to create auction");
        }

        return CreatedAtAction(nameof(GetAuction), new { id = auction.Id }, newAuction);

    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, [FromBody] UpdateAuctionDto auctionDto)
    {
        var auction = await repo.GetAuctionEntityById(id);

        if (auction == null)
        {
            return NotFound();
        }

        // TODO: check seller is the same as current user
        if (auction.Seller != User.Identity?.Name) return Forbid();

        //updating props
        auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = auctionDto.Make ?? auction.Item.Model;
        auction.Item.Year = auctionDto.Year;
        auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = auctionDto.Mileage;

        await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auction));

        var result = await repo.SaveChangesAsync();

        if (!result)
        {
            return BadRequest("Failed to update auction");
        }

        return Ok(mapper.Map<AuctionDto>(auction));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await repo.GetAuctionEntityById(id);
        if (auction == null)
        {
            return NotFound();
        }

        // TODO: check seller is the same as current user
        if (auction.Seller != User.Identity?.Name) return Forbid();
        repo.RemoveAuction(auction);

        await publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

        var result = await repo.SaveChangesAsync();      
        if (!result)
        {
            return BadRequest("Failed to delete auction");
        }
        return Ok();
    }

}
