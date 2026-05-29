using AutoMapper;
using EventManagementApi.Data;
using EventManagementApi.DTOs.Event;
using EventManagementApi.DTOs.Organizer;
using EventManagementApi.DTOs.Ticket;
using EventManagementApi.Mappings;
using EventManagementApi.Models;
using EventManagementApi.Repositories;
using EventManagementApi.Services;
using Microsoft.EntityFrameworkCore;

namespace TicketApp.Tests.Integration
{
    // ═══════════════════════════════════════════
    // TicketService İnteqrasiya Testləri
    // ═══════════════════════════════════════════
    public class TicketServiceIntegrationTests : IDisposable
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly TicketService _service;

        public TicketServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _db = new AppDbContext(options);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _service = new TicketService(
                new TicketRepository(_db),
                new EventRepository(_db),
                _mapper);
        }

        private async Task<(Organizer org, Event ev)> SeedEventAsync()
        {
            var org = new Organizer { Name = "Org", Email = Guid.NewGuid() + "@test.com" };
            _db.Organizers.Add(org);
            await _db.SaveChangesAsync();

            var ev = new Event { Title = "Test Event", Date = DateTime.UtcNow.AddDays(10), Location = "Baku", OrganizerId = org.OrganizerId };
            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            return (org, ev);
        }

        [Fact]
        public async Task CreateAsync_WhenEventExists_SavesTicketToDb()
        {
            // ARRANGE
            var (_, ev) = await SeedEventAsync();
            var dto = new CreateTicketDto { Type = "VIP", Price = 50, QuantityAvailable = 100 };

            // ACT
            var result = await _service.CreateAsync(ev.EventId, dto);

            // ASSERT
            Assert.Equal("Bilet uğurla əlavə edildi!", result);
            Assert.Equal(1, await _db.Tickets.CountAsync());
        }

        [Fact]
        public async Task CreateAsync_WhenEventNotInDb_ReturnsErrorAndDbEmpty()
        {
            // ARRANGE
            var dto = new CreateTicketDto { Type = "Standard", Price = 20, QuantityAvailable = 50 };

            // ACT
            var result = await _service.CreateAsync(999, dto);

            // ASSERT
            Assert.Equal("Bu ID-li event mövcud deyil!", result);
            Assert.Equal(0, await _db.Tickets.CountAsync());
        }

        [Fact]
        public async Task DeleteAsync_WhenTicketExists_RemovesFromDb()
        {
            // ARRANGE
            var (_, ev) = await SeedEventAsync();
            var ticket = new Ticket { EventId = ev.EventId, Type = "VIP", Price = 100, QuantityAvailable = 10 };
            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();

            // ACT
            var result = await _service.DeleteAsync(ticket.TicketId);

            // ASSERT
            Assert.Equal("Bilet uğurla silindi!", result);
            Assert.Equal(0, await _db.Tickets.CountAsync());
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTicketsFromDb()
        {
            // ARRANGE
            var (_, ev) = await SeedEventAsync();
            _db.Tickets.AddRange(
                new Ticket { EventId = ev.EventId, Type = "VIP", Price = 100, QuantityAvailable = 5 },
                new Ticket { EventId = ev.EventId, Type = "Standard", Price = 30, QuantityAvailable = 50 }
            );
            await _db.SaveChangesAsync();

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task UpdateAsync_WhenTicketExists_UpdatesPriceInDb()
        {
            // ARRANGE
            var (_, ev) = await SeedEventAsync();
            var ticket = new Ticket { EventId = ev.EventId, Type = "VIP", Price = 50, QuantityAvailable = 10 };
            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();
            var dto = new UpdateTicketDto { Type = "Premium", Price = 150, QuantityAvailable = 10 };

            // ACT
            var result = await _service.UpdateAsync(ticket.TicketId, dto);

            // ASSERT
            Assert.Equal("Bilet uğurla yeniləndi!", result);
            var updated = await _db.Tickets.FindAsync(ticket.TicketId);
            Assert.Equal(150, updated!.Price);
            Assert.Equal("Premium", updated.Type);
        }

        public void Dispose() => _db.Dispose();
    }

    // ═══════════════════════════════════════════
    // EventService İnteqrasiya Testləri
    // ═══════════════════════════════════════════
    public class EventServiceIntegrationTests : IDisposable
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly EventService _service;

        public EventServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _db = new AppDbContext(options);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _service = new EventService(
                new EventRepository(_db),
                new OrganizerRepository(_db),
                _mapper);
        }

        [Fact]
        public async Task CreateAsync_WhenOrganizerExists_SavesEventToDb()
        {
            // ARRANGE
            var org = new Organizer { Name = "Orq MMC", Email = "orq@test.com" };
            _db.Organizers.Add(org);
            await _db.SaveChangesAsync();
            var dto = new CreateEventDto { Title = "Baku Jazz", Date = DateTime.UtcNow.AddDays(30), Location = "İçərişəhər", OrganizerId = org.OrganizerId };

            // ACT
            var result = await _service.CreateAsync(dto);

            // ASSERT
            Assert.Equal("Event uğurla əlavə edildi!", result);
            Assert.Equal(1, await _db.Events.CountAsync());
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsMappedDto()
        {
            // ARRANGE
            var org = new Organizer { Name = "Org", Email = "o@o.com" };
            _db.Organizers.Add(org);
            await _db.SaveChangesAsync();
            var ev = new Event { Title = "Rock Night", Date = DateTime.UtcNow.AddDays(7), Location = "Baku", OrganizerId = org.OrganizerId };
            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            // ACT
            var result = await _service.GetByIdAsync(ev.EventId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("Rock Night", result!.Title);
        }

        [Fact]
        public async Task DeleteAsync_WhenExists_RemovesFromDb()
        {
            // ARRANGE
            var org = new Organizer { Name = "Org", Email = "d@d.com" };
            _db.Organizers.Add(org);
            await _db.SaveChangesAsync();
            var ev = new Event { Title = "Festival", Date = DateTime.UtcNow.AddDays(5), Location = "Gəncə", OrganizerId = org.OrganizerId };
            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            // ACT
            var result = await _service.DeleteAsync(ev.EventId);

            // ASSERT
            Assert.Equal("Event uğurla silindi!", result);
            Assert.Equal(0, await _db.Events.CountAsync());
        }

        [Fact]
        public async Task UpdateAsync_WhenBothExist_UpdatesTitleInDb()
        {
            // ARRANGE
            var org = new Organizer { Name = "Org", Email = "u@u.com" };
            _db.Organizers.Add(org);
            await _db.SaveChangesAsync();
            var ev = new Event { Title = "Old Title", Date = DateTime.UtcNow.AddDays(5), Location = "Baku", OrganizerId = org.OrganizerId };
            _db.Events.Add(ev);
            await _db.SaveChangesAsync();
            var dto = new UpdateEventDto { Title = "New Title", Date = DateTime.UtcNow.AddDays(10), Location = "Sumqayıt", OrganizerId = org.OrganizerId };

            // ACT
            var result = await _service.UpdateAsync(ev.EventId, dto);

            // ASSERT
            Assert.Equal("Event uğurla yeniləndi!", result);
            var updated = await _db.Events.FindAsync(ev.EventId);
            Assert.Equal("New Title", updated!.Title);
        }

        public void Dispose() => _db.Dispose();
    }

    // ═══════════════════════════════════════════
    // OrganizerService İnteqrasiya Testləri
    // ═══════════════════════════════════════════
    public class OrganizerServiceIntegrationTests : IDisposable
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly OrganizerService _service;

        public OrganizerServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _db = new AppDbContext(options);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _service = new OrganizerService(new OrganizerRepository(_db), _mapper);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailDuplicate_ReturnsErrorAndDbHasOne()
        {
            // ARRANGE
            _db.Organizers.Add(new Organizer { Name = "Birinci", Email = "same@mail.com" });
            await _db.SaveChangesAsync();
            var dto = new CreateOrganizerDto { Name = "İkinci", Email = "same@mail.com" };

            // ACT
            var result = await _service.CreateAsync(dto);

            // ASSERT
            Assert.Equal("Bu e-poçt artıq istifadə olunur!", result);
            Assert.Equal(1, await _db.Organizers.CountAsync());
        }

        [Fact]
        public async Task CreateAsync_WhenEmailUnique_SavesToDb()
        {
            // ARRANGE
            var dto = new CreateOrganizerDto { Name = "Yeni Orq", Email = "unique@mail.com" };

            // ACT
            var result = await _service.CreateAsync(dto);

            // ASSERT
            Assert.Equal("Organizer uğurla əlavə edildi!", result);
            Assert.Equal(1, await _db.Organizers.CountAsync());
        }

        [Fact]
        public async Task UpdateAsync_WhenExists_UpdatesNameInDb()
        {
            // ARRANGE
            var org = new Organizer { Name = "Köhnə Ad", Email = "x@x.com" };
            _db.Organizers.Add(org);
            await _db.SaveChangesAsync();
            var dto = new UpdateOrganizerDto { Name = "Yeni Ad", Email = "x@x.com" };

            // ACT
            var result = await _service.UpdateAsync(org.OrganizerId, dto);

            // ASSERT
            Assert.Equal("Organizer uğurla yeniləndi!", result);
            var updated = await _db.Organizers.FindAsync(org.OrganizerId);
            Assert.Equal("Yeni Ad", updated!.Name);
        }

        public void Dispose() => _db.Dispose();
    }
}