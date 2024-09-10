using System;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;


namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        IRepository<Booking> bookingRepository;

        public BookingManagerTests(){
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            bookingRepository = new FakeBookingRepository(start, end);
            IRepository<Room> roomRepository = new FakeRoomRepository();
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

        [Fact]
        public void Create_A_New_Booking()
        {
            // Arrange
            Booking booking = new Booking();
            DateTime date = DateTime.Today.AddDays(1);
            booking.RoomId = bookingManager.FindAvailableRoom(date, date);
            booking.StartDate = date;
            booking.EndDate = date;

            // ACT
            bool succes = bookingManager.CreateBooking(booking);

            // Assert
            Assert.True(succes);
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
    }
}
