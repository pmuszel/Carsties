using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Util;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests
{
    [Collection("Shared collection")]
    public class AuctionControllerTests : IAsyncLifetime
    {
        private readonly CustomWebAppFactory _factory;
        private readonly HttpClient _httpClient;
        private const string GT_ID = "afbee524-5972-4075-8800-7d1f9d7b0a0c";

        public AuctionControllerTests(CustomWebAppFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetAuctions_ShouldReturn3Auctions()
        {
            //arrenge
            //--

            //act
            List<AuctionDTO> response =  await _httpClient.GetFromJsonAsync<List<AuctionDTO>>("api/auctions");

            //assert
            Assert.Equal(3, response.Count);
        }

        [Fact]
        public async Task GetAuctionById_WithValidId_ShouldReturnAuction()
        {
            //arrenge?                  

            //act
            var response =  await _httpClient.GetFromJsonAsync<AuctionDTO>($"api/auctions/{GT_ID}");

            //assert
            Assert.Equal("GT", response.Model);
        }

        [Fact]
        public async Task GetAuctionById_WithInvalidId_ShouldReturn404()
        {
            //arrenge?                  

            //act
            var response =  await _httpClient.GetAsync($"api/auctions/{Guid.NewGuid()}");

            //assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAuctionById_WithInvalidGuid_ShouldReturn400()
        {
            //arrenge?                  

            //act
            var response =  await _httpClient.GetAsync("api/auctions/not-a-guid");

            //assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateAuction_WithNoAuth_ShouldReturn401()
        {
            //arrenge
            var auction = new CreateAuctionDTO{Make = "test"};                  

            //act
            var response =  await _httpClient.PostAsJsonAsync("api/auctions", auction);

            //assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateAuction_WithAuth_ShouldReturn201()
        {
            //arrenge
            var auction = GetAuctionForCreate();
            _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));                  

            //act
            var response =  await _httpClient.PostAsJsonAsync("api/auctions", auction);

            //assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDTO>();
            Assert.Equal("bob", createdAuction.Seller);
        }

        [Fact]
        public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturn400()
        {
            //arrenge
            var auction = GetAuctionForCreate();
            auction.Make = null;
            _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));                  

            //act
            var response =  await _httpClient.PostAsJsonAsync("api/auctions", auction);

            //assert

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn200()
        {
            // arrange
            var auction = GetAuctionForUpdate();
            _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

            // act
            var response =  await _httpClient.PutAsJsonAsync($"api/auctions/{GT_ID}", auction);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
        {
            // arrange 
            var auction = GetAuctionForUpdate();
            _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("alice"));

            // act
            var response =  await _httpClient.PutAsJsonAsync($"api/auctions/{GT_ID}", auction);

            // assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        public Task InitializeAsync() => Task.CompletedTask;
        public Task DisposeAsync()
        {
            using var scope = _factory.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AuctionDBContext>();

            DbHelper.ReinitDbForTests(db);

            return Task.CompletedTask;
        }

        private CreateAuctionDTO GetAuctionForCreate()
        {
            return new CreateAuctionDTO
            {
                Make = "test",
                Model = "testModel",
                ImageUrl = "test",
                Color = "test",
                Mileage = 10,
                Year = 10,
                ReservePrice = 10,
            };
        }

        private UpdateAuctionDTO GetAuctionForUpdate()
        {
            return new UpdateAuctionDTO
            {
                Make = "testUpdate",
                Model = "testModel",
                Color = "testUpdate",
                Mileage = 20,
                Year = 20,
            };
        }

    }
}