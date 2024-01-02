using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuctionsController(IAuctionRepository repo, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _repo = repo;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }


        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions(string date)
        {
            return await _repo.GetAuctionsAsync(date);

            // var auctions = await _context.Auctions
            //     .Include(x => x.Item)
            //     .OrderBy(x => x.Item.Make)
            //     .ToListAsync();

            //     return _mapper.Map<List<AuctionDTO>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
        {
            var auction = await _repo.GetAuctionById(id);

            if(auction == null) return NotFound();

            return auction;
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO auctionDTO)
        {
            var auction = _mapper.Map<Auction>(auctionDTO);

            auction.Seller = User.Identity.Name;

            _repo.AddAuction(auction);

            var newAuction = _mapper.Map<AuctionDTO>(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

            var result = await _repo.SaveChangesAsync();

            if(!result) return BadRequest("Could not save changes to the DB");

            return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, _mapper.Map<AuctionDTO>(auction));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDTO updateAuctionDTO)
        {
            var auction = await _repo.GetAuctionEntityById(id);

            if(auction == null) return NotFound();

            if(auction.Seller != User.Identity.Name) return Forbid();

            auction.Item.Make = updateAuctionDTO.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDTO.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDTO.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDTO.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDTO.Year ?? auction.Item.Year;

            var auctionUpdated = _mapper.Map<AuctionUpdated>(updateAuctionDTO);
            auctionUpdated.Id = id.ToString();

            await _publishEndpoint.Publish(auctionUpdated);

            var result = await _repo.SaveChangesAsync();

            if(!result) return BadRequest("Could not update auction in the DB");

            return Ok();

        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _repo.GetAuctionEntityById(id);

            if(auction == null) return NotFound();

            if(auction.Seller != User.Identity.Name) return Forbid();

            try
            {
                await _publishEndpoint.Publish<AuctionDeleted>(new {Id = auction.Id.ToString()});

                _repo.RemoveAuction(auction);

                var result = await _repo.SaveChangesAsync();

                if(!result) return BadRequest("Could not delete auction in the DB");
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

            return Ok();
        }
    }
}