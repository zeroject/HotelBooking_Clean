using HotelBooking.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Specs.StepDefinitions
{
    [Binding]
    public class CreatBookingStepDefinition
    {
        private readonly Mock<IRepository<Booking>> mockBookingRepository;
        private readonly Mock<IRepository<Room>> mockRoomRepository;

        BookingManager bookingManager;

        public CreatBookingStepDefinition()
        {
            mockBookingRepository = new Mock<IRepository<Booking>>();
            mockRoomRepository = new Mock<IRepository<Room>>();
            bookingManager = new BookingManager(mockBookingRepository.Object, mockRoomRepository.Object);
        }

        [StepArgumentTransformation]
        public Booking TransformBooking(string bookingDetails)
        {
            // Transform the booking details into a Booking object
            Booking booking = new Booking();
            booking.StartDate = DateTime.Parse(bookingDetails.Split(',')[0]);
            booking.EndDate = DateTime.Parse(bookingDetails.Split(",")[1]);
            booking.IsActive = true;
            return booking;
        }

        [Given("the first room is {int}")]
        public void GivenTheFirstRoomIS(int id)
        {
            Room room = new Room();
            room.Id = id;
        }

        [Given("the second room is {int}")]
        public void GivenTheSecondRoomIS(int id)
        {
            Room room = new Room();
            room.Id = id;
        }

        [When("I create a booking with {string}")]

        public void WhenIcreateAbookingWith(string dates)
        {
            Booking booking = new Booking();
            booking.StartDate = DateTime.Parse(dates.Split(',')[0]);
            booking.EndDate = DateTime.Parse(dates.Split(',')[1]);
        }

        [Then("booking is created with (.*) the result is (.*)")]
        public void ThenTheBookingISCreatedItShouldReturn(Booking booking, bool result)
        {
            bool v = bookingManager.CreateBooking(booking);
            if (result == v) { };
        }
    }
}
