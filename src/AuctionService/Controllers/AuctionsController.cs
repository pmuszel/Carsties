using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDBContext _context;
        private readonly IMapper _mapper;

        public AuctionsController(AuctionDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions(string date)
        {
            var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

            if(!String.IsNullOrEmpty(date))
            {
                query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            }

            return await query.ProjectTo<AuctionDTO>(_mapper.ConfigurationProvider).ToListAsync();

            // var auctions = await _context.Auctions
            //     .Include(x => x.Item)
            //     .OrderBy(x => x.Item.Make)
            //     .ToListAsync();

            //     return _mapper.Map<List<AuctionDTO>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if(auction == null) return NotFound();

            return _mapper.Map<AuctionDTO>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO auctionDTO)
        {
            var auction = _mapper.Map<Auction>(auctionDTO);

            //TODO: add curent user as seller

            auction.Seller = "test";

            _context.Auctions.Add(auction);

            var result = await _context.SaveChangesAsync() > 0;

            if(!result) return BadRequest("Could not save changes to the DB");

            return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, _mapper.Map<AuctionDTO>(auction));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDTO updateAuctionDTO)
        {
            var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if(auction == null) return NotFound();

            //TODO: check seller == username

            auction.Item.Make = updateAuctionDTO.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDTO.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDTO.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDTO.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDTO.Year ?? auction.Item.Year;

            var result = await _context.SaveChangesAsync() > 0;

            if(!result) return BadRequest("Could not update auction in the DB");

            return Ok();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);

            if(auction == null) return NotFound();

            //TODO: check seller == username

            _context.Auctions.Remove(auction);

            var result = await _context.SaveChangesAsync() > 0;

            if(!result) return BadRequest("Could not delete auction in the DB");

            return Ok();
        }
    }
}