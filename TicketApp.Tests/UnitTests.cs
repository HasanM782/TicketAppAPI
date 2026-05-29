using AutoMapper;
using EventManagementApi.DTOs.Event;
using EventManagementApi.DTOs.Organizer;
using EventManagementApi.DTOs.Ticket;
using EventManagementApi.Mappings;
using EventManagementApi.Models;
using EventManagementApi.Repositories;
using EventManagementApi.Services;
using EventManagementApi.Validators;
using Moq;

namespace TicketApp.Tests.Unit
{
    // ═══════════════════════════════════════════
    // TicketService Unit Testləri
    // ═══════════════════════════════════════════
    public class TicketServiceUnitTests
    {
        private readonly Mock<ITicketRepository> _ticketRepo;
        private readonly Mock<IEventRepository> _eventRepo;
        private readonly IMapper _mapper;
        private readonly TicketService _service;

        public TicketServiceUnitTests()
        {
            _ticketRepo = new Mock<ITicketRepository>();
            _eventRepo = new Mock<IEventRepository>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
            _service = new TicketService(_ticketRepo.Object, _eventRepo.Object, _mapper);
        }

        [Fact]
        public async Task CreateAsync_WhenEventNotFound_ReturnsError()
        {
            // ARRANGE
            _eventRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Event?)null);
            var dto = new CreateTicketDto { Type = "VIP", Price = 50, QuantityAvailable = 10 };

            // ACT
            var result = await _service.CreateAsync(99, dto);

