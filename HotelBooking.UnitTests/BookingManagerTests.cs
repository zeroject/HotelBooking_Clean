using System;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using Moq;
using System.Collections.Generic;


namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        IRepository<Booking> bookingRepository;
        Mock<IRepository<Booking>> fakeBookRepo;
        Mock<IRepository<Room>> fakeRoomRepo;

        public BookingManagerTests(){
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            bookingRepository = new FakeBookingRepository(start, end);
            var roomRepository = new FakeRoomRepository();
            fakeRoomRepo = new Mock<IRepository<Room>>();
            fakeBookRepo = new Mock<IRepository<Booking>>();
            bookingManager = new BookingManager(bookingRepository, roomRepository);
        }

        [Fact]
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = DateTime.Today;

            // Act
            Action act = () => bookingManager.FindAvailableRoom(date, date);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, date);
            // Assert
            Assert.NotEqual(-1, roomId);
        }

        [Fact]
        public void FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom()
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, date);

            var bookingForReturnedRoomId = bookingRepository.GetAll().Where(
                b => b.RoomId == roomId
                && b.StartDate <= date
                && b.EndDate >= date
                && b.IsActive);
            
            // Assert
            Assert.Empty(bookingForReturnedRoomId);
        }

        [Theory]
        [InlineData(1, typeof(ArgumentException))] // Days extra is positive, expecting ArgumentException
        [InlineData(0, null)] // Days extra is negative, no exception expected
        [InlineData(5000, typeof(ArgumentException))] // High number of extra days, expecting ArgumentException
        [InlineData(4, typeof(ArgumentException))] // A specific number of extra days, expecting ArgumentException
        public void FindAvailableRoom(int daysextra, Type expectedException)
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);

            // Act
            Action act = () => bookingManager.FindAvailableRoom(date.AddDays(daysextra), date);

            if (expectedException != null)
            {
                // Assert exception
                Assert.Throws(expectedException, act);
            }
            else
            {
                // Assert no exception
                var exception = Record.Exception(act);
                Assert.Null(exception);  // No exception should be thrown for this case
            }
        }


        [Theory]
        [InlineData(1, 10, 1)]
        [InlineData(1, 20, 11)]
        [InlineData(1, 1, 0)]
        [InlineData(-30, 0, 0)]
        [InlineData(1, 5, 0)]
        public void GetFullyOccupiedDates_Tests(int startOffsetDays, int endOffsetDays, int expected)
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(startOffsetDays);
            DateTime endDate = DateTime.Today.AddDays(endOffsetDays);

            // Act
            var result = bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Equal(expected, result.Count); // Assert that the number of fully occupied days matches the expected value
        }

        [Fact]
        public void GetFullyOccupiedDates_ThrowsError()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(2);
            DateTime endDate = DateTime.Today.AddDays(1);

            // Act
            Action act = () => bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void CreateBooking_ThrowsError()
        {
            // Arrange
            Booking booking = new Booking();
            booking.StartDate = DateTime.Today.AddDays(1);
            booking.EndDate = DateTime.Today;
            booking.RoomId = 1;

            // Act
            Action act = () => bookingManager.CreateBooking(booking);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void Create_Booking_MOQ()
        {
            // Arrange
            fakeBookRepo.Setup(x => x.Add(It.IsAny<Booking>()));
            bookingManager = new BookingManager(fakeBookRepo.Object, new FakeRoomRepository());
            Booking booking = new Booking();
            booking.StartDate = DateTime.Today.AddDays(1);
            booking.EndDate = DateTime.Today.AddDays(2); // Set the end date to be in the future
            booking.RoomId = 1;

            // Act
            bookingManager.CreateBooking(booking);

            // Assert
            fakeBookRepo.Verify(x => x.Add(It.IsAny<Booking>()), Times.Once);
        }

        [Fact]
        public void GetFullyOccupiedDates_Test_MOQ()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today.AddDays(2);

            fakeBookRepo.Setup(x => x.GetAll()).Returns(new List<Booking>
            {
                new Booking { StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(5), RoomId = 1, IsActive = true },
                new Booking { StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(5), RoomId = 2, IsActive = true }
            });

            fakeRoomRepo.Setup(x => x.GetAll()).Returns(new List<Room>
            {
                new Room { Id = 1, Description = "A" },
                new Room { Id = 2, Description = "B" }
            });

            bookingManager = new BookingManager(fakeBookRepo.Object, fakeRoomRepo.Object);

            // Act
            var result = bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Equal(2, result.Count); // Assert that the number of fully occupied days matches the expected value
        }
    }
}