            // ASSERT
            Assert.Equal("Bu ID-li event mövcud deyil!", result);
        }

        [Fact]
        public async Task CreateAsync_WhenEventExists_ReturnsSuccess()
        {
            // ARRANGE
            var fakeEvent = new Event { EventId = 1, Title = "Baku Fest", Date = DateTime.UtcNow.AddDays(5), Location = "Baku", OrganizerId = 1 };
            _eventRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeEvent);
            _ticketRepo.Setup(r => r.AddAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            var dto = new CreateTicketDto { Type = "VIP", Price = 50, QuantityAvailable = 10 };

            // ACT
            var result = await _service.CreateAsync(1, dto);

            // ASSERT
            Assert.Equal("Bilet uğurla əlavə edildi!", result);
            _ticketRepo.Verify(r => r.AddAsync(It.IsAny<Ticket>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenTicketNotFound_ReturnsError()
        {
            // ARRANGE
            _ticketRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Ticket?)null);

            // ACT
            var result = await _service.DeleteAsync(999);

            // ASSERT
            Assert.Equal("Bilet tapılmadı!", result);
        }

        [Fact]
        public async Task DeleteAsync_WhenTicketExists_ReturnsSuccess()
        {
            // ARRANGE
            var fakeTicket = new Ticket { TicketId = 1, Type = "VIP", Price = 50, QuantityAvailable = 10, EventId = 1 };
            _ticketRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeTicket);
            _ticketRepo.Setup(r => r.DeleteAsync(fakeTicket)).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.DeleteAsync(1);

            // ASSERT
            Assert.Equal("Bilet uğurla silindi!", result);
            _ticketRepo.Verify(r => r.DeleteAsync(fakeTicket), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenTicketNotFound_ReturnsError()
        {
            // ARRANGE
            _ticketRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Ticket?)null);

            // ACT
            var result = await _service.UpdateAsync(1, new UpdateTicketDto { Type = "VIP", Price = 10, QuantityAvailable = 5 });

            // ASSERT
            Assert.Equal("Bilet tapılmadı!", result);
        }

        [Fact]
        public async Task UpdateAsync_WhenTicketExists_ReturnsSuccess()
        {
            // ARRANGE
            var fakeTicket = new Ticket { TicketId = 1, Type = "VIP", Price = 50, QuantityAvailable = 10, EventId = 1 };
            _ticketRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeTicket);
            _ticketRepo.Setup(r => r.UpdateAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            var dto = new UpdateTicketDto { Type = "Premium", Price = 100, QuantityAvailable = 20 };

            // ACT
            var result = await _service.UpdateAsync(1, dto);

            // ASSERT
            Assert.Equal("Bilet uğurla yeniləndi!", result);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoTickets_ReturnsEmptyList()
        {
            // ARRANGE
            _ticketRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Ticket>());

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            // ARRANGE
            _ticketRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Ticket?)null);

            // ACT
            var result = await _service.GetByIdAsync(1);

            // ASSERT
            Assert.Null(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public async Task CreateAsync_WithInvalidEventIds_ReturnsError(int invalidId)
        {
            // ARRANGE
            _eventRepo.Setup(r => r.GetByIdAsync(invalidId)).ReturnsAsync((Event?)null);
            var dto = new CreateTicketDto { Type = "VIP", Price = 10, QuantityAvailable = 5 };

            // ACT
            var result = await _service.CreateAsync(invalidId, dto);

            // ASSERT
            Assert.Equal("Bu ID-li event mövcud deyil!", result);
        }
    }

    // ═══════════════════════════════════════════
    // EventService Unit Testləri
    // ═══════════════════════════════════════════
    public class EventServiceUnitTests
    {
        private readonly Mock<IEventRepository> _eventRepo;
        private readonly Mock<IOrganizerRepository> _organizerRepo;
        private readonly IMapper _mapper;
        private readonly EventService _service;

        public EventServiceUnitTests()
        {
            _eventRepo = new Mock<IEventRepository>();
            _organizerRepo = new Mock<IOrganizerRepository>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
            _service = new EventService(_eventRepo.Object, _organizerRepo.Object, _mapper);
        }

        [Fact]
        public async Task CreateAsync_WhenOrganizerNotFound_ReturnsError()
        {
            // ARRANGE
            _organizerRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Organizer?)null);
            var dto = new CreateEventDto { Title = "Test", Date = DateTime.UtcNow.AddDays(5), Location = "Baku", OrganizerId = 99 };

            // ACT
            var result = await _service.CreateAsync(dto);

            // ASSERT
            Assert.Equal("Bu ID-li organizer mövcud deyil!", result);
        }

        [Fact]
        public async Task CreateAsync_WhenOrganizerExists_ReturnsSuccess()
        {
            // ARRANGE
            var fakeOrg = new Organizer { OrganizerId = 1, Name = "Orq", Email = "orq@test.com" };
            _organizerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeOrg);
            _eventRepo.Setup(r => r.AddAsync(It.IsAny<Event>())).Returns(Task.CompletedTask);
            var dto = new CreateEventDto { Title = "Baku Jazz", Date = DateTime.UtcNow.AddDays(10), Location = "Baku", OrganizerId = 1 };

            // ACT
            var result = await _service.CreateAsync(dto);

            // ASSERT
            Assert.Equal("Event uğurla əlavə edildi!", result);
            _eventRepo.Verify(r => r.AddAsync(It.IsAny<Event>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenEventNotFound_ReturnsError()
        {
            // ARRANGE
            _eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Event?)null);

            // ACT
            var result = await _service.DeleteAsync(1);

            // ASSERT
            Assert.Equal("Event tapılmadı!", result);
        }

        [Fact]
        public async Task DeleteAsync_WhenEventExists_ReturnsSuccess()
        {
            // ARRANGE
            var fakeEvent = new Event { EventId = 1, Title = "Test", Date = DateTime.UtcNow.AddDays(5), Location = "Baku", OrganizerId = 1 };
            _eventRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeEvent);
            _eventRepo.Setup(r => r.DeleteAsync(fakeEvent)).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.DeleteAsync(1);

            // ASSERT
            Assert.Equal("Event uğurla silindi!", result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            // ARRANGE
            _eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Event?)null);

            // ACT
            var result = await _service.GetByIdAsync(1);

            // ASSERT
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_WhenEventNotFound_ReturnsError()
        {
            // ARRANGE
            _eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Event?)null);
            var dto = new UpdateEventDto { Title = "New", Date = DateTime.UtcNow.AddDays(5), Location = "Baku", OrganizerId = 1 };

            // ACT
            var result = await _service.UpdateAsync(1, dto);

            // ASSERT
            Assert.Equal("Event tapılmadı!", result);
        }

        [Fact]
        public async Task UpdateAsync_WhenOrganizerNotFound_ReturnsError()
        {
            // ARRANGE
            var fakeEvent = new Event { EventId = 1, Title = "Old", Date = DateTime.UtcNow.AddDays(5), Location = "Baku", OrganizerId = 1 };
            _eventRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeEvent);
            _organizerRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Organizer?)null);
            var dto = new UpdateEventDto { Title = "New", Date = DateTime.UtcNow.AddDays(5), Location = "Baku", OrganizerId = 99 };

            // ACT
            var result = await _service.UpdateAsync(1, dto);

            // ASSERT
            Assert.Equal("Bu ID-li organizer mövcud deyil!", result);
        }
    }

    // ═══════════════════════════════════════════
    // OrganizerService Unit Testləri
    // ═══════════════════════════════════════════
    public class OrganizerServiceUnitTests
    {
        private readonly Mock<IOrganizerRepository> _organizerRepo;
        private readonly IMapper _mapper;
        private readonly OrganizerService _service;

        public OrganizerServiceUnitTests()
        {
            _organizerRepo = new Mock<IOrganizerRepository>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
            _service = new OrganizerService(_organizerRepo.Object, _mapper);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailExists_ReturnsError()
        {
            // ARRANGE
            _organizerRepo.Setup(r => r.EmailExistsAsync("test@mail.com")).ReturnsAsync(true);
            var dto = new CreateOrganizerDto { Name = "MMC", Email = "test@mail.com" };

            // ACT
            var result = await _service.CreateAsync(dto);

            // ASSERT
            Assert.Equal("Bu e-poçt artıq istifadə olunur!", result);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailIsNew_ReturnsSuccess()
        {
            // ARRANGE
            _organizerRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _organizerRepo.Setup(r => r.AddAsync(It.IsAny<Organizer>())).Returns(Task.CompletedTask);
            var dto = new CreateOrganizerDto { Name = "Yeni MMC", Email = "yeni@mail.com" };

            // ACT
            var result = await _service.CreateAsync(dto);

            // ASSERT
            Assert.Equal("Organizer uğurla əlavə edildi!", result);
        }

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ReturnsError()
        {
            // ARRANGE
            _organizerRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Organizer?)null);

            // ACT
            var result = await _service.DeleteAsync(1);

            // ASSERT
            Assert.Equal("Organizer tapılmadı!", result);
        }

        [Fact]
        public async Task DeleteAsync_WhenExists_ReturnsSuccess()
        {
            // ARRANGE
            var fakeOrg = new Organizer { OrganizerId = 1, Name = "Org", Email = "a@b.com" };
            _organizerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeOrg);
            _organizerRepo.Setup(r => r.DeleteAsync(fakeOrg)).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.DeleteAsync(1);

            // ASSERT
            Assert.Equal("Organizer uğurla silindi!", result);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ReturnsError()
        {
            // ARRANGE
            _organizerRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Organizer?)null);

            // ACT
            var result = await _service.UpdateAsync(1, new UpdateOrganizerDto { Name = "X", Email = "x@x.com" });

            // ASSERT
            Assert.Equal("Organizer tapılmadı!", result);
        }
    }

    // ═══════════════════════════════════════════
    // Validator Unit Testləri
    // ═══════════════════════════════════════════
    public class ValidatorUnitTests
    {
        [Fact]
        public async Task TicketValidator_WhenPriceIsZero_ShouldFail()
        {
            var validator = new CreateTicketDtoValidator();
            var dto = new CreateTicketDto { Type = "VIP", Price = 0, QuantityAvailable = 10 };
            var result = await validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Price");
        }

        [Fact]
        public async Task TicketValidator_WhenTypeIsEmpty_ShouldFail()
        {
            var validator = new CreateTicketDtoValidator();
            var dto = new CreateTicketDto { Type = "", Price = 50, QuantityAvailable = 10 };
            var result = await validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Type");
        }

        [Fact]
        public async Task TicketValidator_WhenAllValid_ShouldPass()
        {
            var validator = new CreateTicketDtoValidator();
            var dto = new CreateTicketDto { Type = "Standard", Price = 25, QuantityAvailable = 50 };
            var result = await validator.ValidateAsync(dto);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task EventValidator_WhenDateIsInPast_ShouldFail()
        {
            var validator = new CreateEventDtoValidator();
            var dto = new CreateEventDto { Title = "Test", Date = DateTime.UtcNow.AddDays(-1), Location = "Baku", OrganizerId = 1 };
            var result = await validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Date");
        }

        [Fact]
        public async Task EventValidator_WhenTitleEmpty_ShouldFail()
        {
            var validator = new CreateEventDtoValidator();
            var dto = new CreateEventDto { Title = "", Date = DateTime.UtcNow.AddDays(5), Location = "Baku", OrganizerId = 1 };
            var result = await validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Title");
        }

        [Fact]
        public async Task EventValidator_WhenAllValid_ShouldPass()
        {
            var validator = new CreateEventDtoValidator();
            var dto = new CreateEventDto { Title = "Concert", Date = DateTime.UtcNow.AddDays(10), Location = "Baku", OrganizerId = 1 };
            var result = await validator.ValidateAsync(dto);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task OrganizerValidator_WhenEmailInvalid_ShouldFail()
        {
            var validator = new CreateOrganizerDtoValidator();
            var dto = new CreateOrganizerDto { Name = "MMC", Email = "notanemail" };
            var result = await validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public async Task OrganizerValidator_WhenNameEmpty_ShouldFail()
        {
            var validator = new CreateOrganizerDtoValidator();
            var dto = new CreateOrganizerDto { Name = "", Email = "valid@mail.com" };
            var result = await validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        }
    }
}